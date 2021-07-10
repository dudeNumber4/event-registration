namespace EventData

module RecordTypes =

  // id, Day, Title, Description
  type SessionRecord = (string * string * string * string)
  let sessionFileName = "Session.csv"

  // id, firstName, LastName, Email, OrgName, Industry
  type RegistrantRecord = (string * string * string * string * string * string)
  let registrantFileName = "Registrant.csv"

  // id, registrationId, unbounded SessionIds
  type RegistrationRecord = (string * string * string List)
  let registrationFileName = "Registration.csv"

// Registrant (personal info)
// Registration -> Registrant + (sessions)
  type EventRegistrationRecord =
    | RegistrantRecord of RegistrantRecord
    | SessionRecord of SessionRecord
    | RegistrationRecord of RegistrationRecord
