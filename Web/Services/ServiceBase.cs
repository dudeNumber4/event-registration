using EventRepository;
using System;
using System.Linq;
using EventRepo = EventRepository.EventRepository;

namespace EventRegistration.Services
{

    public abstract class ServiceBase
    {
        protected EventRepo _eventRepository = new EventRepo(new DefaultDataPreparer());
    }

}
