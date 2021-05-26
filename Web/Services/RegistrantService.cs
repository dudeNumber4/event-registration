using EventModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventRegistration.Services
{

    public class RegistrantService : ServiceBase
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Registrant> GetRegistrant(string id) => await _eventRepository.GetRegistrant(int.Parse(id));

        public async Task<int> AddRegistrant(Registrant r) => await _eventRepository.AddRecord(EventRepository.RecordTypes.Registrant, r);

    }

}
