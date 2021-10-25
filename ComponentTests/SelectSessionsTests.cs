using Bunit;
using EventRegistration.Partials;
using EventRegistration.Services;
using EventRepository;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;
using BU_TestContext = Bunit.TestContext;
using EventRepo = EventRepository.EventRepository;
using MS_TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;

namespace ComponentTests
{
    
    [TestClass]
    public class SelectSessionsTests
    {

        private static BU_TestContext _blazorTestContext = new BU_TestContext();
        private static IDataPreparer preparer = new TestDataPreparer();
        private static string _registrationPath;
        private static string _registrantPath;
        private static string _sessionPath;

        [ClassInitialize]
        public static void ClassInit(MS_TestContext _)
        {
            // dependencies
            _blazorTestContext.Services.AddSingleton(preparer);
            var eventRepository = new EventRepo(preparer);
            _blazorTestContext.Services.AddSingleton<IEventRepository>(eventRepository);
            _blazorTestContext.Services.AddSingleton<RegistrationService>();
            _blazorTestContext.Services.AddSingleton<SessionService>();
            InitializeDataRepository(eventRepository.DataPath());
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            _blazorTestContext.Dispose();
            if (File.Exists(_registrationPath)) File.Delete(_registrationPath);
            if (File.Exists(_registrantPath)) File.Delete(_registrantPath);
            if (File.Exists(_sessionPath)) File.Delete(_sessionPath);
        }

        [TestMethod]
        public void ComponentRendersProperly()
        {
            var cut = _blazorTestContext.RenderComponent<SelectSessions>(parameters => parameters.Add(p => p.RegistrationId, "1"));
            cut.MarkupMatches(TestResources.SelectSessionsComponentRendered);
        }

        [TestMethod]
        public void ClickHandlerDispatchesSessionId()
        {
            var cut = _blazorTestContext.RenderComponent<SelectSessions>(parameters => parameters.Add(p => p.RegistrationId, "1"));
            TestEventHandler eventReceiver = new TestEventHandler();
            cut.Instance.OnAdd = new EventCallback<int>(eventReceiver, null);
            var firstButton = cut.FindAll("button")[0];
            firstButton.Click();
            var ticks = Environment.TickCount;
            while (((Environment.TickCount - ticks) < 500) && (eventReceiver.ValueReceived == 0))
            {
                Thread.Sleep(3);
            }
            Assert.IsTrue(eventReceiver.ValueReceived > 0);
        }

        private static void InitializeDataRepository(string dataPath)
        {
            _sessionPath = Path.Combine(dataPath, RecordTypeConverter.GetFileName(RecordTypes.Session));
            _registrantPath = Path.Combine(dataPath, RecordTypeConverter.GetFileName(RecordTypes.Registrant));
            _registrationPath = Path.Combine(dataPath, RecordTypeConverter.GetFileName(RecordTypes.Registration));
            File.WriteAllText(_sessionPath, TestResources.Session);
            File.WriteAllText(_registrantPath, TestResources.Registrant);
            File.WriteAllText(_registrationPath, TestResources.Registration);
        }

    }

}
