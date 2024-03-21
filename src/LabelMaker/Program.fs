namespace LabelMaker

module Program =

    [<EntryPoint>]
    let main args =

        let owner = "jonsagara"
        let repo = "Sagara.Core"

        // Create a fine-grained personal access token with the following permissions:
        // * Issues: read and write
        // * Pull requests: read and write (a pull request is still an issue)
        let token = ""
        
        GitHubApi.createPriorityLabelsAsync owner repo token
        |> Async.AwaitTask
        |> Async.RunSynchronously

        0