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

type ResponseCategory = 
    | Approved
    | Considering
    | NeedsMoreDetail
    | Rejected

type ResponseInfo = {
    Responder: AdminInfo
    ResponseDate: DateTime
    Category: ResponseCategory
    Notes: string
}

type SuggestionWithResponse = {
    Suggestion: SuggestionInfo
    Response: ResponseInfo
}

type Suggestion = 
    | Proposed of ProposedSuggestion
    | Responded of SuggestionWithResponse

type AppRequest =
    | GetUser
    | PostChangeUser of string
    | GetAllSuggestions
    | GetSuggestion of int
    | PostSuggestion of title: string * description: string
    | PostSuggestionResponse of sid: int * category: string * notes: string

type AppState = {
    Users: User list
    CurrentUser: User
    Suggestions: Suggestion list
    NextId: int
}