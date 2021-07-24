using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventModels;
using EventRepository;

namespace EventRegistration.Services
{

    /// <summary>
    /// This "service" is really in place of some API in some very complex separate project/solution
    /// It's only state is the repository in the base class.
    /// </summary>
    public class SessionService: ServiceBase
    {

        public async Task<List<Session>> GetAllSessions(bool createNew = true)
        {
            var sessions = await _eventRepository.GetAllSessions().ConfigureAwait(false);
            if ((sessions.Count == 0) && createNew)
            {
                sessions = await _eventRepository.GetAllSessions().ConfigureAwait(false);
            }
            return sessions;
        }

        public async Task EditSession(Session s)
        {
            await _eventRepository.UpdateRecord(s, RecordTypes.Session).ConfigureAwait(false);
        }

        public async Task<int> AddSession(Session s) => await _eventRepository.AddRecord(RecordTypes.Session, s);
        public async Task UpdateSession(Session s) => await _eventRepository.UpdateRecord(s, RecordTypes.Session);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Session> GetSession(string id)
        {
            var session = await _eventRepository.GetSession(int.Parse(id)).ConfigureAwait(false);
            return session;
        }

        /// <summary>
        /// temp
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAllSessions()
        {
            await _eventRepository.DeleteFile(RecordTypes.Session).ConfigureAwait(false);
        }

        public async Task DeleteSession(int id)
        {
            await _eventRepository.DeleteRecord(id.ToString(), RecordTypes.Session).ConfigureAwait(false);
        }

    }

}
