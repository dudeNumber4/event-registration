using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{

	public class Session
	{
		public int Id { get; set; }
		public DayOfWeek Day { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
	}

}
