using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventRepository;
using EventRepo = EventRepository.EventRepository;

namespace EventRegistration.Services
{

    public abstract class ServiceBase
    {
        protected EventRepo _eventRepository = new EventRepo();
    }

}
