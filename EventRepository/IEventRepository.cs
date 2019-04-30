using EventModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventRepository
{

    public interface IEventRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rt"></param>
        Task DeleteFile(RecordTypes rt);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="fileContents">Fields for the record.</param>
        Task AddRecord(RecordTypes rt, IEnumerable<string> fileContents);

        Task<Itinerary> GetItinerary(string id);
        Task<Registration> GetRegistration(string id);
        Task<Registrant> GetRegistrant(string id);
        Task<Session> GetSession(string id);
        Task<List<Session>> GetAllSessions();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rt"></param>
        Task DeleteRecord(string id, RecordTypes rt);
    }

}
