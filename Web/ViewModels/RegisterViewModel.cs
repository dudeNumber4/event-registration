using EventModels;
using EventRegistration.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EventRegistration.ViewModels
{
   
    public class RegisterViewModel
    {

        private RegistrantService _registrantService;
        public string ValidationMsg { get; set; }
        public Registrant Registrant { get; set; } = new();

        public RegisterViewModel(RegistrantService registrantService) => _registrantService = registrantService;

        public async Task<bool> Register()
        {
            (bool valid, string reason) = Registrant.IsValid();
            if (valid)
            {
                if (!(await _registrantService.RegistrantExists(Registrant)))
                {
                    await _registrantService.AddRegistrant(Registrant);
                    return true;
                }
                else
                    ValidationMsg = "Registration already exists";
            }
            else
                ValidationMsg = reason;
            return false;
        }

    }
}
