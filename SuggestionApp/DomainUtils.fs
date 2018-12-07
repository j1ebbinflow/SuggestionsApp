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
                -> suggestion.Id = sid)