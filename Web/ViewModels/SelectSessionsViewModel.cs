using EventModels;
using EventRegistration.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventRegistration.ViewModels
{
    public class SelectSessionsViewModel : ViewModelBase
    {

        private SessionService _sessionService;

        public SelectSessionsViewModel(RegistrationService registrationService, SessionService sessionService) : base(registrationService)
            => _sessionService = sessionService;

        public SessionList PopulateSessions(string registrationId) => _sessionService.PopulateSessions(registrationId, _registrationService, false);

    }
}
