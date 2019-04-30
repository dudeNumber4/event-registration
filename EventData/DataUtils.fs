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
      try
        use f = File.CreateText(filePath)
        ()  // use can't be the last statement :(
      with
        | :? System.FieldAccessException -> () // async, tests show create directly after check for existence fails.


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

  // Iterate through the reader writing to the writer unless line with id is found
  let rec private deleteLineWith_ id (reader:StreamReader) (writer:StreamWriter) =
    if (reader.EndOfStream) then ()
    else
      let currentLine = reader.ReadLine()
      let currentWords = StringUtils.getQuotedStrings currentLine
      if (currentWords.Head <> id) then writer.WriteLine(currentLine)
      deleteLineWith_ id reader writer
    
  let private deleteLineWith id (filePath:string) =
    let tempPath = filePath + ".tmp"
    let reader = new StreamReader(filePath)
    let writer = new StreamWriter(tempPath)
    try
      deleteLineWith_ id reader writer |> ignore
    finally
      reader.Close() |> ignore
      writer.Close() |> ignore
    File.Delete(filePath)
    File.Move(tempPath, filePath)

  // Given open stream, return string list of line with matching id
  let rec private getLineWith_ id (reader:StreamReader) =
    if (reader.EndOfStream) then []
    else
      let currentLine = reader.ReadLine()
      let currentWords = StringUtils.getQuotedStrings currentLine
      if (currentWords.Head = id) then currentWords
      else getLineWith_ id reader

  // Return string list of line (list contains line's values) with matching id
  let private getLineWith id (filePath:string) =
    if (File.Exists(filePath)) then
      use reader = new StreamReader(filePath)
      getLineWith_ id reader
    else []

  // Given an open stream, return max id of file
  let rec private getMaxId_ id currentMax (reader:StreamReader) =
    if (reader.EndOfStream) then currentMax
    else
      let currentLine = reader.ReadLine()
      let currentFields = StringUtils.getQuotedStrings currentLine
      match currentFields with
      | [] -> currentMax  // blank line for some reason
      | head :: t -> let currentId = System.Convert.ToInt32(head)
                     if (currentId < currentMax) then
                       getMaxId_ id currentMax reader
                     else 
                       getMaxId_ id currentId reader

  // Return max id of file
  let private getMaxId (filePath:string) =
    if (File.Exists(filePath)) then
      try
        use reader = new StreamReader(filePath)
        getMaxId_ id 0 reader
      with
        | :? System.FieldAccessException -> 0 // async, tests show attempt to access, file doesn't exist directly after check for existence returns true.
    else
      0

  // param fileName is one of RecordTypes' file names
  let private nextId fileName =
    match fileName with
    // You'd think you could match on a value like RecordTypes.RegistrantRecord, but you can't
    | "Registrant.csv" -> getMaxId (Path.Combine(dataDirectory, RecordTypes.registrantFileName)) + 1
    | "Registration.csv" -> getMaxId (Path.Combine(dataDirectory, RecordTypes.registrationFileName)) + 1
    | "Session.csv" -> getMaxId (Path.Combine(dataDirectory, RecordTypes.sessionFileName)) + 1
    | "Itinerary.csv" -> getMaxId (Path.Combine(dataDirectory, RecordTypes.itineraryFileName)) + 1
    | _ -> failwith (sprintf "%s: unexpected file name value" fileName)

  // param fileName is one of RecordTypes' file names
  let public AddRecord fileName (list:string List) =
    let id = (nextId fileName).ToString()
    match fileName with
    // You'd think you could match on a value like RecordTypes.RegistrantRecord, but you can't
    | "Registrant.csv" -> if (list.Length = 5) then
                            addRecord (EventRegistrationRecord.RegistrantRecord((id, list.[0], list.[1], list.[2], list.[3], list.[4])))
                          else failwith (sprintf "%s: expected list length of 5" fileName)
    | "Registration.csv" -> if (list.Length = 1) then
                              addRecord (EventRegistrationRecord.RegistrationRecord((id, list.[0])))
                            else failwith (sprintf "%s: expected list length of 1" fileName)
    | "Session.csv" -> if (list.Length = 3) then
                         addRecord (EventRegistrationRecord.SessionRecord((id, list.[0], list.[1], list.[2])))
                       else failwith (sprintf "%s: expected list length of 3" fileName)
    | "Itinerary.csv" -> if (list.Length > 1) then
                           let sessionList = List.skip 1 list  // peel off the session list
                           addRecord (EventRegistrationRecord.ItineraryRecord((id, list.[0], sessionList)))
                         else failwith (sprintf "%s: expected list length > 1" fileName)
    | _ -> failwith (sprintf "%s: unexpected value" fileName)

  // Pass id and the type of record.  I may want to transform these into tuples later.
  // param fileName is one of RecordTypes' file names
  let public GetRecord id fileName =
    match fileName with
    // You'd think you could match on a value like RecordTypes.RegistrantRecord, but you can't
    | "Registrant.csv" -> getLineWith id (Path.Combine(dataDirectory, RecordTypes.registrantFileName))
    | "Registration.csv" -> getLineWith id (Path.Combine(dataDirectory, RecordTypes.registrationFileName))
    | "Session.csv" -> getLineWith id (Path.Combine(dataDirectory, RecordTypes.sessionFileName))
    | "Itinerary.csv" -> getLineWith id (Path.Combine(dataDirectory, RecordTypes.itineraryFileName))
    | _ -> failwith (sprintf "%s: unexpected file name value" fileName)

  // Load session records into list.  Start with empty list
  let rec private loadSessions (sessionList:string List List) currentId maxId =
    let nextSession (nextList:string List List) =
      if currentId >= maxId then
        nextList
      else
        loadSessions nextList (currentId + 1) maxId
    let currentRecord = GetRecord (currentId.ToString()) "Session.csv"
    match currentRecord with
    | [] -> nextSession sessionList // blank line; skip it.
    | _ -> nextSession (currentRecord::sessionList)

  // Pass id and the type of record.
  // param fileName is one of RecordTypes' file names
  let public DeleteRecord id fileName =
    match fileName with
    // You'd think you could match on a value like RecordTypes.RegistrantRecord, but you can't
    | "Registrant.csv" -> deleteLineWith id (Path.Combine(dataDirectory, RecordTypes.registrantFileName))
    | "Registration.csv" -> deleteLineWith id (Path.Combine(dataDirectory, RecordTypes.registrationFileName))
    | "Session.csv" -> deleteLineWith id (Path.Combine(dataDirectory, RecordTypes.sessionFileName))
    | "Itinerary.csv" -> deleteLineWith id (Path.Combine(dataDirectory, RecordTypes.itineraryFileName))
    | _ -> failwith (sprintf "%s: unexpected file name value" fileName)

  // param fileName is one of RecordTypes' file names
  let public DeleteFile fileName =
    let mutable path = ""
    match fileName with
    // You'd think you could match on a value like RecordTypes.RegistrantRecord, but you can't
    | "Registrant.csv" -> path <- (Path.Combine(dataDirectory, RecordTypes.registrantFileName))
    | "Registration.csv" -> path <- (Path.Combine(dataDirectory, RecordTypes.registrationFileName))
    | "Session.csv" -> path <- (Path.Combine(dataDirectory, RecordTypes.sessionFileName))
    | "Itinerary.csv" -> path <- (Path.Combine(dataDirectory, RecordTypes.itineraryFileName))
    | _ -> ()
    if (File.Exists(path)) then File.Delete(path) |> ignore

  let public GetAllSessions() =
    let maxId = getMaxId (Path.Combine(dataDirectory, RecordTypes.sessionFileName))
    loadSessions [] 1 maxId
