using EventRepository;
using System;
using System.Linq;

namespace EventRegistration.Services
{

    public abstract class ServiceBase
    {
        protected readonly IEventRepository _eventRepository;
        public ServiceBase(IEventRepository eventRepo) => _eventRepository = eventRepo;
    }

}
