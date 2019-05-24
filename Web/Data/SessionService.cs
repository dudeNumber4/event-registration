using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventModels;
using EventRepository;

namespace EventRegistration.Data
{

    /// <summary>
    /// This "service" is really in place of some API in some very complex separate project/solution
    /// </summary>
    public class SessionService: ServiceBase
    {

        public async Task<List<Session>> GetAllSessions(bool createNew = true)
        {
            var sessions = await _eventRepository.GetAllSessions();
            if ((sessions.Count == 0) && createNew)
            {
                await LoadSampleSessions();
                sessions = await _eventRepository.GetAllSessions();
            }
            return sessions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Session> GetSession(string id)
        {
            var session = await _eventRepository.GetSession(id);
            return session;
        }

        /// <summary>
        /// temp
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAllSessions()
        {
            await _eventRepository.DeleteFile(RecordTypes.Session);
        }

        public async Task DeleteSession(int id)
        {
            await _eventRepository.DeleteRecord(id.ToString(), RecordTypes.Session);
        }

        private async Task LoadSampleSessions()
        {
            await _eventRepository.AddRecord(RecordTypes.Session, GetSampleSessionEnumerable(2));
            await _eventRepository.AddRecord(RecordTypes.Session, GetSampleSessionEnumerable(1));
        }

        // temp
        private static IEnumerable<string> GetSampleSessionEnumerable(int i)
        {
            yield return ((int)((DayOfWeek)i)).ToString();
            yield return $"Some Title {i}";
            yield return $"Some Description {i}";
        }

    }
}
