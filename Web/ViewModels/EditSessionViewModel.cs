using EventModels;
using EventRegistration.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventRegistration.ViewModels
{
    
    public class EditSessionViewModel
    {

        private Session session;
        private SessionService sessionService;

        public static EditSessionViewModel CreateAsync(SessionService sessionService, string sessionId)
        {
            EditSessionViewModel result = new();
            result.sessionService = sessionService;
            result.session = sessionService.GetSession(sessionId) ?? new Session { Title = "New Session" };
            return result;
        }

        public Session Session => session;

        public int Submit()
        {
            if (session.Id == 0)
            {
                return sessionService.AddSession(session);
            }
            else
            {
                sessionService.UpdateSession(session);
                return session.Id;
            }
        }

        private EditSessionViewModel() {}

    }

}
