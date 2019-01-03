using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System;
using Common;

namespace UnitTests
{

	/// <summary>
	/// Tests of the common models/objects.
	/// </summary>
	[TestClass]
	public class SessionListTests
	{

		private static SessionList GetSingleSessionList() => new SessionList
			{
				new Session{ Day = DayOfWeek.Sunday, Title = "Bobo Loves Cake", Description = "See him eat it", Id = 1 }
			};

		private static SessionList GetMultipleSessionList() => new SessionList
			{
				new Session{ Day = DayOfWeek.Sunday, Title = "Bobo Loves Cake", Description = "See him eat it", Id = 1 },
				new Session{ Day = DayOfWeek.Monday, Title = "Who let the dogs out?", Description = "Who?", Id = 2 },
				new Session{ Day = DayOfWeek.Tuesday, Title = "Whyfore", Description = "You bury me in the cold, cold ground?", Id = 3 }
			};

		[TestMethod]
		public void SessionListSuggestionsDoesntBlowUpOnEmptyList()
		{
			var sessionList = new SessionList();
			var result = sessionList.GetSuggestedSessions();
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public void SessionListGetsSuggestedSessions_SingleSession()
		{
			var sessionList = GetSingleSessionList();
			var result = sessionList.GetSuggestedSessions();
			Assert.IsNotNull(result);
			Assert.IsFalse(result.Count == 0);
		}

		[TestMethod]
		public void SessionListGetsSuggestedSessions_MultipleSessions()
		{
			var sessionList = GetMultipleSessionList();
			var result = sessionList.GetSuggestedSessions();
			Assert.IsNotNull(result);
			Assert.IsFalse(result.Count == 0);
		}

		[TestMethod]
		public void SessionList_GetHashCode()
		{
			var sessionList1 = GetSingleSessionList();
			var sessionList2 = GetMultipleSessionList();
			Assert.AreNotEqual(sessionList1.GetHashCode(), sessionList2.GetHashCode());
		}

	}

}
