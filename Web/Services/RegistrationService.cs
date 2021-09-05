using EventModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventRegistration.Services
{
    public class RegistrationService : ServiceBase
    {
        public Registration GetRegistrationBy(int registrantId) => _eventRepository.GetRegistrationBy(registrantId);
        
        public Registration GetRegistration(int id) => _eventRepository.GetRegistration(id);

        public Registration CreateRegistration(int registrantId)
        {
            var result = new Registration { RegistrantId = registrantId };
            return result with { Id = _eventRepository.AddRecord(EventRepository.RecordTypes.Registration, result) };
        }

        public void AddSession(int registrantId, int sessionId)
        {
            var registration = GetRegistrationBy(registrantId);
            registration.SessionIds.Add(sessionId);
            _eventRepository.UpdateRecord(registration, EventRepository.RecordTypes.Registration);
        }
    }
}
