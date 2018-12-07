module Requests

open System
open Domain
open DomainUtils

let getUser (state: AppState) =
    Output.displayUser state.CurrentUser
    state

let postChangeUser (state: AppState) targetEmail = 
    match User.tryFindByEmail targetEmail state.Users with
    | None -> 
        printfn "Could not find user with email: %s" targetEmail
        state
    | Some user -> 
        let newState = { state with CurrentUser = user}
        Output.displayUser user
        newState

let getSuggestion (state: AppState) suggestionId = 
    match Suggestion.tryFindById suggestionId state.Suggestions with
    | None -> 
        printfn "Could not find suggestion with id: %d" suggestionId
        state
    | Some suggestion -> 
        printfn "Suggestion %d: %A" suggestionId suggestion   
        state

let getAllSuggestions (state: AppState) = 
    state.Suggestions
    |> List.iter(fun elem -> printfn "%A" elem )
    state

let postSuggestion title description (state: AppState) = 
    let newSuggestion = Proposed {
        Suggestion = {
            Id = state.NextId
            Title = title
            Description = description
            Creator = state.CurrentUser
            ProposedDate = DateTime.UtcNow
        }
    }
    { state with NextId = state.NextId + 1; Suggestions = newSuggestion::state.Suggestions }

let postSuggestionResponse sid category notes (state: AppState) = 
    match state.CurrentUser with
    | NormalUser _ -> 
        printfn "Current user is not an admin and cannot respond to a suggestion"
        state
    | AdminUser admin -> 
        match ResponseCategory.tryParse category with 
        | None -> 
            printfn "Could not match category: %s to a system option. Options are: %A" category (typedefof<ResponseCategory>)
            state
        | Some rCategory -> 
            match Suggestion.tryFindById sid state.Suggestions with
            | None -> 
                printfn "Could not find suggestion with id: %d" sid
                state
            | Some suggestion -> 
                match suggestion with
                | Proposed { Suggestion = sInfo } -> 
                    let newSuggestion = 
                        Responded {
                            Suggestion = sInfo
                            Response = {
                                Responder = admin
                                Category = rCategory
                                Notes = notes
                                ResponseDate = DateTime.UtcNow
                            }
                        }
                    let suggestions' = Suggestion.replaceInList (fun s -> s = suggestion) newSuggestion state.Suggestions
                    { state with Suggestions = suggestions' }
                | Responded _
                | Closed _ -> 
                    printfn "Suggestion with id: %d was not in the proposed state, so it could not be responded to" sid
                    state

let postCloseSuggestion sid notes (state: AppState) = 
    match state.CurrentUser with
    | NormalUser _ -> 
        printfn "Current user is not an admin and cannot respond to a suggestion"
        state
    | AdminUser admin -> 
        match Suggestion.tryFindById sid state.Suggestions with
        | None -> 
            printfn "Could not find suggestion with id: %d" sid
            state
        | Some suggestion -> 
            match suggestion with
            | Responded sug ->
                let newSuggestion = 
                    Closed { 
                        Suggestion = sug.Suggestion
                        Response = sug.Response
                        Closure = {
                            Closer = admin
                            Notes = notes
                            ClosureDate = DateTime.UtcNow
                        }
                    }
                let suggestions' = Suggestion.replaceInList (fun s -> s = suggestion) newSuggestion state.Suggestions
                { state with Suggestions = suggestions' }
            | Proposed _ 
            | Closed _ -> 
                printfn "Suggestion with id: %d was not in the proposed state, so it could not be responded to" sid
                state

let processRequest (request: AppRequest) (state: AppState) = 
    match request with
    | GetUser -> getUser state
    | PostChangeUser targetEmail -> postChangeUser state targetEmail
    | GetSuggestion sid -> getSuggestion state sid
    | GetAllSuggestions -> getAllSuggestions state
    | PostSuggestion(title, description) -> postSuggestion title description state
    | PostSuggestionResponse(sid, category, notes) -> postSuggestionResponse sid category notes state
    | PostCloseSuggestion(sid, notes) -> postCloseSuggestion sid notes state
<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes
