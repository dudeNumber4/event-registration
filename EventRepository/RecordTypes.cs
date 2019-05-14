using System;
using System.Collections.Generic;
using System.Text;
using EventData;

namespace EventRepository
{

    /// <summary>
    /// Just a map for C# consumers to the F# enumeration (which isn't an enumeration because it can't be because... I forget).
    /// </summary>
    public enum RecordTypes
    {
        Itinerary, Registrant, Registration, Session
    }

    internal static class RecordTypeConverter
    {
        internal static string GetFileName(RecordTypes rt)
        {
            switch (rt)
            {
                case RecordTypes.Itinerary:
                    return EventData.RecordTypes.itineraryFileName;
                case RecordTypes.Registrant:
                    return EventData.RecordTypes.registrantFileName;
                case RecordTypes.Registration:
                    return EventData.RecordTypes.registrationFileName;
                case RecordTypes.Session:
                    return EventData.RecordTypes.sessionFileName;
                default:
                    return string.Empty;
            }
        }
    }

}
