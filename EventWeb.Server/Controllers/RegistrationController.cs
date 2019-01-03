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
		/// RESUME: Let's get this calling through to the client with the type.
		/// Prolly, we'll want to post up with the type and have that go through as is to the repository.  Which means the repo will have to change those from records to types as well.
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