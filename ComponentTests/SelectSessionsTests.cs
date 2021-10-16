using System;
using EventRegistration.Partials;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bunit;
using BU_TestContext = Bunit.TestContext;
using MS_TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using EventRegistration.Services;
using Microsoft.Extensions.DependencyInjection;
using EventRepo = EventRepository.EventRepository;
using EventRepository;
using System.IO;

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
            _blazorTestContext.Services.AddSingleton(preparer);
            var eventRepository = new EventRepo(preparer);
            InitializeDataFiles(eventRepository.DataPath());
            _blazorTestContext.Services.AddSingleton<IEventRepository>(eventRepository);
            _blazorTestContext.Services.AddSingleton<RegistrationService>();
            _blazorTestContext.Services.AddSingleton<SessionService>();
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
        public void ComponentRenders()
        {
            var cut = _blazorTestContext.RenderComponent<SelectSessions>(parameters => parameters.Add(p => p.RegistrationId, "1"));
            cut.MarkupMatches(TestResources.SelectSessionsComponentRendered);
        }
        
        private static void InitializeDataFiles(string dataPath)
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
