using EventModels;
using EventRegistration.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventRegistration.ViewModels
{
    public class RegisterViewModel
    {

        private RegistrantService _registrantService;
        private RegistrationService _registrationService;
        public string ValidationMsg { get; set; }
        public Registrant Registrant { get; set; } = new();

        public RegisterViewModel(RegistrantService registrantService, RegistrationService registrationService)
        {
            _registrantService = registrantService;
            _registrationService = registrationService;
        }

        public async Task<bool> Register()
        {
            // validate email
            // validite registration doesn't already exist.
            // Set validation message
        }

    }
}
