using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{

	/// <summary>
	/// Registration becomes itinerary upon selecting session list and posting.
	/// </summary>
	public class Itinerary
	{
		public int Id { get; set; }
		public int RegistrationId { get; set; }
		public SessionList SessionList { get; set; }
	}

}
