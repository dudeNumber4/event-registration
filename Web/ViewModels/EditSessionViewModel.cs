using EventModels;
using EventRegistration.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventRegistration.ViewModels
{
    
    public class EditSessionViewModel
    {

        private Session session;
        private SessionService sessionService;

        public static async Task<EditSessionViewModel> CreateAsync(SessionService sessionService, string sessionId)
        {
            EditSessionViewModel result = new();
            result.sessionService = sessionService;
            result.session = await sessionService.GetSession(sessionId) ?? new Session { Title = "New Session" };
            return result;
        }

        public Session Session => session;

        public async Task<int> Submit()
        {
            if (session.Id == 0)
            {
                return await sessionService.AddSession(session);
            }
            else
            {
                await sessionService.UpdateSession(session);
                return session.Id;
            }
        }

        private EditSessionViewModel() {}

    }

}
