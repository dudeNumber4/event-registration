using EventModels;
using EventRegistration.Services;
using System;
using System.Linq;

namespace EventRegistration.ViewModels
{
    public class ViewModelBase
    {

        private string _registrationId;
        protected RegistrationService _registrationService;

        public Registration Registration { get; set; }

        public string RegistrationId 
        { 
            get => _registrationId;
            set
            {
                _registrationId = value;
                if ((_registrationService is not null) && int.TryParse(value, out var s))
                    Registration = _registrationService.GetRegistration(s);
            }
        }

        public ViewModelBase(RegistrationService registrationService) => _registrationService = registrationService;

        public Registration CreateRegistration(int registrationId) => _registrationService.CreateRegistration(registrationId);

        public SessionList PopulateSelectedSessions(SessionService sessionService, string registrationId) => sessionService.PopulateSessions(registrationId, _registrationService, true);

        public string RegistrantName(RegistrantService registrantService)
        {
            if (Registration is null)
                return string.Empty;
            else
            {
                var registrant = registrantService.GetRegistrant(Registration.RegistrantId.ToString());
                return registrant?.ToName();
            }
        }

    }
}
