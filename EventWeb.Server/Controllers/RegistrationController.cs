using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using EventRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventWeb.Server.Controllers
{

	[Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {

		private IEventRepository _eventRepository;

		public RegistrationController(IEventRepository eventRepository)
		{
			_eventRepository = eventRepository;
		}

		/// <summary>
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<Registration> GetRegistration(string id)
		{
			var registration = await _eventRepository.GetRegistration(id);
			return registration;
		}

    }

}