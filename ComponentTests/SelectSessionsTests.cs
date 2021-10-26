using System;
using Bunit;
using EventRegistration.Partials;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using MS_TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;

namespace ComponentTests
{
    
    [TestClass]
    public class SelectSessionsTests
    {

        [ClassInitialize]
        public static void ClassInit(MS_TestContext _) => TestDriver.Init();

        [ClassCleanup]
        public static void Cleanup() => TestDriver.Cleanup();

        [TestMethod]
        public void ComponentRendersProperly()
        {
            var cut = TestDriver.BlazorTestContext.RenderComponent<SelectSessions>(parameters => parameters.Add(p => p.RegistrationId, "1"));
            cut.MarkupMatches(TestResources.SelectSessionsComponentRendered);
        }

        [TestMethod]
        [SuppressMessage("TestEventHandler", "BL0005:Component parameter should not be set outside of its component")]
        public void ClickHandlerDispatchesSessionId()
        {
            var cut = TestDriver.BlazorTestContext.RenderComponent<SelectSessions>(parameters => parameters.Add(p => p.RegistrationId, "1"));
            TestEventHandler eventReceiver = new TestEventHandler();
            cut.Instance.OnAdd = new EventCallback<int>(eventReceiver, null);
            var firstButton = cut.FindAll("button")[0];
            firstButton.Click();
            TestDriver.WaitFor(() => eventReceiver.ValueReceived != 0);
            Assert.IsTrue(eventReceiver.ValueReceived > 0);
        }

    }

}
