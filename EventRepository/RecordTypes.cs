using System;

namespace EventRepository
{

    /// <summary>
    /// Just a map for C# consumers to the F# enumeration (which isn't an enumeration because it can't be because... I forget).
    /// </summary>
    public enum RecordTypes
    {
        RegistrationTemp, Registrant, Registration, Session
    }

    internal static class RecordTypeConverter
    {
        internal static string GetFileName(RecordTypes rt) => rt switch
        {
            RecordTypes.RegistrationTemp => EventData.RecordTypes.registrationTempFileName,
            RecordTypes.Registrant => EventData.RecordTypes.registrantFileName,
            RecordTypes.Registration => EventData.RecordTypes.registrationFileName,
            RecordTypes.Session => EventData.RecordTypes.sessionFileName,
            _ => string.Empty
        };
    }

}
