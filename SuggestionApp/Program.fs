open System
open UserInput
open ApplicationState
open Domain
open Requests
open Utils

let rec appCycle (state: AppState) =
    let input = Console.ReadLine()
    Output.resetScreen ()
    let uiRequest =
        input
        |> splitLineIntoArgs
        |> tryProcessInput

    match uiRequest with
    | UiError message -> 
        Output.displayFatal message
        ()
    | UiCommand ExitCommand -> ()
    | UiCommand PreHandledCommand -> appCycle state
    | UiCommand ResetScreenCommand -> 
        Output.resetScreen ()
        appCycle state
    | AppRequest request ->   
        match processRequest request state with
        | Error data -> 
            Output.displayMessage "Failed" data.Output
            appCycle data.NextState
        | Ok data -> 
            Output.displayMessage "Succeeded" data.Output
            appCycle data.NextState

let startAppCycle state =
    Output.resetScreen()
    appCycle state

[<EntryPoint>]
let main _ =
    startAppCycle initialState
    0
