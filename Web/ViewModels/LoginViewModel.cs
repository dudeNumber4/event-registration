﻿using System;
using EventModels;
using EventRegistration.Services;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Linq;

namespace EventRegistration.ViewModels
{
    
    public class LoginViewModel: ViewModelBase
    {
        
        private RegistrantService _registrantService;

        [Required]
        public string Email { get; set; }
        public string ValidationMsg { get; set; }
        public Registrant Registrant { get; set; }

        public LoginViewModel(RegistrantService registrantService, RegistrationService registrationService): base(registrationService) =>
            _registrantService = registrantService;

        public bool RegistrantExists()
        {
            Registrant = _registrantService.GetRegistrantByEmail(Email);
            return Registrant != null;
        }

        public bool RegistrationExists()
        {
            if (Registrant == null)
                return false;
            else
            {
                Registration = _registrationService.GetRegistrationBy(Registrant.Id);
                return Registration != null;
            }
        }

        public bool SessionsExist()
        {
            if (Registrant == null)
                return false;
            else
            {
                Registration = _registrationService.GetRegistrationBy(Registrant.Id);
                return Registration.SessionIds.Any();
            }
        }

    }

}
