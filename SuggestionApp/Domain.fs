module Domain

open System

type NameInfo = {
    FirstName: string
    MiddleName: string option
    LastName: string
}

type UserInfo = {
    Id: int
    Name: NameInfo
    Email: string
}

type AdminInfo = {
    Id: int
    Name: NameInfo
    Email: string
}

type User = 
    | NormalUser of UserInfo
    | AdminUser of AdminInfo

type SuggestionInfo = {
    Id: int
    Title: string
    Description: string
    Creator: User
    ProposedDate: DateTime
}

type ProposedSuggestion = {
    Suggestion: SuggestionInfo
}

type Suggestion = 
    | Proposed of ProposedSuggestion

type AppRequest =
    | GetUser
    | PostChangeUser of string
    | GetAllSuggestions
    | GetSuggestion of int
    | PostSuggestion of title: string * description: string

type AppState = {
    Users: User list
    CurrentUser: User
    Suggestions: Suggestion list
    NextId: int
}