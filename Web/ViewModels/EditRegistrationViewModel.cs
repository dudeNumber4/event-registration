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
        private string _registrationId;

        public static async Task<EditRegistrationViewModel> CreateAsync(RegistrationService registrationService, RegistrantService registrantService, string registrationId)
        {
            EditRegistrationViewModel result = new(registrationService, registrantService, registrationId);
            if (int.TryParse(registrationId, out var s))
                result.Registration = await registrationService.GetRegistration(s);
            return result;
        }

        public async Task<string> RegistrantName()
        {
            var registrant = await _registrantService.GetRegistrant(_registrationId);
            return registrant?.ToName();
        }

        private EditRegistrationViewModel(RegistrationService registrationService, RegistrantService registrantService, string registrationId) :base(registrationService)
        {
            _registrantService = registrantService;
            _registrationId = registrationId;
        }

    }

}
