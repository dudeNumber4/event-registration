using EventModels;
using EventData;
using Microsoft.FSharp.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

namespace EventRepository
{

    /// <summary>
    /// async for completeness of sample.
    /// </summary>
    public class EventRepository
    {

        public bool DataFileExists() => DataUtils.DataFileExists();

        public async Task UpdateRecord(IEventRecord eventRecord, RecordTypes rt)
        {
            if (eventRecord == null)
            {
                return;
            }
            else
            {
                var existing = await GetRecord(eventRecord.Id.ToString(), rt);
                if (existing.Any())
                {
                    await DeleteRecord(eventRecord.Id.ToString(), rt);
                }


                // resume here; don't think switch expressions work yet.
                //Task x = rt switch
                //{
                //    RecordTypes.Itinerary => await AddRecord(RecordTypes.Itinerary, ItineraryContents(eventRecord as Itinerary)),
                //    RecordTypes.Session => await AddRecord(RecordTypes.Session, SessionContents(eventRecord as Session)),
                //    RecordTypes.Registration => await AddRecord(RecordTypes.Registration, RegistrationContents(eventRecord as Registration)),
                //    RecordTypes.Registrant => await AddRecord(RecordTypes.Registrant, RegistrantContents(eventRecord as Registrant))
                //};
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="fileContents">Fields for the record.</param>
        public async Task AddRecord(RecordTypes rt, IEnumerable<string> fileContents)
        {
            await Task.Run(() => DataUtils.AddRecord(RecordTypeConverter.GetFileName(rt), ListModule.OfSeq(fileContents)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public async Task<bool> AddItinerary(Itinerary i)
        {
            Debug.Assert(i != null);
            Debug.Assert(i.RegistrationId > 0);
            var registration = await GetRecord(i.RegistrationId.ToString(), RecordTypes.Registration);
            if (!registration.Any())
            {
                return await Task.FromResult(false);
            }
            else
            {
                await AddRecord(RecordTypes.Itinerary, ItineraryContents(i));
                return await Task.FromResult(true);
            }
        }

        public async Task AddRegistration(Registration r)
        {
            await AddRecord(RecordTypes.Registration, RegistrationContents(r));
        }

        public async Task AddRegistrant(Registrant r)
        {
            await AddRecord(RecordTypes.Registrant, RegistrantContents(r));
        }

        public async Task AddSession(Session s)
        {
            await AddRecord(RecordTypes.Session, SessionContents(s));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rt"></param>
        public async Task DeleteFile(RecordTypes rt)
        {
            await Task.Run(() => DataUtils.DeleteFile(RecordTypeConverter.GetFileName(rt)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rt"></param>
        public async Task DeleteRecord(string id, RecordTypes rt)
        {
            await Task.Run(() => DataUtils.DeleteRecord(id, RecordTypeConverter.GetFileName(rt)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rt"></param>
        public async Task<List<string>> GetRecord(string id, RecordTypes rt)
        {
            var result = await Task.FromResult(GetCSharpList(DataUtils.GetRecord(id, RecordTypeConverter.GetFileName(rt))));
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public async Task<Itinerary> GetItinerary(string id)
        {
            var record = await Task.FromResult(GetCSharpList(DataUtils.GetRecord(id, RecordTypeConverter.GetFileName(RecordTypes.Itinerary))));
            return new Itinerary().FromBasicRecord(record) as Itinerary;
        }

        public async Task<Registration> GetRegistration(string id)
        {
            var record = await Task.FromResult(GetCSharpList(DataUtils.GetRecord(id, RecordTypeConverter.GetFileName(RecordTypes.Registration))));
            return new Registration().FromBasicRecord(record) as Registration;
        }

        public async Task<Registrant> GetRegistrant(string id)
        {
            var record = await Task.FromResult(GetCSharpList(DataUtils.GetRecord(id, RecordTypeConverter.GetFileName(RecordTypes.Registrant))));
            return new Registrant().FromBasicRecord(record) as Registrant;
        }

        public async Task<Session> GetSession(string id)
        {
            var record = await Task.FromResult(GetCSharpList(DataUtils.GetRecord(id, RecordTypeConverter.GetFileName(RecordTypes.Session))));
            return new Session().FromBasicRecord(record) as Session;
        }

        public async Task<List<Session>> GetAllSessions()
        {
            FSharpList<FSharpList<string>> dataSessions = await Task.FromResult(DataUtils.GetAllSessions());
            return GetCSharpList(dataSessions).Select(s => new Session().FromBasicRecord(s) as Session).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list">F# list of F# List of string</param>
        /// <returns>C# list of list of string</returns>
        private List<List<string>> GetCSharpList(FSharpList<FSharpList<string>> list)
        {
            var result = new List<List<string>>(list.Count());
            IEnumerable<FSharpList<string>> enumerable = SeqModule.OfList(list);
            foreach (var item in enumerable)
            {
                result.Add(GetCSharpList(item));
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list">F# list</param>
        /// <returns>C# list</returns>
        private List<string> GetCSharpList(FSharpList<string> list)
        {
            IEnumerable<string> enumerable = SeqModule.OfList(list);
            return new List<string>(enumerable);
        }

        private IEnumerable<string> ItineraryContents(Itinerary i)
        {
            //yield return i.Id.ToString(); database generates this
            yield return i.RegistrationId.ToString();
            foreach (var session in i.SessionList)
            {
                yield return session.Id.ToString();
            }
        }

        private IEnumerable<string> RegistrationContents(Registration r)
        {
            //yield return r.Id.ToString(); database generates this
            yield return r.RegistrantId.ToString();
        }

        private IEnumerable<string> RegistrantContents(Registrant r)
        {
            //yield return r.Id.ToString(); database generates this
            yield return r.PersonalInfo?.FirstName;
            yield return r.PersonalInfo?.LastName;
            yield return r.PersonalInfo?.Email;
            yield return r.EmploymentInfo?.OrgName;
            yield return r.EmploymentInfo?.Industry;
        }

        private IEnumerable<string> SessionContents(Session s)
        {
            yield return ((int)s.Day).ToString();
            yield return s.Title;
            yield return s.Description;
        }

    }

}
