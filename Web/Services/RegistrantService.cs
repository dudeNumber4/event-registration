﻿using EventModels;
using EventRepository;

namespace EventRegistration.Services
{

    public class RegistrantService : ServiceBase
    {

        public RegistrantService(IEventRepository eventRepo) : base(eventRepo) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Registrant GetRegistrant(string id) => _eventRepository.GetRegistrant(int.Parse(id));
        
        public Registrant GetRegistrantByEmail(string email) => _eventRepository.GetRegistrant(email);

        public int AddRegistrant(Registrant r) => _eventRepository.AddRecord(EventRepository.RecordTypes.Registrant, r);

        public bool RegistrantExists(Registrant r)
            => GetRegistrantByEmail(r?.PersonalInfo?.Email) != null;

    }

}
