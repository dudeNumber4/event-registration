using EventModels;
using EventRegistration.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace EventRegistration.ViewModels
{
    
    public class LoginViewModel
    {
        
        private RegistrantService _registrantService;
        private RegistrationService _registrationService;

        [Required]
        public string Email { get; set; }
        public string ValidationMsg { get; set; }
        public Registrant Registrant { get; set; }

        public LoginViewModel(RegistrantService registrantService, RegistrationService registrationService)
        {
            _registrantService = registrantService;
            _registrationService = registrationService;
        }

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
                return await _registrationService.RegistrationExists(Registrant.Id);
        }

    }

}
