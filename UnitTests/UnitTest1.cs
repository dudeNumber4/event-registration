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

		[ClassCleanup]
		public static void ClassCleanup()
		{
			DataUtils.DeleteFile(RecordTypes.itineraryFileName);
			DataUtils.DeleteFile(RecordTypes.registrantFileName);
			DataUtils.DeleteFile(RecordTypes.registrationFileName);
			DataUtils.DeleteFile(RecordTypes.sessionFileName);
		}

		[TestMethod]
		[DataRow("Registrant.csv")]
		[DataRow("Registration.csv")]
		[DataRow("Session.csv")]
		[DataRow("Itinerary.csv")]
		public void ProcessRecord(string fileName)
		{
			// add 2 records of the given type
			DataUtils.AddRecord(fileName, ListModule.OfSeq(GetEnumerable(fileName)));
			DataUtils.AddRecord(fileName, ListModule.OfSeq(GetEnumerable(fileName)));

			// retrieve by id (should have fresh ids)
			FSharpList<string> result = DataUtils.GetRecord("1", fileName);
			var list = GetCSharpList(result);
			Assert.AreEqual(GetRecordCount(fileName), list.Count);
			Assert.AreEqual("1", list[0]);
			Assert.AreEqual(GetEnumerable(fileName).First(), list[1]);

			// delete
			DataUtils.DeleteRecord("1", fileName);

			// retrieve nothing
			result = DataUtils.GetRecord("1", fileName);
			list = GetCSharpList(result);
			Assert.IsFalse(result.Any());
		}

		private int GetRecordCount(string fileName)
		{
			switch (fileName)
			{
				case "Itinerary.csv": return 4;
				case "Registrant.csv": return 6;
				case "Registration.csv": return 2;
				case "Session.csv": return 4;
				default: return 0;
			}
		}

		private IEnumerable<string> GetEnumerable(string fileName)
		{
			switch (fileName)
			{
				case "Itinerary.csv": return GetItineraryEnumerable();
				case "Registrant.csv": return GetRegistrantEnumerable();
				case "Registration.csv": return GetRegistrationEnumerable();
				case "Session.csv": return GetSessionEnumerable();
				default: return Enumerable.Empty<string>();
			}
		}

		private IEnumerable<string> GetRegistrantEnumerable()
		{
			yield return "firstName";
			yield return "lastName";
			yield return "email";
			yield return "orgName";
			yield return "industry";
		}

		private IEnumerable<string> GetRegistrationEnumerable()
		{
			yield return "registrant id";
		}

		private IEnumerable<string> GetSessionEnumerable()
		{
			yield return "day";
			yield return "title";
			yield return "description";
		}

		private IEnumerable<string> GetItineraryEnumerable()
		{
			yield return "registration id";
			yield return "session id 1";
			yield return "session id 1";
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

	}

}
