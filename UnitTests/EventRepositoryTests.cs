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

        private static object _lock = new object();
        internal const string FIRST_RECORD_ID = "1";
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
            _eventRepository.DeleteFile(RecordTypes.Itinerary);
            _eventRepository.DeleteFile(RecordTypes.Registrant);
            _eventRepository.DeleteFile(RecordTypes.Registration);
            _eventRepository.DeleteFile(RecordTypes.Session);
        }

        [TestMethod]
        public void GetAllSessions()
        {
            lock (_lock)
            {
                Task.WaitAll(new Task[] { _eventRepository.AddRecord(RecordTypes.Session, GetEnumerableDataFor(RecordTypes.Session)) });
                Task.WaitAll(new Task[] { _eventRepository.AddRecord(RecordTypes.Session, GetEnumerableDataFor(RecordTypes.Session)) });
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
                // All the helper functions are for itinerary; the thing that needs all the others.
                void AddRecord(RecordTypes recordType)
                {
                    Task.WaitAll(new Task[] { _eventRepository.AddRecord(recordType, GetEnumerableDataFor(recordType)) });
                }

                // add record of the given type
                AddRecord(rt);

                // retrieve by id (should have fresh ids)
                var eventRecord = GetEventRecord(rt);
                Assert.IsNotNull(eventRecord);

                void AddNewSession()
                {
                    AddRecord(RecordTypes.Session);
                    _eventRepository.AddSession(GetEventRecord(RecordTypes.Session) as Session).Wait();
                }
                void AddNewRegistrant() => _eventRepository.AddRecord(RecordTypes.Registrant, GetEnumerableDataFor(RecordTypes.Registrant)).Wait();
                void AddNewRegistration()
                {
                    AddNewRegistrant();
                    var registrantRecord = GetEventRecord(RecordTypes.Registrant);
                    var registration = new Registration { RegistrantId = registrantRecord.Id };
                    _eventRepository.AddRegistration(registration).Wait();
                }

                // Save it as a new typed object.
                switch (rt)
                {
                    case RecordTypes.Itinerary:
                        // must first add related registrant, registration, session, all the things.
                        AddNewSession();
                        AddNewRegistration();
                        var registrationRecord = GetEventRecord(RecordTypes.Registrant);
                        var i = eventRecord as Itinerary;
                        i.RegistrationId = registrationRecord.Id;
                        var sessionRecord = GetEventRecord(RecordTypes.Session);
                        Assert.IsNotNull(i.SessionList);
                        i.SessionList.Add(sessionRecord as Session);
                        Assert.IsTrue(_eventRepository.AddItinerary(i).Result);
                        break;
                    case RecordTypes.Registrant:
                        _eventRepository.AddRegistrant(eventRecord as Registrant).Wait();
                        break;
                    case RecordTypes.Registration:
                        _eventRepository.AddRegistration(eventRecord as Registration).Wait();
                        break;
                    case RecordTypes.Session:
                        AddNewSession();
                        break;
                    default:
                        throw new NotImplementedException(rt.ToString());
                }

                // retrieve it; ensure it was saved correctly
                eventRecord = GetEventRecord(rt, eventRecord.Id.ToString());
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
                Task.WaitAll(new Task[] { _eventRepository.AddRecord(rt, GetEnumerableDataFor(rt)) });
                Task.WaitAll(new Task[] { _eventRepository.AddRecord(rt, GetEnumerableDataFor(rt)) });

                // retrieve by id (should have fresh ids)
                var eventRecord = GetEventRecord(rt);
                Assert.IsNotNull(eventRecord);
                Assert.AreEqual(int.Parse(FIRST_RECORD_ID), eventRecord.Id);
                ValidateEventRecord(eventRecord, rt);

                // delete
                Task.WaitAll(new Task[] { _eventRepository.DeleteRecord(FIRST_RECORD_ID, rt) });

                // retrieve nothing
                eventRecord = GetEventRecord(rt);
                Assert.IsNull(eventRecord);
            }
        }

        [TestMethod]
        public void DeleteRecord()
        {
            lock (_lock)
            {
                _eventRepository.DeleteFile(RecordTypes.Session).Wait();
                Task.WaitAll(new Task[] { _eventRepository.AddRecord(RecordTypes.Session, GetEnumerableDataFor(RecordTypes.Session)) });
                Task.WaitAll(new Task[] { _eventRepository.AddRecord(RecordTypes.Session, GetEnumerableDataFor(RecordTypes.Session)) });
                List<Session> result = _eventRepository.GetAllSessions().Result;
                Assert.AreEqual(2, result.Count);
                // delete one and refresh.
                _eventRepository.DeleteRecord(result[0].Id.ToString(), RecordTypes.Session).Wait();
                result = _eventRepository.GetAllSessions().Result;
                Assert.AreEqual(1, result.Count);
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

        //[TestMethod]
        //public void GetNextId()
        //{
        //    lock (_lock)
        //    {
        //        Task.WaitAll(new Task[] { _eventRepository.AddRecord(RecordTypes.Session, GetEnumerableDataFor(RecordTypes.Session)) });
        //        List<Session> result = _eventRepository.GetAllSessions().Result;
        //        var nextId = _eventRepository.NextId(RecordTypes.Session);
        //        Assert.IsTrue(nextId == result.Count() + 1);
        //    }
        //}

        void ValidateEventRecord(IEventRecord eventRecord, RecordTypes rt)
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

        private IEventRecord GetEventRecord(RecordTypes rt, string id = FIRST_RECORD_ID)
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

        internal static IEnumerable<string> GetEnumerableDataFor(RecordTypes rt)
        {
            switch (rt)
            {
                case RecordTypes.Itinerary: return GetItineraryEnumerable();
                case RecordTypes.Registrant: return GetRegistrantEnumerable();
                case RecordTypes.Registration: return GetRegistrationEnumerable();
                case RecordTypes.Session: return GetSessionEnumerable();
                default: return Enumerable.Empty<string>();
            }
        }

        private static IEnumerable<string> GetRegistrantEnumerable()
        {
            yield return "firstName";
            yield return "lastName";
            yield return "email";
            yield return "orgName";
            yield return "industry";
        }

        private static IEnumerable<string> GetRegistrationEnumerable()
        {
            yield return FIRST_RECORD_ID; // registrant id
        }

        private static IEnumerable<string> GetSessionEnumerable()
        {
            yield return ((int)DayOfWeek.Friday).ToString();
            yield return "title";
            yield return "description";
        }

        private static IEnumerable<string> GetItineraryEnumerable()
        {
            yield return FIRST_RECORD_ID; // id
            yield return FIRST_RECORD_ID; // registrationId
            yield return FIRST_RECORD_ID; // SessionIds
            yield return "2";
        }

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