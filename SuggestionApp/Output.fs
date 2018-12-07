module Output

open System

let resetScreen () = 
    Console.Clear()
    printfn "Suggestions App"

let displayFatal message = 
    resetScreen ()
    printfn "An Fatal Error occured: %s. Exiting." message

let displayMessage successMessage message = 
    resetScreen ()
    printfn "Request %s. Response:\n%s" successMessage message

