using Common;
using EventData;
using Microsoft.FSharp.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace EventRepository
{

	/// <summary>
	/// async for completeness of sample.
	/// </summary>
	public class EventRepository : IEventRepository
	{

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rt"></param>
		/// <param name="fileContents">Fields for the record.</param>
		public async Task AddRecord(RecordTypes rt, IEnumerable<string> fileContents)
		{
			await Task.Run(() => DataUtils.AddRecord(RecordTypeConverter.RecordTypeToString(rt), ListModule.OfSeq(fileContents)));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rt"></param>
		public async Task DeleteFile(RecordTypes rt)
		{
			await Task.Run(() => DataUtils.DeleteFile(RecordTypeConverter.RecordTypeToString(rt)));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="rt"></param>
		public async Task DeleteRecord(string id, RecordTypes rt)
		{
			await Task.Run(() => DataUtils.DeleteRecord(id, RecordTypeConverter.RecordTypeToString(rt)));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="rt"></param>
		public async Task<List<string>> GetRecord(string id, RecordTypes rt)
		{
			var result = await Task.FromResult(GetCSharpList(DataUtils.GetRecord(id, RecordTypeConverter.RecordTypeToString(rt))));
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		public async Task<Itinerary> GetItinerary(string id)
		{
			var record = await Task.FromResult(GetCSharpList(DataUtils.GetRecord(id, RecordTypeConverter.RecordTypeToString(RecordTypes.Itinerary))));
			return new Itinerary().FromBasicRecord(record) as Itinerary;
		}

		public async Task<Registration> GetRegistration(string id)
		{
			var record = await Task.FromResult(GetCSharpList(DataUtils.GetRecord(id, RecordTypeConverter.RecordTypeToString(RecordTypes.Registration))));
			return new Registration().FromBasicRecord(record) as Registration;
		}

		public async Task<Registrant> GetRegistrant(string id)
		{
			var record = await Task.FromResult(GetCSharpList(DataUtils.GetRecord(id, RecordTypeConverter.RecordTypeToString(RecordTypes.Registrant))));
			return new Registrant().FromBasicRecord(record) as Registrant;
		}

		public async Task<Session> GetSession(string id)
		{
			var record = await Task.FromResult(GetCSharpList(DataUtils.GetRecord(id, RecordTypeConverter.RecordTypeToString(RecordTypes.Session))));
			return new Session().FromBasicRecord(record) as Session;
		}

		public async Task<List<Session>> GetAllSessions()
		{
			FSharpList<FSharpList<string>> dataSessions = await Task.FromResult(DataUtils.GetAllSessions());
			return GetCSharpList(dataSessions).Select(s => new Session().FromBasicRecord(s) as Session).ToList();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="list">F# list of F# List of string</param>
		/// <returns>C# list of list of string</returns>
		private List<List<string>> GetCSharpList(FSharpList<FSharpList<string>> list)
		{
			var result = new List<List<string>>(list.Count());
			IEnumerable<FSharpList<string>> enumerable = SeqModule.OfList(list);
			foreach (var item in enumerable)
			{
				result.Add(GetCSharpList(item));
			}
			return result;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="list">F# list</param>
		/// <returns>C# list</returns>
		private List<string> GetCSharpList(FSharpList<string> list)
		{
			IEnumerable<string> enumerable = SeqModule.OfList(list);
			return new List<string>(enumerable);
		}

	}

}
