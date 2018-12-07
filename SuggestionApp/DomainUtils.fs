module DomainUtils
open Domain

[<RequireQualifiedAccess>]
module User = 
    let tryFindByEmail targetEmail (users: User list) =
        users
        |> List.tryFind(fun currentUser -> 
            match currentUser with 
            | NormalUser { Email = email } 
            | AdminUser { Email = email }
                -> email = targetEmail)

[<RequireQualifiedAccess>]
module Suggestion = 
    let tryFindById sid (suggestions: Suggestion list) = 
        suggestions
        |> List.tryFind(fun elem -> 
            match elem with
            | Proposed { Suggestion = suggestion }
            | Responded { Suggestion = suggestion }
            | Closed { Suggestion = suggestion }
                -> suggestion.Id = sid)

    let replaceInList (predicate: Suggestion -> bool) (replacement: Suggestion) (currentSuggestions: Suggestion list) = 
        currentSuggestions
        |> List.map (fun elem -> 
            if predicate elem then
                replacement
            else 
                elem)

[<RequireQualifiedAccess>]
module ResponseCategory = 
    let tryParse (categoryString: string) = 
        match categoryString.Trim().ToLowerInvariant() with
        | "approved" -> Some Approved
        | "considering" -> Some Considering
        | "needsmoredetail" -> Some NeedsMoreDetail
        | "rejected" -> Some Rejected
        | _ -> None
