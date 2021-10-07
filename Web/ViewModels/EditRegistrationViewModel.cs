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

        public static EditRegistrationViewModel Create(RegistrationService registrationService, RegistrantService registrantService, string registrationId)
        {
            EditRegistrationViewModel result = new(registrationService, registrantService, registrationId);
            result.RegistrationId = registrationId;
            return result;
        }

        private EditRegistrationViewModel(RegistrationService registrationService, RegistrantService registrantService, string registrationId) :base(registrationService)
            => _registrantService = registrantService;

        public void AddSession(int sessionId)
        {
            var registrant = _registrantService.GetRegistrant(Registration.RegistrantId.ToString());
            _registrationService.AddSession(registrant.Id, sessionId);
        }
        
        public void RemoveSession(int sessionId)
        {
            var registrant = _registrantService.GetRegistrant(Registration.RegistrantId.ToString());
            _registrationService.RemoveSession(registrant.Id, sessionId);
        }

    }

}
