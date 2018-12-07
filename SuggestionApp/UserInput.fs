module UserInput

open CommandLine
open Domain

[<Verb("reset", HelpText = "Reset screen to welcome message")>]
type ResetScreenOptions () = class end

[<Verb("exit", HelpText = "Exit Application")>]
type ExitOptions () = class end

type UiCommand = 
    | ExitCommand
    | ResetScreenCommand
    | PreHandledCommand

type UiRequest = 
    | UiCommand of UiCommand
    | UiError of string

// verb Vote ? Up / down id
// verb comment? Comment id

let tryProcessInput (inputArgs: string []) = 
    let result = 
        Parser.Default.ParseArguments<
            ResetScreenOptions,
            ExitOptions> inputArgs
    match result with
    | :? CommandLine.Parsed<obj> as command ->
        match command.Value with
        | :? ResetScreenOptions -> UiCommand ResetScreenCommand
        | :? ExitOptions -> UiCommand ExitCommand
        | unkownParsedvalue -> 
            UiError <| sprintf "Unexpected verb options was parsed. Missing Option configuration?: %s: %A." (unkownParsedvalue.GetType().Name) unkownParsedvalue
    | :? CommandLine.NotParsed<obj> ->
        // Parsing library already handles outputing help for bad command
        UiCommand PreHandledCommand
    | unkownParsedvalue -> UiError <| sprintf "Input could not be handled properly: %A." unkownParsedvalue