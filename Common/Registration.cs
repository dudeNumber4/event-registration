using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{

	/// <summary>
	/// Registrant becomes registration upon signature.
	/// </summary>
	public class Registration: IEventRecord
	{
		public int Id { get; set; }
		public int RegistrantId { get; set; }

		public IEventRecord FromBasicRecord(List<string> record)
		{
			if ((record?.Count == 2) && int.TryParse(record[0], out var id) && int.TryParse(record[1], out var registrantId))
			{
				var result = new Registration
				{
					Id = id,
					RegistrantId = registrantId
				};
				return result;
			}
			else
			{
				return null;
			}
		}

	}

}
