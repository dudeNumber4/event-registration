using EventModels;
using EventRegistration.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventRegistration.ViewModels
{
    public class ItineraryViewModel : ViewModelBase
    {
        public ItineraryViewModel(RegistrationService registrationService) : base(registrationService) { }
        
        public static ItineraryViewModel Create(RegistrationService registrationService, string registrationId)
            => new(registrationService) { RegistrationId = registrationId };
    }
}
