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

let processRequest (request: AppRequest) (state: AppState) = 
    match request with
    | GetUser -> getUser state
    | PostChangeUser targetEmail -> postChangeUser state targetEmail
    | GetSuggestion sid -> getSuggestion state sid
    | GetAllSuggestions -> getAllSuggestions state
    | PostSuggestion(title, description) -> postSuggestion title description state