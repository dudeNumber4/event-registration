using EventModels;
using EventRepository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EventRepo = EventRepository.EventRepository;

namespace UnitTests
{

    /// <summary>
    /// Tests for the data access (F# simple data) methods.
    /// </summary>
    [TestClass]
    public class EventRepositoryTests
    {

        // Last time I checked, MSTest doesn't have a very good story for async tests.
        private static object _lock = new object();
        internal const int FIRST_RECORD_ID = 1;
        private static EventRepo _eventRepository;

        [ClassInitialize]
        public static void ClassInit(TestContext tc) => _eventRepository = new EventRepo(new TempDataPreparer());

        [TestMethod]
        public void GetAllSessions()
        {
            lock (_lock)
            {
                Task.WaitAll(new Task[] { _eventRepository.AddRecord(RecordTypes.Session, GetNew(RecordTypes.Session)) });
                Task.WaitAll(new Task[] { _eventRepository.AddRecord(RecordTypes.Session, GetNew(RecordTypes.Session)) });
                List<Session> result = _eventRepository.GetAllSessions().Result;
                Assert.IsTrue(result.Count >= 2);
                ValidateEventRecord(result[result.Count - 2], RecordTypes.Session);
                ValidateEventRecord(result[result.Count - 1], RecordTypes.Session);
                Assert.AreNotEqual(result[result.Count - 2].Id, result[result.Count - 1].Id);
            }
        }

        [TestMethod]
        [DataRow(RecordTypes.Registrant)]
        [DataRow(RecordTypes.Session)]
        [DataRow(RecordTypes.Registration)]
        public void AddConcreteTypes(RecordTypes rt)
        {
            lock (_lock)
            {
                IEventRecord eventObject = rt switch
                {
                    RecordTypes.Registration => GetNewRegistration(),
                    RecordTypes.Registrant => GetNewRegistrant(),
                    RecordTypes.Session => GetNewSession(),
                    _ => null
                };

                int newId = rt switch
                {
                    RecordTypes.Registration => _eventRepository.AddRecord(RecordTypes.Registration, eventObject).Result,
                    RecordTypes.Registrant => _eventRepository.AddRecord(RecordTypes.Registrant, eventObject).Result,
                    RecordTypes.Session => _eventRepository.AddRecord(RecordTypes.Session, eventObject).Result,
                    _ => 0
                };

                // retrieve it; ensure it was saved correctly
                var eventRecord = GetEventRecord(rt, newId);
                ValidateEventRecord(eventRecord, rt);
            }
        }

        [TestMethod]
        [DataRow(RecordTypes.Registrant)]
        [DataRow(RecordTypes.Session)]
        [DataRow(RecordTypes.Registration)]
        public void ProcessRecord(RecordTypes rt)
        {
            lock (_lock)
            {
                // add 2 records of the given type
                Task.WaitAll(new Task[] { _eventRepository.AddRecord(rt, GetNew(rt)) });
                Task.WaitAll(new Task[] { _eventRepository.AddRecord(rt, GetNew(rt)) });

                // retrieve by id (should have fresh ids)
                var eventRecord = GetEventRecord(rt);
                Assert.IsNotNull(eventRecord);
                Assert.AreEqual(FIRST_RECORD_ID, eventRecord.Id);
                ValidateEventRecord(eventRecord, rt);

                // delete
                Task.WaitAll(new Task[] { _eventRepository.DeleteRecord(FIRST_RECORD_ID.ToString(), rt) });

                // retrieve nothing
                eventRecord = GetEventRecord(rt);
                Assert.IsNull(eventRecord);
            }
        }

        [TestMethod]
        public void DeleteSession()
        {
            lock (_lock)
            {
                Task.WaitAll(new Task[] { _eventRepository.AddRecord(RecordTypes.Session, GetNew(RecordTypes.Session)) });
                Task.WaitAll(new Task[] { _eventRepository.AddRecord(RecordTypes.Session, GetNew(RecordTypes.Session)) });
                List<Session> result = _eventRepository.GetAllSessions().Result;
                var sessionCount = result.Count;
                Assert.IsTrue(sessionCount >= 2);
                // delete one and refresh.
                _eventRepository.DeleteRecord(result[0].Id.ToString(), RecordTypes.Session).Wait();
                result = _eventRepository.GetAllSessions().Result;
                Assert.IsTrue(result.Count == (sessionCount - 1));
            }
        }

        [TestMethod]
        public void UpdateSession()
        {
            const DayOfWeek testDay = DayOfWeek.Monday;
            const string testTitle = nameof(testTitle);
            const string testDesc = nameof(testDesc);
            lock (_lock)
            {
                // Add 2 sessions
                Task.WaitAll(new Task[] { _eventRepository.AddRecord(RecordTypes.Session, GetNew(RecordTypes.Session)) });
                Task.WaitAll(new Task[] { _eventRepository.AddRecord(RecordTypes.Session, GetNew(RecordTypes.Session)) });
                List<Session> result = _eventRepository.GetAllSessions().Result;
                Assert.IsTrue(result.Count >= 2);
                // edit; refresh.
                Session session = result[result.Count - 1];
                int initialSessionId = session.Id;
                session.Day = testDay;
                session.Description = testDesc;
                session.Title = testTitle;
                _eventRepository.UpdateRecord(session, RecordTypes.Session).Wait();
                result = _eventRepository.GetAllSessions().Result;
                Assert.IsTrue(result.Count >= 2);
                session = result.FirstOrDefault(s => s.Title == testTitle);
                Assert.IsNotNull(session);
                Assert.AreEqual(initialSessionId, session.Id);
                Assert.AreEqual(testDay, session.Day);
                Assert.AreEqual(testDesc, session.Description);
            }
        }

