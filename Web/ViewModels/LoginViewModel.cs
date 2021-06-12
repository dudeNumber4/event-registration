using EventRegistration.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace EventRegistration.ViewModels
{
    public class LoginViewModel
    {
        private RegistrantService _registrantService;

        [Required]
        public string Email { get; set; }
        public string ValidationMsg { get; set; }
        public LoginViewModel(RegistrantService registrantService) => _registrantService = registrantService;
        public bool Valid()
        {
            var registrant = _registrantService.GetRegistrantByEmail(Email).Result;
            return registrant != null;
        }
    }
}
