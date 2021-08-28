using EventModels;
using EventRegistration.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventRegistration.ViewModels
{
    public class SelectedSessionsViewModel : ViewModelBase
    {

        private SessionService _sessionService;

        public SelectedSessionsViewModel(RegistrationService registrationService, SessionService sessionService) : base(registrationService)
            => _sessionService = sessionService;

        public SessionList PopulateSessions(string registrationId) => _sessionService.PopulateSessions(registrationId, _registrationService, true);

    }
}
