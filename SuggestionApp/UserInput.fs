module UserInput

open CommandLine
open Domain

[<Verb("reset", HelpText = "Reset screen to welcome message")>]
type ResetScreenOptions () = class end

[<Verb("exit", HelpText = "Exit Application")>]
type ExitOptions () = class end

[<Verb("user", HelpText = "Display current user")>]
type DisplayUserOptions () = class end

[<Verb("changeUser", HelpText = "Select current user")>]
type ChangeUserOptions = {
    [<Option("email", Required = true, HelpText="Email of user to 'Login' as")>] email: string;
}

[<Verb("getSuggestions", HelpText = "Get all suggestions")>]
type GetAllSuggestionsOptions () = class end

[<Verb("getSuggestion", HelpText = "Get Suggestion by id")>]
type GetSuggestionOptions = {
    [<Option("id", Required = true, HelpText="Id of suggestion")>] sid: int;
}

[<Verb("addSuggestion", HelpText = "Add suggestion")>]
type AddSuggestionOptions = {
    [<Option("title", Required = true, HelpText="Title of suggestion")>] title: string
    [<Option('d', "Description", Required = true, HelpText="Description of suggestion")>] description: string
}

type UiCommand = 
    | ExitCommand
    | ResetScreenCommand
    | PreHandledCommand

type UiRequest = 
    | UiCommand of UiCommand
    | UiError of string
    | AppRequest of AppRequest

// verb Vote ? Up / down id
// verb comment? Comment id

let tryProcessInput (inputArgs: string []) = 
    let result = 
        Parser.Default.ParseArguments<
            ResetScreenOptions,
            ExitOptions,
            DisplayUserOptions,
            ChangeUserOptions,
            GetAllSuggestionsOptions,
            GetSuggestionOptions,
            AddSuggestionOptions> inputArgs
    match result with
    | :? CommandLine.Parsed<obj> as command ->
        match command.Value with
        | :? ResetScreenOptions -> UiCommand ResetScreenCommand
        | :? ExitOptions -> UiCommand ExitCommand
        | :? DisplayUserOptions -> AppRequest GetUser
        | :? ChangeUserOptions as options-> AppRequest <| PostChangeUser options.email
        | :? GetAllSuggestionsOptions -> AppRequest GetAllSuggestions
        | :? GetSuggestionOptions as options -> AppRequest <| GetSuggestion options.sid
        | :? AddSuggestionOptions as options -> AppRequest <| PostSuggestion(options.title, options.description)
        | unkownParsedvalue -> 
            UiError <| sprintf "Unexpected verb options was parsed. Missing Option configuration?: %s: %A." (unkownParsedvalue.GetType().Name) unkownParsedvalue
    | :? CommandLine.NotParsed<obj> ->
        // Parsing library already handles outputing help for bad command
        UiCommand PreHandledCommand
    | unkownParsedvalue -> UiError <| sprintf "Input could not be handled properly: %A." unkownParsedvalue