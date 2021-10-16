using EventModels;
using EventRepository;

namespace EventRegistration.Services
{
    public class RegistrationService : ServiceBase
    {

        public RegistrationService(IEventRepository eventRepo) : base(eventRepo) { }

        public Registration GetRegistrationBy(int registrantId) => _eventRepository.GetRegistrationBy(registrantId);
        
        public Registration GetRegistration(int id) => _eventRepository.GetRegistration(id);

        public Registration CreateRegistration(int registrantId)
        {
            var result = new Registration { RegistrantId = registrantId };
            return result with { Id = _eventRepository.AddRecord(EventRepository.RecordTypes.Registration, result) };
        }

        public void AddSession(int registrantId, int sessionId) => ChangeSessionList(registrantId, sessionId, true);

        public void RemoveSession(int registrantId, int sessionId) => ChangeSessionList(registrantId, sessionId, false);

        private void ChangeSessionList(int registrantId, int sessionId, bool add)
        {
            var registration = GetRegistrationBy(registrantId);
            if (add)
                registration.SessionIds.Add(sessionId);
            else
                registration.SessionIds.Remove(sessionId);
            _eventRepository.UpdateRecord(registration, EventRepository.RecordTypes.Registration);
        }

    }
}
