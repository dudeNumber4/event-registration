﻿using EventModels;
using EventRepository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
        public static void ClassInit(TestContext tc)
        {
            _eventRepository = new EventRepository.EventRepository();
            if (_eventRepository.DataFileExists())
            {
                throw new Exception("A data file already exists in root of solution");
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _eventRepository.DeleteFile(RecordTypes.Itinerary).Wait();
            _eventRepository.DeleteFile(RecordTypes.Registrant).Wait();
            _eventRepository.DeleteFile(RecordTypes.Registration).Wait();
            _eventRepository.DeleteFile(RecordTypes.Session).Wait();
        }

        [TestMethod]
        public void GetAllSessions()
        {
            lock (_lock)
            {
                _eventRepository.DeleteFile(RecordTypes.Session).Wait();
                Task.WaitAll(new Task[] { _eventRepository.AddRecord(RecordTypes.Session, GetNew(RecordTypes.Session)) });
                Task.WaitAll(new Task[] { _eventRepository.AddRecord(RecordTypes.Session, GetNew(RecordTypes.Session)) });
                List<Session> result = _eventRepository.GetAllSessions().Result;
                Assert.AreEqual(2, result.Count);
                ValidateEventRecord(result[0], RecordTypes.Session);
                ValidateEventRecord(result[1], RecordTypes.Session);
                Assert.AreNotEqual(result[0].Id, result[1].Id);
            }
        }

        [TestMethod]
        [DataRow(RecordTypes.Registrant)]
        [DataRow(RecordTypes.Registration)]
        [DataRow(RecordTypes.Session)]
        [DataRow(RecordTypes.Itinerary)]
        public void AddConcreteTypes(RecordTypes rt)
        {
            lock (_lock)
            {
                IEventRecord eventObject = rt switch
                {
                    RecordTypes.Itinerary => GetNewItinerary(),
                    RecordTypes.Registrant => GetNewRegistrant(),
                    RecordTypes.Registration => GetNewRegistration(),
                    RecordTypes.Session => GetNewSession(),
                    _ => null
                };

                int newId = rt switch
                {
                    RecordTypes.Itinerary => _eventRepository.AddRecord(RecordTypes.Itinerary, eventObject).Result,
                    RecordTypes.Registrant => _eventRepository.AddRecord(RecordTypes.Registrant, eventObject).Result,
                    RecordTypes.Registration => _eventRepository.AddRecord(RecordTypes.Registration, eventObject).Result,
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
        [DataRow(RecordTypes.Registration)]
        [DataRow(RecordTypes.Session)]
        [DataRow(RecordTypes.Itinerary)]
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
                _eventRepository.DeleteFile(RecordTypes.Session).Wait();
                Task.WaitAll(new Task[] { _eventRepository.AddRecord(RecordTypes.Session, GetNew(RecordTypes.Session)) });
                Task.WaitAll(new Task[] { _eventRepository.AddRecord(RecordTypes.Session, GetNew(RecordTypes.Session)) });
                List<Session> result = _eventRepository.GetAllSessions().Result;
                Assert.AreEqual(2, result.Count);
                // delete one and refresh.
                _eventRepository.DeleteRecord(result[0].Id.ToString(), RecordTypes.Session).Wait();
                result = _eventRepository.GetAllSessions().Result;
                Assert.AreEqual(1, result.Count);
            }
        }

        [TestMethod]
        public void UpdateSession()
        {
            const DayOfWeek testDay = DayOfWeek.Monday;
            const string testTitle = "titl";
            const string testDesc = "descr";
            lock (_lock)
            {
                _eventRepository.DeleteFile(RecordTypes.Session).Wait();
                // Add 2 sessions
                Task.WaitAll(new Task[] { _eventRepository.AddRecord(RecordTypes.Session, GetNew(RecordTypes.Session)) });
                Task.WaitAll(new Task[] { _eventRepository.AddRecord(RecordTypes.Session, GetNew(RecordTypes.Session)) });
                List<Session> result = _eventRepository.GetAllSessions().Result;
                Assert.AreEqual(2, result.Count);
                // edit; refresh.
                Session session = result[0];
                int initialSessionId = session.Id;
                session.Day = testDay;
                session.Description = testDesc;
                session.Title = testTitle;
                _eventRepository.UpdateRecord(result[0], RecordTypes.Session).Wait();
                result = _eventRepository.GetAllSessions().Result;
                Assert.AreEqual(2, result.Count);
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

        private void ValidateEventRecord(IEventRecord eventRecord, RecordTypes rt)
        {
            switch (rt)
            {
                case RecordTypes.Itinerary:
                    ValidateItinerary(eventRecord as Itinerary);
                    break;
                case RecordTypes.Registrant:
                    ValidateRegistrant(eventRecord as Registrant);
                    break;
                case RecordTypes.Registration:
                    ValidateRegistration(eventRecord as Registration);
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
                case RecordTypes.Itinerary:
                    result = _eventRepository.GetItinerary(id).Result;
                    break;
                case RecordTypes.Registrant:
                    result = _eventRepository.GetRegistrant(id).Result;
                    break;
                case RecordTypes.Registration:
                    result = _eventRepository.GetRegistration(id).Result;
                    break;
                case RecordTypes.Session:
                    result = _eventRepository.GetSession(id).Result;
                    break;
                default:
                    return null;
            }
            return result;
        }

        internal static IEventRecord GetNew(RecordTypes rt)
        {
            return rt switch
            {
                RecordTypes.Itinerary => GetNewItinerary(),
                RecordTypes.Registrant => GetNewRegistrant(),
                RecordTypes.Registration => GetNewRegistration(),
                RecordTypes.Session => GetNewSession(),
                _ => null
            };
        }

        private static Registrant GetNewRegistrant() => new Registrant 
        { 
            EmploymentInfo = new Employment { Industry = nameof(Employment.Industry), OrgName = nameof(Employment.OrgName) },
            PersonalInfo = new Personal { Email = nameof(Personal.Email), FirstName = nameof(Personal.FirstName), LastName = nameof(Personal.LastName) }
        };

        private static Registration GetNewRegistration() => new Registration { RegistrantId = FIRST_RECORD_ID };

        private static Session GetNewSession() => new Session { Day = DayOfWeek.Friday, Title = "title", Description = "description" };

        private static Itinerary GetNewItinerary() => new Itinerary { RegistrationId = FIRST_RECORD_ID, SessionIds = new List<int> { FIRST_RECORD_ID } };

        void ValidateSession(Session session)
        {
            Assert.IsNotNull(session);
            Assert.IsTrue(session.Id > 0);
            Assert.IsFalse(string.IsNullOrEmpty(session.Title));
            Assert.IsFalse(string.IsNullOrEmpty(session.Description));
        }

        void ValidateRegistration(Registration registration)
        {
            Assert.IsNotNull(registration);
            Assert.IsTrue(registration.Id > 0);
            Assert.IsTrue(registration.RegistrantId > 0);
        }

        void ValidateItinerary(Itinerary itinerary)
        {
            Assert.IsNotNull(itinerary);
            Assert.IsTrue(itinerary.Id > 0);
            Assert.IsTrue(itinerary.RegistrationId > 0);
            Assert.IsNotNull(itinerary.SessionIds);
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
