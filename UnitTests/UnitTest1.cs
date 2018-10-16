using Microsoft.VisualStudio.TestTools.UnitTesting;
using EventData;
using FSharpx.Collections;
using Microsoft.FSharp.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UnitTests
{

	[TestClass]
	public class UnitTest1
	{

		[TestMethod]
		public void ItineraryRecord()
		{
			// add the record
			DataUtils.AddRecord(RecordTypes.itineraryFileName, GetItineraryRecord());

			// retrieve
			FSharpList<string> result = DataUtils.GetRecord("1", RecordTypes.itineraryFileName);
			var list = GetCSharpList(result);
			Assert.AreEqual(4, list.Count);
			Assert.AreEqual("1", list[0]);
			Assert.AreEqual("2", list[1]);
		}

		[TestMethod]
		public void SessionRecord()
		{
			// add the record
			DataUtils.AddRecord(RecordTypes.sessionFileName, GetSessionRecord());

			// retrieve
			FSharpList<string> result = DataUtils.GetRecord("1", RecordTypes.sessionFileName);
			var list = GetCSharpList(result);
			Assert.AreEqual(4, list.Count);
			Assert.AreEqual("1", list[0]);
			Assert.AreEqual("2", list[1]);
		}

		[TestMethod]
		public void RegistrantRecord()
		{
			// add the record
			DataUtils.AddRecord(RecordTypes.registrantFileName, GetRegistrantRecord());

			// retrieve
			FSharpList<string> result = DataUtils.GetRecord("1", RecordTypes.registrantFileName);
			var list = GetCSharpList(result);
			Assert.AreEqual(6, list.Count);
			Assert.AreEqual("1", list[0]);
			Assert.AreEqual("2", list[1]);
		}

		[TestMethod]
		public void RegistrationRecord()
		{
			// add the record
			DataUtils.AddRecord(RecordTypes.registrationFileName, GetRegistrationRecord());

			// retrieve
			FSharpList<string> result = DataUtils.GetRecord("1", RecordTypes.registrationFileName);
			var list = GetCSharpList(result);
			Assert.AreEqual("1", list[0]);
			Assert.AreEqual("2", list[1]);
		}

		/// <summary>
		/// ToDo: Centralize this when we start using it for real.
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		private List<string> GetCSharpList(FSharpList<string> list)
		{
			IEnumerable<string> enumerable = SeqModule.OfList(list);
			return new List<string>(enumerable);
		}

		private FSharpList<string> GetItineraryRecord()
		{
			return ListModule.OfSeq(GetSessionRecordEnumerable());  // 4 items will test itinerary
		}

		private FSharpList<string> GetSessionRecord()
		{
			// ListModule is F# list.
			return ListModule.OfSeq(GetSessionRecordEnumerable());
		}

		private FSharpList<string> GetRegistrantRecord()
		{
			return ListModule.OfSeq(GetRegistrantRecordEnumerable());
		}

		private FSharpList<string> GetRegistrationRecord()
		{
			return ListModule.OfSeq(GetRegistrationRecordEnumerable());
		}

		/// <summary>
		/// Just get 4 fields.
		/// </summary>
		private IEnumerable<string> GetSessionRecordEnumerable()
		{
			return GetRegistrationRecordEnumerable().Concat(GetRegistrationRecordEnumerable());
		}

		/// <summary>
		/// Just get 6 fields.
		/// </summary>
		private IEnumerable<string> GetRegistrantRecordEnumerable()
		{
			return GetRegistrationRecordEnumerable().Concat(GetRegistrationRecordEnumerable()).Concat(GetRegistrationRecordEnumerable());
		}

		/// <summary>
		/// A registration record is just 2 ids.
		/// </summary>
		private IEnumerable<string> GetRegistrationRecordEnumerable()
		{
			yield return "1";
			yield return "2";
		}

	}

}