        [TestMethod]
        public void GetSessionsBeforeInitialized()
        {
            lock (_lock)
            {
                _eventRepository.DeleteFile(RecordTypes.Session).Wait();
                List<Session> sessions = _eventRepository.GetAllSessions().Result; // don't blow up.
                Assert.IsFalse(sessions.Any());
            }
        }

        [TestMethod]
        public void GetRegistrantByEmail()
        {
            lock (_lock)
            {
                var registrantId = _eventRepository.AddRecord(RecordTypes.Registrant, GetNew(RecordTypes.Registrant)).Result;
                var registrant = _eventRepository.GetRegistrant(registrantId).Result;
                var result = _eventRepository.GetRegistrant(registrant.PersonalInfo.Email).Result;
                Assert.AreEqual(registrant, result);
            }
        }

        [TestMethod]
        public void RegistrationShouldBeValidWithoutSessionList()
        {
            lock (_lock)
            {
                Registration r = GetNewRegistration();
                r.SessionIds.Clear();
                var registrationId = _eventRepository.NextId(RecordTypes.Registration);
                r.Id = registrationId;
                _eventRepository.AddRecord(RecordTypes.Registration, r).Wait();
                r = _eventRepository.GetRegistration(r.Id).Result;
                Assert.AreEqual(r.SessionIds.Count, 0);
            }
        }

        private void ValidateEventRecord(IEventRecord eventRecord, RecordTypes rt)
        {
            switch (rt)
            {
                case RecordTypes.Registration:
                    ValidateRegistration(eventRecord as Registration);
                    break;
                case RecordTypes.Registrant:
                    ValidateRegistrant(eventRecord as Registrant);
                    break;
                case RecordTypes.Session:
                    ValidateSession(eventRecord as Session);
                    break;
                default:
                    Assert.Fail($"You added another type to {nameof(RecordTypes)}");
                    break;
            }
        }

        private IEventRecord GetEventRecord(RecordTypes rt, int id = FIRST_RECORD_ID)
        {
            IEventRecord result = null;
            switch (rt)
            {
                // When I had the code simply return what result is set to below, it was always null.
                // This even though .Result is a blocking call.  Something extremely screwy with the task system going on there.
                case RecordTypes.Registration:
                    result = _eventRepository.GetRegistration(id).Result;
                    break;
                case RecordTypes.Registrant:
                    result = _eventRepository.GetRegistrant(id).Result;
                    break;
                case RecordTypes.Session:
                    result = _eventRepository.GetSession(id).Result;
                    break;
                default:
                    return null;
            }
            return result;
        }

        internal static IEventRecord GetNew(RecordTypes rt) => rt switch
        {
            RecordTypes.Registration => GetNewRegistration(),
            RecordTypes.Registrant => GetNewRegistrant(),
            RecordTypes.Session => GetNewSession(),
            _ => null
        };

        private static Registrant GetNewRegistrant() => new Registrant 
        { 
            EmploymentInfo = new Employment { Industry = nameof(Employment.Industry), OrgName = nameof(Employment.OrgName) },
            PersonalInfo = new Personal { Email = nameof(Personal.Email), FirstName = nameof(Personal.FirstName), LastName = nameof(Personal.LastName) }
        };

        private static Session GetNewSession() => new() { Day = DayOfWeek.Friday, Title = "title", Description = "description" };

        private static Registration GetNewRegistration() => new() { Id = FIRST_RECORD_ID, RegistrationId = FIRST_RECORD_ID, SessionIds = new List<int> { FIRST_RECORD_ID } };

        void ValidateSession(Session session)
        {
            Assert.IsNotNull(session);
            Assert.IsTrue(session.Id > 0);
            Assert.IsFalse(string.IsNullOrEmpty(session.Title));
            Assert.IsFalse(string.IsNullOrEmpty(session.Description));
        }

        void ValidateRegistration(Registration r)
        {
            Assert.IsNotNull(r);
            Assert.IsTrue(r.Id > 0);
            Assert.IsTrue(r.RegistrationId > 0);
            Assert.IsNotNull(r.SessionIds);
        }

        void ValidateRegistrant(Registrant registrant)
        {
            Assert.IsNotNull(registrant);
            Assert.IsNotNull(registrant.PersonalInfo);
            Assert.IsFalse(string.IsNullOrEmpty(registrant.PersonalInfo.Email));
            Assert.IsFalse(string.IsNullOrEmpty(registrant.PersonalInfo.FirstName));
            Assert.IsFalse(string.IsNullOrEmpty(registrant.PersonalInfo.LastName));
            Assert.IsNotNull(registrant.EmploymentInfo);
            Assert.IsFalse(string.IsNullOrEmpty(registrant.EmploymentInfo.Industry));
            Assert.IsFalse(string.IsNullOrEmpty(registrant.EmploymentInfo.OrgName));
        }

    }

}
