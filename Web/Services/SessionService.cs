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

        public List<Session> GetAllSessions() => _eventRepository.GetAllSessions();

        public void EditSession(Session s) => _eventRepository.UpdateRecord(s, RecordTypes.Session);

        public int AddSession(Session s) => _eventRepository.AddRecord(RecordTypes.Session, s);
        public void UpdateSession(Session s) => _eventRepository.UpdateRecord(s, RecordTypes.Session);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Session GetSession(string id) => _eventRepository.GetSession(int.Parse(id));

        /// <summary>
        /// temp
        /// </summary>
        /// <returns></returns>
        public void DeleteAllSessions() => _eventRepository.DeleteFile(RecordTypes.Session);

        public void DeleteSession(int id) => _eventRepository.DeleteRecord(id.ToString(), RecordTypes.Session);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registrationId"></param>
        /// <param name="registrationService"></param>
        /// <param name="registeredFor">If true, return sessions registered for, else sessions not registered for</param>
        /// <returns></returns>
        public SessionList PopulateSessions(string registrationId, RegistrationService registrationService, bool registeredFor)
        {
            if (int.TryParse(registrationId, out var id))
            {
                var registration = registrationService.GetRegistration(id);
                var sessions = GetAllSessions();
                return new SessionList(sessions?.Where(s => registeredFor ? registration.SessionIds.Contains(s.Id) : !registration.SessionIds.Contains(s.Id)).ToList());
            }
            return new SessionList(new());
        }

    }

}
