namespace AutoTogglConsole.FSharp

open System
open System.IO
open FSharp.Data.JsonProvider

module ConfigOps = 
    type Config = JsonProvider<"config.json", RootTypeName = "Config">

    let private _ReadConfig() = File.ReadAllText "config.json"
    let GetConfig() = 
        let c = _ReadConfig() |> Config.Parse
        let msg = 
            match c.Project.Length with
            | 1 -> sprintf "Found 1 project:"
            | _ -> sprintf "Found %i projects:" c.Project.Length
        printfn "%s" msg
        c.Project |> Seq.iter (fun p -> printfn "\t[%i] %s" p.Id p.Name)
        c