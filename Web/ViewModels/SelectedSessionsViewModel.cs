using EventRegistration.Services;
using System;
using System.Linq;

namespace EventRegistration.ViewModels
{
    
    public class SelectedSessionsViewModel : ViewModelBase
    {
        readonly SessionService _sessionService;

        public SelectedSessionsViewModel(RegistrationService registrationService, SessionService sessionService) : base(registrationService)
            => _sessionService = sessionService;
    }

}
