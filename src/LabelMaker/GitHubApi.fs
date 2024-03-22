namespace LabelMaker

open System
open System.Linq
open System.Text.Json
open Octokit

module GitHubApi =

    /// Create the new labels in the repo.
    let createLabelsAsync (owner : string) (repo : string) (token : string) =
        task {
            ArgumentException.ThrowIfNullOrWhiteSpace(owner, nameof owner)
            ArgumentException.ThrowIfNullOrWhiteSpace(repo, nameof repo)
            ArgumentException.ThrowIfNullOrWhiteSpace(token, nameof token)

            // Set up the GitHub client.
            let githubClient = GitHubClient(new ProductHeaderValue("LabelMaker", "1.0"))
            githubClient.Credentials <- Credentials(token)

            
            //
            // Get the existing labels from the repo.
            //

            let! existingLabels = githubClient.Issue.Labels.GetAllForRepository(owner = owner, name = repo)
            
            // Get a case-insensitive, distinct list of label names.
            let existingLabelNames = 
                existingLabels
                    .Select(_.Name)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase)

            // Only create new labels where the name does not yet exist.
            let newLabelsToCreate =
                Data.NewLabels
                |> Array.filter (fun nl -> not(existingLabelNames.Contains(nl.Name)))


            //
            // Create the new labels.
            //

            for newLabel in newLabelsToCreate do

                printfn $"Creating label with name {newLabel.Name}, color {newLabel.Color}, and description {newLabel.Description}..."
                let! response = githubClient.Issue.Labels.Create(owner = owner, name = repo, newLabel = newLabel)
                printfn $"Done. Response: {JsonSerializer.Serialize(response)}"
        }

