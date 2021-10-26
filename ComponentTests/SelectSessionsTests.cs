using System;
using Bunit;
using EventRegistration.Partials;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;

namespace ComponentTests
{
    
    [TestClass]
    public class SelectSessionsTests: TestBase
    {

        [TestInitialize]
        public void ClassInit() => Init();

        [TestCleanup]
        public void TestCleanup() => Cleanup();

        [TestMethod]
        public void ComponentRendersProperly()
        {
            var cut = BlazorTestContext.RenderComponent<SelectSessions>(parameters => parameters.Add(p => p.RegistrationId, "1"));
            cut.MarkupMatches(TestResources.SelectSessionsComponentRendered);
        }

        [TestMethod]
        [SuppressMessage("TestEventHandler", "BL0005:Component parameter should not be set outside of its component")]
        public void ClickHandlerDispatchesSessionId()
        {
            var cut = BlazorTestContext.RenderComponent<SelectSessions>(parameters => parameters.Add(p => p.RegistrationId, "1"));
            TestEventHandler eventReceiver = new TestEventHandler();
            cut.Instance.OnAdd = new EventCallback<int>(eventReceiver, null);
            var firstButton = cut.FindAll("button")[0];
            firstButton.Click();
            TestBase.WaitFor(() => eventReceiver.ValueReceived != 0);
            Assert.IsTrue(eventReceiver.ValueReceived > 0);
        }

    }

}
