using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
	public interface IEventRecord
	{
		int Id { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="record">Data returned from EventRepository for a given file type</param>
		/// <returns></returns>
		IEventRecord FromBasicRecord(List<string> record);
	}
}
