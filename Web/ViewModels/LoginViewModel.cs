using System;
using EventModels;
using EventRegistration.Services;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

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

        public async Task<bool> RegistrantExists()
        {
            Registrant = await _registrantService.GetRegistrantByEmail(Email);
            return Registrant != null;
        }

        public async Task<bool> RegistrationExists()
        {
            if (Registrant == null)
                return false;
            else
            {
                // WRONG!!  get registration by registrant id.
                Registration = await _registrationService.GetRegistration(Registrant.Id);
                return Registration != null;
            }
        }

    }

}
