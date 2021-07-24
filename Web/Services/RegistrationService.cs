using EventModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventRegistration.Services
{
    public class RegistrationService : ServiceBase
    {
        public async Task<Registration> GetRegistration(int id)
            => await _eventRepository.GetRegistration(id);

        public async Task<Registration> CreateRegistration(int registrantId)
        {
            var result = new Registration { RegistrantId = registrantId };
            return result with { Id = await _eventRepository.AddRecord(EventRepository.RecordTypes.Registration, result) };
        }
    }
}
