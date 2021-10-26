using EventRegistration.Services;
using EventRepository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;
using BU_TestContext = Bunit.TestContext;
using EventRepo = EventRepository.EventRepository;

namespace ComponentTests
{
    
    public class TestBase
    {
        
        protected BU_TestContext BlazorTestContext = new BU_TestContext();
        private IDataPreparer preparer = new TestDataPreparer();
        private string _registrationPath;
        private string _registrantPath;
        private string _sessionPath;

        internal static void WaitFor(Func<bool> f)
        {
            var ticks = Environment.TickCount;
            while (((Environment.TickCount - ticks) < 500) && !f())
            {
                Thread.Sleep(3);
            }
        }

        [TestInitialize]
        internal void Init()
        {
            // dependencies
            BlazorTestContext.Services.AddSingleton(preparer);
            var eventRepository = new EventRepo(preparer);
            BlazorTestContext.Services.AddSingleton<IEventRepository>(eventRepository);
            BlazorTestContext.Services.AddSingleton<RegistrationService>();
            BlazorTestContext.Services.AddSingleton<RegistrantService>();
            BlazorTestContext.Services.AddSingleton<SessionService>();
            InitializeDataRepository(eventRepository.DataPath());
        }

        [TestCleanup]
        internal void Cleanup()
        {
            BlazorTestContext.Dispose();
            if (File.Exists(_registrationPath)) File.Delete(_registrationPath);
            if (File.Exists(_registrantPath)) File.Delete(_registrantPath);
            if (File.Exists(_sessionPath)) File.Delete(_sessionPath);
        }

        private void InitializeDataRepository(string dataPath)
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
