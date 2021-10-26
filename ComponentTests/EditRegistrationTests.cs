using AngleSharp.Dom;
using Bunit;
using EventRegistration.Pages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace ComponentTests
{

    [TestClass]
    public class EditRegistrationTests: TestBase
    {
        
        [TestInitialize]
        public void ClassInit() => Init();

        [TestCleanup]
        public void TestCleanup() => Cleanup();

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

            var cut = BlazorTestContext.RenderComponent<EditRegistration>(parameters => parameters.Add(p => p.RegistrationId, "1"));
            Assert.AreEqual(1, GetSelectedItems(cut).Count());
            var selectedSessionList = cut.Find("ul");
            Assert.AreEqual(1, selectedSessionList.Children.Length);
            var firstSessionDiv = cut.FindAll("div.session-title-selectable")[0];
            Assert.IsNotNull(firstSessionDiv);
            var firstSessionName = firstSessionDiv.InnerHtml;
            var firstButton = cut.FindAll("button")[0];  // button near first avail
            firstButton.Click();  // add first item from avail to selected
            TestBase.WaitFor(() => GetSelectedItems(cut).Count() > 0);
            Assert.IsTrue(SelectedItemsContains(cut, firstSessionName));
        }

    }
}
