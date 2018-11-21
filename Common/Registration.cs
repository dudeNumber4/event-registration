using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{

	/// <summary>
	/// Registrant becomes registration upon signature.
	/// </summary>
	public class Registration
	{
		public int Id { get; set; }
		public int RegistrantId { get; set; }
	}

}
