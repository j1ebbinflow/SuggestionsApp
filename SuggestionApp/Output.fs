module Output

open System
open Domain

let resetScreen () = 
    Console.Clear()
    printfn "Suggestions App"

let displayFatal message = 
    resetScreen ()
    printfn "An Fatal Error occured: %s. Exiting." message

let displayUser (user: User) = 
    resetScreen ()
    printfn "Current User: %A" user
