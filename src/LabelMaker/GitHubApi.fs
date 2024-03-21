namespace LabelMaker

open System
open System.Linq
open System.Net.Http
open System.Net.Http.Headers
open System.Text.Json
open System.Text

module GitHubApi =

    let private initHttpClient () =
        let httpClient = new HttpClient()
        httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28")
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("LabelMaker/1.0")
        httpClient

    // Storing this in a static field is okay because it's a utility that runs once, not a server application that runs 
    //   continuously. If it were the latter, we'd use .NET DI and let it handle the HttpClient lifetime.
    let private _httpClient = initHttpClient()

    /// Add an Accept header to the request message with the given mediaType.
    let private addAcceptHeader (mediaType : string) (requestMsg : HttpRequestMessage) =
        requestMsg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType))
        requestMsg

    /// Add an authorization header with a bearer token.
    let private addBearerTokenAuthorizationHeader (token : string) (requestMsg : HttpRequestMessage) =
        requestMsg.Headers.Authorization <- new AuthenticationHeaderValue("Bearer", token)
        requestMsg

    /// Serialize the value and add it to the request body as UTF-8 encoded JSON text.
    let private addJsonContent<'TValue> (value : 'TValue) (requestMsg : HttpRequestMessage) =
        requestMsg.Content <- new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8)
        requestMsg

    /// Get all labels from the GitHub repository.
    let private getExistingLabelsAsync (owner : string) (repo : string) (token : string) =
        task {
            use requestMsg = 
                new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/repos/{owner}/{repo}/labels?per_page=100")
                |> addAcceptHeader "application/vnd.github+json"
                |> addBearerTokenAuthorizationHeader token

            use! responseMsg = _httpClient.SendAsync(requestMsg)
            let! responseBody = responseMsg.Content.ReadAsStringAsync()
            return JsonSerializer.Deserialize<ExistingLabel[]>(responseBody)
        }

    let private createLabelAsync (owner : string) (repo : string) (token : string) (newLabel : NewLabel) =
        task {
            use requestMsg = 
                new HttpRequestMessage(HttpMethod.Post, $"https://api.github.com/repos/{owner}/{repo}/labels")
                |> addBearerTokenAuthorizationHeader token
                |> addJsonContent newLabel

            use! responseMsg = _httpClient.SendAsync(requestMsg)
            return! responseMsg.Content.ReadAsStringAsync()
        }


    /// Create the priority-1 through priority-5 labels in the repo.
    let createPriorityLabelsAsync (owner : string) (repo : string) (token : string) =
        task {
            ArgumentException.ThrowIfNullOrWhiteSpace(owner, nameof owner)
            ArgumentException.ThrowIfNullOrWhiteSpace(repo, nameof repo)
            ArgumentException.ThrowIfNullOrWhiteSpace(token, nameof token)

            
            //
            // Get the existing labels from the repo.
            //

            let! existingLabels = getExistingLabelsAsync owner repo token

            // Get a case-insensitive, distinct list of label names.
            let existingLabelNames = 
                existingLabels
                    .Select(_.Name)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase)


            //
            // Loop through the labels to create, and, if the name doesn't already exist, create the 
            //   label in GitHub.
            //

            for newLabel in Data.NewLabels.Where(fun pl -> not(existingLabelNames.Contains(pl.Name))) do

                printfn $"Creating label with name {newLabel.Name}, color {newLabel.Color}, and description {newLabel.Description}..."
                let! responseText = createLabelAsync owner repo token newLabel
                printfn $"Done. Response: {responseText}"
        }

