using EventRegistration.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventRegistration.ViewModels
{
    
    public class EditRegistrationViewModel: ViewModelBase
    {

        public EditRegistrationViewModel(RegistrationService registrationService, int registrationId) : base(registrationService) 
        { 
            //Registration = _registrationService.GetRegistration()
        }

    }

}
