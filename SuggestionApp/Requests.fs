module Requests

open System
open Domain
open DomainUtils

let getUser (state: AppState) =
    Ok {
        Output = sprintf "Current User: %A" state.CurrentUser
        NextState = state
    }

let postChangeUser (state: AppState) targetEmail = 
    match User.tryFindByEmail targetEmail state.Users with
    | None -> 
        Error { 
            Output = sprintf "Error: Could not find user with email: %s" targetEmail
            NextState = state
        }
    | Some user -> 
        Ok {
            Output = sprintf "Current User: %A" user
            NextState = { state with CurrentUser = user }
        }

let getSuggestion (state: AppState) suggestionId = 
    match Suggestion.tryFindById suggestionId state.Suggestions with
    | None -> 
        Error { 
            Output = sprintf "Error: Could not find suggestion with id: %d" suggestionId
            NextState = state
        }
    | Some suggestion ->
        Ok {
            Output = sprintf "Suggestion %d: %A" suggestionId suggestion  
            NextState = state
        }

let getAllSuggestions (state: AppState) = 
    let output = 
        state.Suggestions
        |> List.map(fun elem -> sprintf "%A" elem )
        |> String.concat "\n"
    Ok {
        Output = output
        NextState = state
    }

let postSuggestion title description (state: AppState) = 
    let sid = state.NextId
    let newSuggestion = Proposed {
        Suggestion = {
            Id = sid
            Title = title
            Description = description
            Creator = state.CurrentUser
            ProposedDate = DateTime.UtcNow
        }
    }
    Ok {
        Output = sprintf "Suggestion with id %d was created" sid
        NextState = { state with NextId = state.NextId + 1; Suggestions = newSuggestion::state.Suggestions }
    }
    
let useCurrentUserIfAdmin state = 
    match state.CurrentUser with
    | NormalUser _ -> 
        Error { 
            Output = "Current user is not an admin and cannot respond to a suggestion"
            NextState = state
        }
    | AdminUser admin -> 
        Ok admin

let tryParseCategory category state = 
    match ResponseCategory.tryParse category with 
    | None -> 
        Error { 
            Output = sprintf "Could not match category: %s to a system option. Options are: %A" category (typedefof<ResponseCategory>)
            NextState = state
        }
    | Some rCategory->
        Ok rCategory

let trySelectSuggestion sid state =
    match Suggestion.tryFindById sid state.Suggestions with
    | None -> 
        Error { 
            Output = sprintf "Could not find suggestion with id: %d" sid
            NextState = state
        }
    | Some suggestion -> 
        Ok suggestion

let postSuggestionResponse' sid category notes (state: AppState) = 
    state
    |> useCurrentUserIfAdmin
    |> Result.bind(fun admin -> 
        tryParseCategory category state
        |> Result.bind(fun category -> Ok (admin, category))
        )
    |> Result.bind(fun (admin, category) -> 
        trySelectSuggestion sid state
        |> Result.bind(fun suggestion -> Ok (admin, category, suggestion))
        )
    |> Result.bind(fun (admin, category, suggestion) -> 
        match suggestion with
        | Proposed { Suggestion = sInfo } -> 
            let newSuggestion = 
                Responded {
                    Suggestion = sInfo
                    Response = {
                        Responder = admin
                        Category = category
                        Notes = notes
                        ResponseDate = DateTime.UtcNow
                    }
                }
            let suggestions' = Suggestion.replaceInList (fun s -> s = suggestion) newSuggestion state.Suggestions
            Ok {
                Output = sprintf "Updated Suggestion: %A" newSuggestion
                NextState = { state with Suggestions = suggestions' }
            }

        | Responded _
        | Closed _ -> 
            Error {
                Output = sprintf "Suggestion with id: %d was not in the proposed state, so it could not be responded to" sid
                NextState = state
            }
    )

let postSuggestionResponse sid category notes (state: AppState) = 
    match state.CurrentUser with
    | NormalUser _ -> 
        Error { 
            Output = "Current user is not an admin and cannot respond to a suggestion"
            NextState = state
        }
    | AdminUser admin -> 
        match ResponseCategory.tryParse category with 
        | None -> 
            Error { 
                Output = sprintf "Could not match category: %s to a system option. Options are: %A" category (typedefof<ResponseCategory>)
                NextState = state
            }
        | Some rCategory -> 
            match Suggestion.tryFindById sid state.Suggestions with
            | None -> 
                Error {
                    Output = sprintf "Could not find suggestion with id: %d" sid
                    NextState = state
                }
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
                    Ok {
                        Output = sprintf "Updated Suggestion: %A" newSuggestion
                        NextState = { state with Suggestions = suggestions' }
                    }
                | Responded _
                | Closed _ -> 
                    Error {
                        Output = sprintf "Suggestion with id: %d was not in the proposed state, so it could not be responded to" sid
                        NextState = state
                    }

let postCloseSuggestion sid notes (state: AppState) = 
    match state.CurrentUser with
    | NormalUser _ -> 
         Error { 
            Output = "Current user is not an admin and cannot respond to a suggestion"
            NextState = state
        }
    | AdminUser admin -> 
        match Suggestion.tryFindById sid state.Suggestions with
        | None -> 
            Error {
                Output = sprintf "Could not find suggestion with id: %d" sid
                NextState = state
            }
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
                Ok {
                    Output = sprintf "Updated Suggestion: %A" newSuggestion
                    NextState = { state with Suggestions = suggestions' }
                }
            | Proposed _ 
            | Closed _ -> 
                Error {
                    Output = sprintf "Suggestion with id: %d was not in the responded state, so it could not be closed to" sid
                    NextState = state
                }

let processRequest (request: AppRequest) (state: AppState) = 
    match request with
    | GetUser -> getUser state
    | PostChangeUser targetEmail -> postChangeUser state targetEmail
    | GetSuggestion sid -> getSuggestion state sid
    | GetAllSuggestions -> getAllSuggestions state
    | PostSuggestion(title, description) -> postSuggestion title description state
    | PostSuggestionResponse(sid, category, notes) -> postSuggestionResponse sid category notes state
    | PostCloseSuggestion(sid, notes) -> postCloseSuggestion sid notes state
