using EventRegistration.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventRegistration.ViewModels
{
    
    public class EditRegistrationViewModel: ViewModelBase
    {

        private RegistrantService _registrantService;

        public static EditRegistrationViewModel CreateAsync(RegistrationService registrationService, RegistrantService registrantService, string registrationId)
        {
            EditRegistrationViewModel result = new(registrationService, registrantService, registrationId);
            if (int.TryParse(registrationId, out var s))
                result.Registration = registrationService.GetRegistration(s);
            return result;
        }

        public string RegistrantName()
        {
            if (Registration is null)
                return string.Empty;
            else
            {
                var registrant = _registrantService.GetRegistrant(Registration.RegistrantId.ToString());
                return registrant?.ToName();
            }
        }

        private EditRegistrationViewModel(RegistrationService registrationService, RegistrantService registrantService, string registrationId) :base(registrationService)
            => _registrantService = registrantService;

    }

}
