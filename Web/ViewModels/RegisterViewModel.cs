using EventModels;
using EventRegistration.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EventRegistration.ViewModels
{
   
    public class RegisterViewModel: ViewModelBase
    {

        private RegistrantService _registrantService;
        public string ValidationMsg { get; set; }
        public Registrant Registrant { get; set; } = new();

        public RegisterViewModel(RegistrantService registrantService, RegistrationService registrationService) : base(registrationService) =>
            _registrantService = registrantService;

        public async Task<bool> Register()
        {
            (bool valid, string reason) = Registrant.IsValid();
            if (valid)
            {
                if (!(await _registrantService.RegistrantExists(Registrant)))
                {
                    Registrant.Id = await _registrantService.AddRegistrant(Registrant);
                    return true;
                }
                else
                    ValidationMsg = "Registration already exists, please login";
            }
            else
                ValidationMsg = reason;
            return false;
        }

    }

}
