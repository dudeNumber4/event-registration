namespace EventData

module RecordTypes =

  // id, firstName, LastName, Email, OrgName, Industry
  type RegistrantRecord = (string * string * string * string * string * string)
  let registrantFileName = "Registrant.csv"

  // id, registrationId, unbounded SessionIds
  type ItineraryRecord = (string * string * string List)
  let itineraryFileName = "Itinerary.csv"

  // id, RegistrantId
  type RegistrationRecord = (string * string)
  let registrationFileName = "Registration.csv"

  // id, Day, Title, Description
  type SessionRecord = (string * string * string * string)
  let sessionFileName = "Session.csv"

