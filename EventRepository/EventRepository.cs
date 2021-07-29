using EventData;
using EventModels;
using Microsoft.FSharp.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EventRepository
{

    /// <summary>
    /// This could quite obviously benefit from caching, not the purpose of this solution.
    /// </summary>
    public class EventRepository
    {

        public EventRepository(IDataPreparer dataPreparer)
        {
            dataPreparer = dataPreparer ?? new DefaultDataPreparer();
            dataPreparer.Prepare();
        }

        public void UpdateRecord(IEventRecord eventRecord, RecordTypes rt)
        {
            if (eventRecord == null)
            {
                return;
            }
            else
            {
                int id = eventRecord.Id;
                var existing = GetRecord(id.ToString(), rt);
                if (existing.Any())
                    DataUtils.UpdateRecord(RecordTypeConverter.GetFileName(rt), GetFSharpList(eventRecord.ToFullRecord()));
                else
                    throw new Exception($"{nameof(UpdateRecord)}: record of type {rt} with id {id} not found.");
            }
        }

        public int NextId(RecordTypes rt) => DataUtils.NextId(RecordTypeConverter.GetFileName(rt));

        /// <summary>
        /// Assumes NEW record; no incoming id respected.
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="recordContents">Fields for the record.</param>
        public int AddRecord(RecordTypes rt, IEventRecord record)
        {
            FSharpList<string> recordValues = GetFSharpList(record.ToBasicRecord());
            var result = DataUtils.AddRecord(RecordTypeConverter.GetFileName(rt), recordValues);
            Debug.Print($"result: {result}");
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rt"></param>
        public void DeleteFile(RecordTypes rt) => DataDriver.DeleteFile(RecordTypeConverter.GetFileName(rt));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rt"></param>
        public void DeleteRecord(string id, RecordTypes rt) => DataUtils.DeleteRecord(id, RecordTypeConverter.GetFileName(rt));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rt"></param>
        public List<string> GetRecord(string id, RecordTypes rt) => GetCSharpList(DataUtils.GetRecord(id, RecordTypeConverter.GetFileName(rt)));

        public Registration GetRegistration(int id)
        {
            var record = GetCSharpList(DataUtils.GetRecord(id.ToString(), RecordTypeConverter.GetFileName(RecordTypes.Registration)));
            return new Registration().FromBasicRecord(record) as Registration;
        }
        public Registration GetRegistrationBy(int registrantId)
        {
            var registrations = GetAllRegistrations();  // not built for speed
            return registrations.FirstOrDefault(r => r.RegistrantId == registrantId);
        }

        public Registrant GetRegistrant(int id)
        {
            var record = GetCSharpList(DataUtils.GetRecord(id.ToString(), RecordTypeConverter.GetFileName(RecordTypes.Registrant)));
            return new Registrant().FromBasicRecord(record) as Registrant;
        }

        public Registrant GetRegistrant(string email)
        {
            var registrants = GetAllRegistrants();
            return registrants.FirstOrDefault(r => r?.PersonalInfo?.Email == email);
        }

        public Session GetSession(int id)
        {
            var record = GetCSharpList(DataUtils.GetRecord(id.ToString(), RecordTypeConverter.GetFileName(RecordTypes.Session)));
            return new Session().FromBasicRecord(record) as Session;
        }

        /// <summary>
        /// Apparently I wasn't thinking of performance here.
        /// </summary>
        /// <returns></returns>
        public List<Session> GetAllSessions()
        {
            FSharpList<FSharpList<string>> dataSessions = DataUtils.GetAllSessions();
            return GetCSharpList(dataSessions).Select(s => new Session().FromBasicRecord(s) as Session).ToList();
        }
        
        public List<Registrant> GetAllRegistrants()
        {
            FSharpList<FSharpList<string>> registrants = DataUtils.GetAllRegistrants();
            return GetCSharpList(registrants).Select(r => new Registrant().FromBasicRecord(r) as Registrant).ToList();
        }

        public List<Registration> GetAllRegistrations()
        {
            FSharpList<FSharpList<string>> registrations = DataUtils.GetAllRegistrations();
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

        private FSharpList<string> GetFSharpList(IEnumerable<string> list) => ListModule.OfSeq(list);

    }

}
