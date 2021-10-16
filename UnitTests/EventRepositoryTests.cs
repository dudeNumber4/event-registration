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
        private static object _lock = new();
        internal const int FIRST_RECORD_ID = 1;
        private static EventRepo _eventRepository;

        [ClassInitialize]
        public static void ClassInit(TestContext _) => _eventRepository = new EventRepo(new TestDataPreparer());

        [TestMethod]
        public void GetAllSessions()
        {
            _eventRepository.AddRecord(RecordTypes.Session, GetNew(RecordTypes.Session));
            _eventRepository.AddRecord(RecordTypes.Session, GetNew(RecordTypes.Session));
            List<Session> result = _eventRepository.GetAllSessions();
            Assert.IsTrue(result.Count >= 2);
            ValidateEventRecord(result[result.Count - 2], RecordTypes.Session);
            ValidateEventRecord(result[result.Count - 1], RecordTypes.Session);
            Assert.AreNotEqual(result[result.Count - 2].Id, result[result.Count - 1].Id);
        }

        [TestMethod]
        [DataRow(RecordTypes.Registrant)]
        [DataRow(RecordTypes.Session)]
        [DataRow(RecordTypes.Registration)]
        public void AddConcreteTypes(RecordTypes rt)
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
                RecordTypes.Registration => _eventRepository.AddRecord(RecordTypes.Registration, eventObject),
                RecordTypes.Registrant => _eventRepository.AddRecord(RecordTypes.Registrant, eventObject),
                RecordTypes.Session => _eventRepository.AddRecord(RecordTypes.Session, eventObject),
                _ => 0
            };

            // retrieve it; ensure it was saved correctly
            var eventRecord = GetEventRecord(rt, newId);
            ValidateEventRecord(eventRecord, rt);
        }

        [TestMethod]
        [DataRow(RecordTypes.Registrant)]
        [DataRow(RecordTypes.Session)]
        [DataRow(RecordTypes.Registration)]
        public void ProcessRecord(RecordTypes rt)
        {
            // add 2 records of the given type
            _eventRepository.AddRecord(rt, GetNew(rt));
            _eventRepository.AddRecord(rt, GetNew(rt));

            // retrieve by id (should have fresh ids)
            var eventRecord = GetEventRecord(rt);
            Assert.IsNotNull(eventRecord);
            Assert.AreEqual(FIRST_RECORD_ID, eventRecord.Id);
            ValidateEventRecord(eventRecord, rt);

            // delete
            _eventRepository.DeleteRecord(FIRST_RECORD_ID.ToString(), rt);

            // retrieve nothing
            eventRecord = GetEventRecord(rt);
            Assert.IsNull(eventRecord);
        }

        [TestMethod]
        public void DeleteSession()
        {
            _eventRepository.AddRecord(RecordTypes.Session, GetNew(RecordTypes.Session));
            _eventRepository.AddRecord(RecordTypes.Session, GetNew(RecordTypes.Session));
            List<Session> result = _eventRepository.GetAllSessions();
            var sessionCount = result.Count;
            Assert.IsTrue(sessionCount >= 2);
            // delete one and refresh.
            _eventRepository.DeleteRecord(result[0].Id.ToString(), RecordTypes.Session);
            result = _eventRepository.GetAllSessions();
            Assert.IsTrue(result.Count == (sessionCount - 1));
        }

        [TestMethod]
        public void UpdateSession()
        {
            const DayOfWeek testDay = DayOfWeek.Monday;
            const string testTitle = nameof(testTitle);
            const string testDesc = nameof(testDesc);
            // Add 2 sessions
            _eventRepository.AddRecord(RecordTypes.Session, GetNew(RecordTypes.Session));
            _eventRepository.AddRecord(RecordTypes.Session, GetNew(RecordTypes.Session));
            List<Session> result = _eventRepository.GetAllSessions();
            Assert.IsTrue(result.Count >= 2);
            // edit; refresh.
            Session session = result[result.Count - 1];
            int initialSessionId = session.Id;
            session.Day = testDay;
            session.Description = testDesc;
            session.Title = testTitle;
            _eventRepository.UpdateRecord(session, RecordTypes.Session);
            result = _eventRepository.GetAllSessions();
            Assert.IsTrue(result.Count >= 2);
            session = result.FirstOrDefault(s => s.Title == testTitle);
            Assert.IsNotNull(session);
            Assert.AreEqual(initialSessionId, session.Id);
            Assert.AreEqual(testDay, session.Day);
            Assert.AreEqual(testDesc, session.Description);
        }

        [TestMethod]
        public void GetSessionsBeforeInitialized()
        {
            _eventRepository.DeleteFile(RecordTypes.Session);
            List<Session> sessions = _eventRepository.GetAllSessions(); // don't blow up.
            Assert.IsFalse(sessions.Any());
        }

        [TestMethod]
        public void GetRegistrantByEmail()
        {
            var registrantId = _eventRepository.AddRecord(RecordTypes.Registrant, GetNew(RecordTypes.Registrant));
            var registrant = _eventRepository.GetRegistrant(registrantId);
            var result = _eventRepository.GetRegistrant(registrant.PersonalInfo.Email);
            Assert.AreEqual(registrant, result);
        }
        
        [TestMethod]
        public void GetRegistrationByRegistrantId()
        {
            var registrantId1 = _eventRepository.AddRecord(RecordTypes.Registrant, GetNew(RecordTypes.Registrant));
            var registrantId2 = _eventRepository.AddRecord(RecordTypes.Registrant, GetNew(RecordTypes.Registrant));
            var registration1 = new Registration { RegistrantId = registrantId1 };
            var registration2 = new Registration { RegistrantId = registrantId2, SessionIds = new List<int> { 42 } };
            _eventRepository.AddRecord(RecordTypes.Registration, registration1);
            _eventRepository.AddRecord(RecordTypes.Registration, registration2);
            var result = _eventRepository.GetRegistrationBy(registrantId2);
            Assert.IsTrue(result.SessionIds.Any());
            Assert.AreEqual(42, result.SessionIds.First());
        }

        [TestMethod]
        public void RegistrationShouldBeValidWithoutSessionList()
        {
            Registration r = GetNewRegistration();
            r.SessionIds.Clear();
            var registrationId = _eventRepository.AddRecord(RecordTypes.Registration, r);
            r = _eventRepository.GetRegistration(registrationId);
            Assert.AreEqual(r.SessionIds.Count, 0);
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

        private IEventRecord GetEventRecord(RecordTypes rt, int id = FIRST_RECORD_ID) => rt switch
        {
            RecordTypes.Registration => _eventRepository.GetRegistration(id),
            RecordTypes.Registrant => _eventRepository.GetRegistrant(id),
            RecordTypes.Session => _eventRepository.GetSession(id),
            _ => null
        };

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

        private static Registration GetNewRegistration() => new() { Id = FIRST_RECORD_ID, RegistrantId = FIRST_RECORD_ID, SessionIds = new List<int> { FIRST_RECORD_ID } };

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
            Assert.IsTrue(r.RegistrantId > 0);
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
