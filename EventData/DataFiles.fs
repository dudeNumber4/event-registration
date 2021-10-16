namespace EventData

module DataUtils =
  open System.IO
  open System
  open RecordTypes
 
  let private ensureDataFileExists filePath =
    if not (File.Exists(filePath)) then
      try
        use f = File.CreateText(filePath)
        ()  // use can't be the last statement :(
      with
        | :? System.FieldAccessException -> () // async, tests show create directly after check for existence fails.

  let private addRecord record =
    let fileName = DataDriver.getDataFilePath record
    match record with
    | RegistrantRecord r -> ensureDataFileExists fileName |> ignore
                            let id, firstName, LastName, Email, OrgName, Industry = r
                            let csv = $"\"{id}\",\"{firstName}\",\"{LastName}\",\"{Email}\",\"{OrgName}\",\"{Industry}\"{Environment.NewLine}"
                            File.AppendAllText(fileName, csv) |> ignore
    | SessionRecord r -> ensureDataFileExists fileName |> ignore
                         let id, Day, Title, Description = r
                         let csv = $"\"{id}\",\"{Day}\",\"{Title}\",\"{Description}\"{Environment.NewLine}"
                         File.AppendAllText(fileName, csv) |> ignore
    | RegistrationRecord r -> ensureDataFileExists fileName |> ignore
                              let id, registrationId, SessionIds = r
                              let csv = $"\"{id}\",\"{registrationId}\","
                              // add session id list to end of line
                              let newLine = (List.fold (fun accumulator current -> $"{accumulator},\"{current}\"") csv SessionIds) + Environment.NewLine
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
  let public NextId fileName =
    match fileName with
    // You'd think you could match on a value like RecordTypes.RegistrantRecord, but you can't
    | "Registrant.csv" -> getMaxId (Path.Combine(DataDriver.DataPath, RecordTypes.registrantFileName)) + 1
    | "Session.csv" -> getMaxId (Path.Combine(DataDriver.DataPath, RecordTypes.sessionFileName)) + 1
    | "Registration.csv" -> getMaxId (Path.Combine(DataDriver.DataPath, RecordTypes.registrationFileName)) + 1
    | _ -> failwith ($"{fileName}: unexpected file name value")

  let private AddRecordWithId fileName (list:string List) id =
    // nextId must be >= to id
    if (NextId fileName >= id) then
      match fileName with
      // You'd think you could match on a value like RecordTypes.RegistrantRecord, but you can't
      | "Registrant.csv" -> if (list.Length = 5) then
                              addRecord (EventRegistrationRecord.RegistrantRecord((id.ToString(), list.[0], list.[1], list.[2], list.[3], list.[4])))
                            else failwith ($"{fileName}: expected list length of 5")
      | "Session.csv" -> if (list.Length = 3) then
                           addRecord (EventRegistrationRecord.SessionRecord((id.ToString(), list.[0], list.[1], list.[2])))
                         else failwith ($"{fileName}: expected list length of 3")
      | "Registration.csv" -> if (list.Length > 0) then
                                                                          // peel off the session list
                                let sessionList = if (list.Length > 1) then List.skip 1 list else List.Empty
                                addRecord (EventRegistrationRecord.RegistrationRecord((id.ToString(), list.[0], sessionList)))
                              else failwith ($"{fileName}: expected list length > 0")
      | _ -> failwith ($"{fileName}: unexpected value")
      id
    else failwith $"Id {id} already present"

  // param fileName is one of RecordTypes' file names
  let public AddRecord fileName (list:string List) =
    AddRecordWithId fileName list (NextId fileName)

  // Pass id and the type of record.  I may want to transform these into tuples later.
  // param fileName is one of RecordTypes' file names
  let public GetRecord id fileName =
    match fileName with
    // You'd think you could match on a value like RecordTypes.RegistrantRecord, but you can't
    | "Registrant.csv" -> getLineWith id (Path.Combine(DataDriver.DataPath, RecordTypes.registrantFileName))
    | "Session.csv" -> getLineWith id (Path.Combine(DataDriver.DataPath, RecordTypes.sessionFileName))
    | "Registration.csv" -> getLineWith id (Path.Combine(DataDriver.DataPath, RecordTypes.registrationFileName))
    | _ -> failwith ($"{fileName}: unexpected file name value")

  // Can load session, registrant, registration with this func, but not itinerary because it's a different data structure.
  // Load records into list.  Start with empty list
  let rec private loadRecords (list:string List List) currentId maxId fileName =
    let nextRecord (nextList:string List List) =
      if currentId >= maxId then
        nextList
      else
        loadRecords nextList (currentId + 1) maxId fileName
    let currentRecord = GetRecord (currentId.ToString()) fileName
    match currentRecord with
    | [] -> nextRecord list // blank line; skip it.
    | _ -> nextRecord (currentRecord::list)

  let private GetAllRecords fileName =
    let maxId = getMaxId (Path.Combine(DataDriver.DataPath, fileName))
    loadRecords [] 1 maxId fileName

  // Pass id and the type of record.
  // param fileName is one of RecordTypes' file names
  let public DeleteRecord id fileName =
    match fileName with
    // You'd think you could match on a value like RecordTypes.RegistrantRecord, but you can't
    | "Registrant.csv" -> deleteLineWith id (Path.Combine(DataDriver.DataPath, RecordTypes.registrantFileName))
    | "Session.csv" -> deleteLineWith id (Path.Combine(DataDriver.DataPath, RecordTypes.sessionFileName))
    | "Registration.csv" -> deleteLineWith id (Path.Combine(DataDriver.DataPath, RecordTypes.registrationFileName))
    | _ -> failwith ($"{fileName}: unexpected file name value")

  let public UpdateRecord fileName (list:string List) =
    let id = List.head list
    DeleteRecord id fileName |> ignore
    AddRecordWithId fileName (List.tail list) (Convert.ToInt32(id))

  let public GetAllSessions() = GetAllRecords RecordTypes.sessionFileName
  let public GetAllRegistrants() = GetAllRecords RecordTypes.registrantFileName
  let public GetAllRegistrations() = GetAllRecords RecordTypes.registrationFileName
