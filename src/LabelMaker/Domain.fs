namespace LabelMaker

open Octokit

/// The new labels to create.
module Data =
    let NewLabels = [|
        new NewLabel (name = "priority:1", color = "CE0E18", Description = "Highest")
        new NewLabel (name = "priority:2", color = "D85A10", Description = "High")
        new NewLabel (name = "priority:3", color = "7A7737", Description = "Medium")
        new NewLabel (name = "priority:4", color = "0AC8A9", Description = "Low")
        new NewLabel (name = "priority:5", color = "4B8DF0", Description = "Lowest")
        new NewLabel (name = "improvement", color = "A8DBA3", Description = "Improvements to the project")
        |]

