using EventModels;
using EventRegistration.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EventRegistration.ViewModels
{
    public class ViewModelBase
    {

        protected RegistrationService _registrationService;

        public Registration Registration { get; set; }

        public ViewModelBase(RegistrationService registrationService) => _registrationService = registrationService;

        public Registration CreateRegistration(int registrationId) => _registrationService.CreateRegistration(registrationId);

    }
}
