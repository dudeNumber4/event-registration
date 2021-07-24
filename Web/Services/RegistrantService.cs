using EventModels;
using System;
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
        
        public async Task<Registrant> GetRegistrantByEmail(string email) => await _eventRepository.GetRegistrant(email);

        public async Task<int> AddRegistrant(Registrant r) => await _eventRepository.AddRecord(EventRepository.RecordTypes.Registrant, r);

        public async Task<bool> RegistrantExists(Registrant r)
            => await GetRegistrantByEmail(r?.PersonalInfo?.Email) != null;

    }

}
