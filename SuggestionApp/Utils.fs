module Utils
open System.Text.RegularExpressions

let splitLineIntoArgs input = 
    // From https://stackoverflow.com/questions/4780728/regex-split-string-preserving-quotes/4780801#4780801
    let rawArgs = Regex.Split(input, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)")
    rawArgs |> Array.map (fun arg -> arg.Trim('\"'))
