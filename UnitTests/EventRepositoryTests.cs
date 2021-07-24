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
        public static void ClassInit(TestContext tc) => _eventRepository = new EventRepo(new TempDataPreparer());

        [TestMethod]
        public async Task GetAllSessions()
        {
            Task.WaitAll(new Task[] { _eventRepository.AddRecord(RecordTypes.Session, GetNew(RecordTypes.Session)) });
            Task.WaitAll(new Task[] { _eventRepository.AddRecord(RecordTypes.Session, GetNew(RecordTypes.Session)) });
            List<Session> result = await _eventRepository.GetAllSessions();
            Assert.IsTrue(result.Count >= 2);
            ValidateEventRecord(result[result.Count - 2], RecordTypes.Session);
            ValidateEventRecord(result[result.Count - 1], RecordTypes.Session);
            Assert.AreNotEqual(result[result.Count - 2].Id, result[result.Count - 1].Id);
        }

        [TestMethod]
        [DataRow(RecordTypes.Registrant)]
        [DataRow(RecordTypes.Session)]
        [DataRow(RecordTypes.Registration)]
        public async Task AddConcreteTypes(RecordTypes rt)
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
                RecordTypes.Registration => await _eventRepository.AddRecord(RecordTypes.Registration, eventObject),
                RecordTypes.Registrant => await _eventRepository.AddRecord(RecordTypes.Registrant, eventObject),
                RecordTypes.Session => await _eventRepository.AddRecord(RecordTypes.Session, eventObject),
                _ => 0
            };

            // retrieve it; ensure it was saved correctly
            var eventRecord = await GetEventRecord(rt, newId);
            ValidateEventRecord(eventRecord, rt);
        }

        [TestMethod]
        [DataRow(RecordTypes.Registrant)]
        [DataRow(RecordTypes.Session)]
        [DataRow(RecordTypes.Registration)]
        public async Task ProcessRecord(RecordTypes rt)
        {
            // add 2 records of the given type
            await _eventRepository.AddRecord(rt, GetNew(rt));
            await _eventRepository.AddRecord(rt, GetNew(rt));

            // retrieve by id (should have fresh ids)
            var eventRecord = await GetEventRecord(rt);
            Assert.IsNotNull(eventRecord);
            Assert.AreEqual(FIRST_RECORD_ID, eventRecord.Id);
            ValidateEventRecord(eventRecord, rt);

            // delete
            await _eventRepository.DeleteRecord(FIRST_RECORD_ID.ToString(), rt);

            // retrieve nothing
            eventRecord = await GetEventRecord(rt);
            Assert.IsNull(eventRecord);
        }

        [TestMethod]
        public async Task DeleteSession()
        {
            await _eventRepository.AddRecord(RecordTypes.Session, GetNew(RecordTypes.Session));
            await _eventRepository.AddRecord(RecordTypes.Session, GetNew(RecordTypes.Session));
            List<Session> result = await _eventRepository.GetAllSessions();
            var sessionCount = result.Count;
            Assert.IsTrue(sessionCount >= 2);
            // delete one and refresh.
            await _eventRepository.DeleteRecord(result[0].Id.ToString(), RecordTypes.Session);
            result = await _eventRepository.GetAllSessions();
            Assert.IsTrue(result.Count == (sessionCount - 1));
        }

        [TestMethod]
        public async Task UpdateSession()
        {
            const DayOfWeek testDay = DayOfWeek.Monday;
            const string testTitle = nameof(testTitle);
            const string testDesc = nameof(testDesc);
            // Add 2 sessions
            await _eventRepository.AddRecord(RecordTypes.Session, GetNew(RecordTypes.Session));
            await _eventRepository.AddRecord(RecordTypes.Session, GetNew(RecordTypes.Session));
            List<Session> result = await _eventRepository.GetAllSessions();
            Assert.IsTrue(result.Count >= 2);
            // edit; refresh.
            Session session = result[result.Count - 1];
            int initialSessionId = session.Id;
            session.Day = testDay;
            session.Description = testDesc;
            session.Title = testTitle;
            await _eventRepository.UpdateRecord(session, RecordTypes.Session);
            result = await _eventRepository.GetAllSessions();
            Assert.IsTrue(result.Count >= 2);
            session = result.FirstOrDefault(s => s.Title == testTitle);
            Assert.IsNotNull(session);
            Assert.AreEqual(initialSessionId, session.Id);
            Assert.AreEqual(testDay, session.Day);
            Assert.AreEqual(testDesc, session.Description);
        }

        [TestMethod]
        public async Task GetSessionsBeforeInitialized()
        {
            await _eventRepository.DeleteFile(RecordTypes.Session);
            List<Session> sessions = await _eventRepository.GetAllSessions(); // don't blow up.
            Assert.IsFalse(sessions.Any());
        }

        [TestMethod]
        public async Task GetRegistrantByEmail()
        {
            var registrantId = await _eventRepository.AddRecord(RecordTypes.Registrant, GetNew(RecordTypes.Registrant));
            var registrant = await _eventRepository.GetRegistrant(registrantId);
            var result = await _eventRepository.GetRegistrant(registrant.PersonalInfo.Email);
            Assert.AreEqual(registrant, result);
        }
        
        [TestMethod]
        public async Task GetRegistrationByRegistrantId()
        {
            var registrantId1 = await _eventRepository.AddRecord(RecordTypes.Registrant, GetNew(RecordTypes.Registrant));
            var registrantId2 = await _eventRepository.AddRecord(RecordTypes.Registrant, GetNew(RecordTypes.Registrant));
            var registration1 = new Registration { RegistrantId = registrantId1 };
            var registration2 = new Registration { RegistrantId = registrantId2, SessionIds = new List<int> { 42 } };
            await _eventRepository.AddRecord(RecordTypes.Registration, registration1);
            await _eventRepository.AddRecord(RecordTypes.Registration, registration2);
            var result = await _eventRepository.GetRegistrationBy(registrantId2);
            Assert.IsTrue(result.SessionIds.Any());
            Assert.AreEqual(42, result.SessionIds.First());
        }

        [TestMethod]
        public async Task RegistrationShouldBeValidWithoutSessionList()
        {
            Registration r = GetNewRegistration();
            r.SessionIds.Clear();
            var registrationId = await _eventRepository.AddRecord(RecordTypes.Registration, r);
            r = _eventRepository.GetRegistration(registrationId).Result;
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

        private async Task<IEventRecord> GetEventRecord(RecordTypes rt, int id = FIRST_RECORD_ID) => rt switch
        {
            RecordTypes.Registration => await _eventRepository.GetRegistration(id),
            RecordTypes.Registrant => await _eventRepository.GetRegistrant(id),
            RecordTypes.Session => await _eventRepository.GetSession(id),
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
