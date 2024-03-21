namespace LabelMaker

open System.Text.Json.Serialization

/// A label that already exists in GitHub.
type ExistingLabel = {
    [<JsonPropertyName("id")>]
    Id : int64
    [<JsonPropertyName("node_id")>]
    NodeId : string
    [<JsonPropertyName("url")>]
    Url : string
    [<JsonPropertyName("name")>]
    Name : string
    [<JsonPropertyName("color")>]
    Color : string
    [<JsonPropertyName("default")>]
    Default : bool
    [<JsonPropertyName("description")>]
    Description : string
    }
    
/// A label to create in GitHub.
type NewLabel = {
    [<JsonPropertyName("name")>]
    Name : string
    [<JsonPropertyName("color")>]
    Color : string
    [<JsonPropertyName("description")>]
    Description : string
    }

/// The new labels to create.
module Data =
    let NewLabels = [
        { Name = "priority:1"; Color = "CE0E18"; Description = "Highest" }
        { Name = "priority:2"; Color = "D85A10"; Description = "High" }
        { Name = "priority:3"; Color = "7A7737"; Description = "Medium" }
        { Name = "priority:4"; Color = "0AC8A9"; Description = "Low" }
        { Name = "priority:5"; Color = "4B8DF0"; Description = "Lowest" }
        { Name = "improvement"; Color = "A8DBA3"; Description = "Improvements to the project" }
        ]

