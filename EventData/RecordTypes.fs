namespace EventData

module RecordTypes =

  // id, Day, Title, Description
  type SessionRecord = (string * string * string * string)
  let sessionFileName = "Session.csv"

  // id, firstName, LastName, Email, OrgName, Industry
  type RegistrantRecord = (string * string * string * string * string * string)
  let registrantFileName = "Registrant.csv"

  // id, RegistrantId
  type RegistrationRecord = (string * string)
  let registrationFileName = "Registration.csv"

  // id, registrationId, unbounded SessionIds
  type ItineraryRecord = (string * string * string List)
  let itineraryFileName = "Itinerary.csv"

// Itinerary -> Registration + (sessions)
// Registration -> Registrant + (I think I originally meant to add some kind of flag that the registrant has read the waiver or something)
// Registrant (personal info)