namespace AutoTogglConsole.FSharp

open System
open System.IO
open Newtonsoft.Json
open JFSharpKit
open JFSharpKit.Kits
open Chessie.ErrorHandling

module Settings = 
    type PossibleErrors = 
        | ConfigDoesNotExist
        | ConfigFileIsEmpty
        | CouldNotLoadConfig
        | FailedToDeserialize of string
        | MissingApiToken
        | MissingWorkspaceId
        static member StringifyErrors =
            function
            | ConfigDoesNotExist -> "The configuration file does not exist."
            | ConfigFileIsEmpty -> "The configuration file is empty."
            | CouldNotLoadConfig -> "Could not locate or read the config file."
            | FailedToDeserialize s -> sprintf "Failed to deserialize due to %s" s
            | MissingApiToken -> "Missing API Token."
            | MissingWorkspaceId -> "Missing Workspace Id."
    type Project =
        { id: int
          wid: int
          cid: int
          pid: int
          name: string
          active: bool
          keywords: string[] }
    type MyConfig = 
        { ApiToken: string
          WorkspaceId: int
          Delay: int
          NeutralWindows: string[]
          Projects: Project[] }
    
    let private validateConfigExists path =
        if File.Exists path then ok path
        else
            try
                File.Create path |> ignore
                ok path
            with ex -> failwith ex.Message

    let private readOrFail path =
        try File.ReadAllText path |> ok
        with ex -> PossibleErrors.CouldNotLoadConfig |> fail

    let private deserializeFile contents =
        if contents <> "" then
            try contents |> JsonConvert.DeserializeObject<MyConfig> |> ok
            with e -> e.Message |> PossibleErrors.FailedToDeserialize |> fail
        else
            PossibleErrors.ConfigFileIsEmpty |> fail

    let private validateAnonymous (prop: string) (pe: PossibleErrors) =
        if prop = "" then pe |> fail
        else prop |> ok

    let private validateApiToken cfg = validateAnonymous cfg.ApiToken MissingApiToken

    //let private _ReadConfig() = 
    //    FileKit.ReadIfExists "config.json"
    //    |> function
    //    | Some contents -> contents |> ok
    //    | None -> PossibleErrors.CouldNotLoadConfig |> fail

    let GetConfig() = 
        "config.json"
        |> validateConfigExists
        |> bind readOrFail
        |> bind deserializeFile
        |> bind validateApiToken
        |> function
        | Ok (d,_) -> d |> JsonConvert.DeserializeObject<MyConfig>
        | Bad p -> 
            p 
            |> Seq.map PossibleErrors.StringifyErrors
            |> String.concat ".\n"
            |> failwith

        //|> JsonConvert.DeserializeObject<MyConfig>
        //let msg = 
        //    match c.Projects.Length with
        //    | 1 -> sprintf "Found 1 project:"
        //    | _ -> sprintf "Found %i projects:" c.Projects.Length
        //printfn "%s" msg
        //c.Projects |> Seq.iter (fun p -> printfn "\t[%i] %s" p.Id p.Name)
