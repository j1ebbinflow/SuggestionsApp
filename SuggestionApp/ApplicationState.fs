module ApplicationState

open Domain
open System

let adminUser = AdminUser { 
    Id = 1
    Name = {
        FirstName = "Adrian"
        MiddleName = None
        LastName = "User"
    }
    Email = "admin@s.com"
}

let normalUser = NormalUser { 
    Id = 2
    Name = {
        FirstName = "Norman"
        MiddleName = None
        LastName = "User"
    }
    Email = "normal@s.com"
}

let suggestionOne = Proposed {
    Suggestion = {
        Id = 1
        Title = "Office Coat Racks"
        Description = "Find some coat racks for the office."
        Creator = normalUser
        ProposedDate = DateTime.UtcNow
    }
}

let suggestionTwo = Proposed { 
    Suggestion = {
        Id = 2
        Title = "Desk Storage"
        Description = "Find some under desk storage to help keep desks tidy"
        Creator = adminUser
        ProposedDate = DateTime.UtcNow
    }
}

let initialState = {
    Users = [normalUser; adminUser]
    CurrentUser = normalUser
    Suggestions = [suggestionOne; suggestionTwo]
    NextId = 3
}
