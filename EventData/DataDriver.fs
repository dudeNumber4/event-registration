﻿namespace EventData

module DataDriver =
  open System.Reflection
  open System.IO
  open RecordTypes
  
  // A common solution root location
  let private defaultPath = 
    let result = Assembly.GetExecutingAssembly().Location |> Path.GetDirectoryName
    $"{result}..\..\..\..\.."
  
  let mutable DataPath = defaultPath
  
  let internal getDataFilePath record =
    match record with
    | RegistrantRecord r -> Path.Combine(DataPath, RecordTypes.registrantFileName)
    | SessionRecord r -> Path.Combine(DataPath, RecordTypes.sessionFileName)
    | RegistrationRecord r -> Path.Combine(DataPath, RecordTypes.registrationFileName)

  let private FlushFile fileName =
    let fullPath = Path.Combine(DataPath, fileName)
    if (File.Exists fullPath) then File.Delete fullPath
  
  let public Set (path) = DataPath <- path
  
  // param fileName is one of RecordTypes' file names
  let public DeleteFile fileName =
    let mutable path = ""
    match fileName with
    // You'd think you could match on a value like RecordTypes.RegistrantRecord, but you can't
    | "Registrant.csv" -> path <- (Path.Combine(DataPath, RecordTypes.registrantFileName))
    | "Session.csv" -> path <- (Path.Combine(DataPath, RecordTypes.sessionFileName))
    | "Registration.csv" -> path <- (Path.Combine(DataPath, RecordTypes.registrationFileName))
    | _ -> ()
    if (File.Exists(path)) then File.Delete(path) |> ignore

  let public Flush = 
    FlushFile RecordTypes.sessionFileName
    FlushFile RecordTypes.registrantFileName
    FlushFile RecordTypes.registrationFileName