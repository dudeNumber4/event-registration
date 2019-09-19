using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EventRegistration.Data;
using EventModels;
using Microsoft.AspNetCore.Components;

namespace EventRegistration.Partials
{
    public class SimpleSessionFormModel : PageModel
    {

        // DI?
        SessionService SessionService = new SessionService();

        public Session Session { get; set; } // must get from service

        [Parameter] // this attribute means url parameter
        public string SessionId { get; set; }

        public void OnGet()
        {
            Session = SessionService.GetSession(SessionId).Result; // dunno how to async here
        }

        private void HandleValidSubmit()
        {
            Console.WriteLine("OnValidSubmit");
            // route back to sessions.
        }

    }
}
