using System;

namespace EventRepository
{

    /// <summary>
    /// Just a map for C# consumers to the F# enumeration (which isn't an enumeration because it can't be because... I forget).
    /// </summary>
    public enum RecordTypes
    {
        Registration, Registrant, Session
    }

    public static class RecordTypeConverter
    {
        public static string GetFileName(RecordTypes rt) => rt switch
        {
            RecordTypes.Registration => EventData.RecordTypes.registrationFileName,
            RecordTypes.Registrant => EventData.RecordTypes.registrantFileName,
            RecordTypes.Session => EventData.RecordTypes.sessionFileName,
            _ => string.Empty
        };
    }

}
