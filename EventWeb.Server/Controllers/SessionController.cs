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
    public class SessionController : ControllerBase
    {

		private IEventRepository _eventRepository;

		public SessionController(IEventRepository eventRepository)
		{
			_eventRepository = eventRepository;
		}

		public async Task<List<Session>> GetAllSessions()
		{
			var sessions = await _eventRepository.GetAllSessions();
			if (sessions.Count == 0)
			{
				await LoadSampleSessions();
				sessions = await _eventRepository.GetAllSessions();
			}
			return sessions;
		}

		/// <summary>
		/// RESUME: Let's get this calling through to the client with the type.
		/// Prolly, we'll want to post up with the type and have that go through as is to the repository.  Which means the repo will have to change those from records to types as well.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<Session> GetSession(string id)
		{
			var session = await _eventRepository.GetSession(id);
			return session;
		}

		private async Task LoadSampleSessions()
		{
			await _eventRepository.AddRecord(RecordTypes.Session, GetSampleSessionEnumerable(1));
			await _eventRepository.AddRecord(RecordTypes.Session, GetSampleSessionEnumerable(2));
		}

		private static IEnumerable<string> GetSampleSessionEnumerable(int i)
		{
			yield return ((DayOfWeek)i).ToString();
			yield return $"Some Title {i}";
			yield return $"Some Description {i}";
		}

	}

}