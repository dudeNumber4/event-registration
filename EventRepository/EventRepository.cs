using EventData;
using EventModels;
using Microsoft.FSharp.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventRepository
{

    /// <summary>
    /// This could quite obviously benefit from caching, not the purpose of this solution.
    /// </summary>
    public class EventRepository
    {

        public async Task UpdateRecord(IEventRecord eventRecord, RecordTypes rt)
        {
            if (eventRecord == null)
            {
                return;
            }
            else
            {
                // Delete and re-add
                var existing = await GetRecord(eventRecord.Id.ToString(), rt);
                if (existing.Any())
                {
                    await DeleteRecord(eventRecord.Id.ToString(), rt).ConfigureAwait(false);
                }

                await AddRecord(rt, eventRecord).ConfigureAwait(false);
            }
        }

        public int NextId(RecordTypes rt) => DataUtils.NextId(RecordTypeConverter.GetFileName(rt));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="recordContents">Fields for the record.</param>
        public async Task<int> AddRecord(RecordTypes rt, IEventRecord record)
        {
            return await Task.Run(() =>
            {
                FSharpList<string> recordValues = ListModule.OfSeq(record.ToBasicRecord());
                string newId = DataUtils.AddRecord(RecordTypeConverter.GetFileName(rt), recordValues);
                return Convert.ToInt32(newId);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rt"></param>
        public async Task DeleteFile(RecordTypes rt) => await Task.Run(() => DataDriver.DeleteFile(RecordTypeConverter.GetFileName(rt)));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rt"></param>
        public async Task DeleteRecord(string id, RecordTypes rt) => await Task.Run(() => DataUtils.DeleteRecord(id, RecordTypeConverter.GetFileName(rt)));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rt"></param>
        public async Task<List<string>> GetRecord(string id, RecordTypes rt) => await Task.FromResult(GetCSharpList(DataUtils.GetRecord(id, RecordTypeConverter.GetFileName(rt))));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public async Task<Registration> GetRegistration(int id)
        {
            var record = await Task.FromResult(GetCSharpList(DataUtils.GetRecord(id.ToString(), RecordTypeConverter.GetFileName(RecordTypes.Registration))));
            return new Registration().FromBasicRecord(record) as Registration;
        }

        public async Task<Registrant> GetRegistrant(int id)
        {
            var record = await Task.FromResult(GetCSharpList(DataUtils.GetRecord(id.ToString(), RecordTypeConverter.GetFileName(RecordTypes.Registrant))));
            return new Registrant().FromBasicRecord(record) as Registrant;
        }

        public async Task<Registrant> GetRegistrant(string email)
        {
            var registrants = await GetAllRegistrants();
            return registrants.FirstOrDefault(r => r.PersonalInfo.Email == email);
        }

        public async Task<Session> GetSession(int id)
        {
            var record = await Task.FromResult(GetCSharpList(DataUtils.GetRecord(id.ToString(), RecordTypeConverter.GetFileName(RecordTypes.Session))));
            return new Session().FromBasicRecord(record) as Session;
        }

        /// <summary>
        /// Apparently I wasn't thinking of performance here.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Session>> GetAllSessions()
        {
            FSharpList<FSharpList<string>> dataSessions = await Task.FromResult(DataUtils.GetAllSessions());
            return GetCSharpList(dataSessions).Select(s => new Session().FromBasicRecord(s) as Session).ToList();
        }
        
        public async Task<List<Registrant>> GetAllRegistrants()
        {
            FSharpList<FSharpList<string>> registrants = await Task.FromResult(DataUtils.GetAllRegistrants());
            return GetCSharpList(registrants).Select(r => new Registrant().FromBasicRecord(r) as Registrant).ToList();
        }

        public async Task<List<Registration>> GetAllRegistrations()
        {
            FSharpList<FSharpList<string>> registrations = await Task.FromResult(DataUtils.GetAllRegistrations());
            return GetCSharpList(registrations).Select(r => new Registration().FromBasicRecord(r) as Registration).ToList();
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
        private List<string> GetCSharpList(FSharpList<string> list) => new List<string>(SeqModule.OfList(list));

    }

}
