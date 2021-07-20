using EventModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventRegistration.Services
{
    public class RegistrationService : ServiceBase
    {
        public async Task<bool> RegistrationExists(int registrantId)
            => await _eventRepository.GetRegistration(registrantId) != null;
    }
}
