using AngleSharp.Dom;
using Bunit;
using EventRegistration.Pages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MS_TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentTests
{

    [TestClass]
    public class EditRegistrationTests
    {
        [ClassInitialize]
        public static void ClassInit(MS_TestContext _) => TestDriver.Init();

        [ClassCleanup]
        public static void Cleanup() => TestDriver.Cleanup();

        [TestMethod]
        public void CanMoveSessionFromAvailToSelected()
        {
            IRefreshableElementCollection<IElement> GetSelectedItems(IRenderedComponent<EditRegistration> cut) => cut.FindAll("span.session-title-selected");
            bool SelectedItemsContains(IRenderedComponent<EditRegistration> cut, string innerHtml)
            {
                foreach (var item in GetSelectedItems(cut))
                    if (item.InnerHtml.Contains(innerHtml))
                        return true;
                return false;
            }

            var cut = TestDriver.BlazorTestContext.RenderComponent<EditRegistration>(parameters => parameters.Add(p => p.RegistrationId, "1"));
            Assert.AreEqual(1, GetSelectedItems(cut).Count());
            var selectedSessionList = cut.Find("ul");
            Assert.AreEqual(1, selectedSessionList.Children.Length);
            var firstSessionDiv = cut.FindAll("div.session-title-selectable")[0];
            Assert.IsNotNull(firstSessionDiv);
            var firstSessionName = firstSessionDiv.InnerHtml;
            var firstButton = cut.FindAll("button")[0];  // button near first avail
            firstButton.Click();  // add first item from avail to selected
            TestDriver.WaitFor(() => GetSelectedItems(cut).Count() > 0);
            Assert.IsTrue(SelectedItemsContains(cut, firstSessionName));
        }

    }
}
