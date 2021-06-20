using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventModels;
using EventRepository;

namespace EventRegistration.Services
{

    public class RegistrationService : ServiceBase
    {

        //public async Task<int> AddRegistration(Registration registration)
        //{

        //}

        public async Task<Registration> GetRegistration(int registrantId) => await _eventRepository.GetRegistrationByRegistrantId(registrantId);

    }

}
