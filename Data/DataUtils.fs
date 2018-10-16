namespace EventData

module DataUtils =
  open System.Reflection
  open System.IO
  open System
 
  type EventRegistrationRecord =
    | RegistrantRecord of RecordTypes.RegistrantRecord
    | RegistrationRecord of RecordTypes.RegistrationRecord
    | SessionRecord of RecordTypes.SessionRecord
    | ItineraryRecord of RecordTypes.ItineraryRecord

  // A common solution root location
  let private dataDirectory =
    let location = Assembly.GetExecutingAssembly().Location |> Path.GetDirectoryName
    location + "..\..\..\..\.."

  let private ensureDataFileExists filePath =
    if not (File.Exists(filePath)) then
      use f = File.CreateText(filePath)
      ()  // use can't be the last statement :(

  let private addRecord record =
    match record with
    | RegistrantRecord r -> let fileName = Path.Combine(dataDirectory, RecordTypes.registrantFileName)
                            ensureDataFileExists fileName |> ignore
                            let id, firstName, LastName, Email, OrgName, Industry = r
                            // sure will be nice when F# gets string interpolation
                            let csv = sprintf "\"%s\",\"%s\",\"%s\",\"%s\",\"%s\",\"%s\"%s" id firstName LastName Email OrgName Industry Environment.NewLine
                            File.AppendAllText(fileName, csv) |> ignore
    | RegistrationRecord r -> let fileName = Path.Combine(dataDirectory, RecordTypes.registrationFileName)
                              ensureDataFileExists fileName |> ignore
                              let id, RegistrantId = r
                              let csv = sprintf "\"%s\",\"%s\"%s" id RegistrantId Environment.NewLine
                              File.AppendAllText(fileName, csv) |> ignore
    | SessionRecord r -> let fileName = Path.Combine(dataDirectory, RecordTypes.sessionFileName)
                         ensureDataFileExists fileName |> ignore
                         let id, Day, Title, Description = r
                         let csv = sprintf "\"%s\",\"%s\",\"%s\",\"%s\"%s" id Day Title Description Environment.NewLine
                         File.AppendAllText(fileName, csv) |> ignore
    | ItineraryRecord r -> let fileName = Path.Combine(dataDirectory, RecordTypes.itineraryFileName)
                           ensureDataFileExists fileName |> ignore
                           let id, registrationId, SessionIds = r
                           let csv = sprintf "\"%s\",\"%s\"," id registrationId
                           // add id list to end of line
                           let newLine = (List.fold (fun accumulator current -> sprintf "%s,\"%s\"" accumulator current) csv SessionIds) + Environment.NewLine
                           File.AppendAllText(fileName, newLine) |> ignore

  // param fileName is one of RecordTypes' file names
  let public AddRecord fileName (list:string List) =
    match fileName with
    // You'd think you could match on a value like RecordTypes.RegistrantRecord, but you can't
    | "Registrant.csv" -> if (list.Length = 6) then
                            addRecord (EventRegistrationRecord.RegistrantRecord((list.[0], list.[1], list.[2], list.[3], list.[4], list.[5])))
                          else failwith (sprintf "%s: expected list length of 6" fileName)
    | "Registration.csv" -> if (list.Length = 2) then
                              addRecord (EventRegistrationRecord.RegistrationRecord((list.[0], list.[1])))
                            else failwith (sprintf "%s: expected list length of 2" fileName)
    | "Session.csv" -> if (list.Length = 4) then
                         addRecord (EventRegistrationRecord.SessionRecord((list.[0], list.[1], list.[2], list.[3])))
                       else failwith (sprintf "%s: expected list length of 4" fileName)
    | "Itinerary.csv" -> if (list.Length > 2) then
                           let sessionList = List.skip 2 list  // peel off the session list
                           addRecord (EventRegistrationRecord.ItineraryRecord((list.[0], list.[1], sessionList)))
                         else failwith (sprintf "%s: expected list length > 2" fileName)
    | _ -> failwith (sprintf "%s: unexpected value" fileName)

  // Given open stream, return empty string or string list of line with matching id
  let rec private getLineWith_ id (reader:StreamReader) =
    if (reader.EndOfStream) then []
    else
      let currentLine = reader.ReadLine()
      let currentWords = StringUtils.getQuotedStrings currentLine
      if (currentWords.Head = id) then currentWords
      else getLineWith_ id reader

  // Return empty string or string list of line with matching id
  let private getLineWith id (filePath:string) =
    use reader = new StreamReader(filePath)
    getLineWith_ id reader

  // Pass id and the type of record.  I may want to transform these into tuples later.
  // param fileName is one of RecordTypes' file names
  let public GetRecord id fileName =
    match fileName with
    // You'd think you could match on a value like RecordTypes.RegistrantRecord, but you can't
    | "Registrant.csv" -> getLineWith id (Path.Combine(dataDirectory, RecordTypes.registrantFileName))
    | "Registration.csv" -> getLineWith id (Path.Combine(dataDirectory, RecordTypes.registrationFileName))
    | "Session.csv" -> getLineWith id (Path.Combine(dataDirectory, RecordTypes.sessionFileName))
    | "Itinerary.csv" -> getLineWith id (Path.Combine(dataDirectory, RecordTypes.itineraryFileName))
    | _ -> failwith (sprintf "%s: unexpected value" fileName)

