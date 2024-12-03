using Git_To_OpenAI.Api;
using Git_To_OpenAI.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/github/projects", async (HttpClient client, [FromQuery] string accessToken) =>
{
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

    var response = await client.GetAsync("https://api.github.com/user/repos");
    if (!response.IsSuccessStatusCode)
    {
        return Results.BadRequest("Failed to retrieve repositories. Check your credentials or token.");
    }

    var repositories = await response.Content.ReadFromJsonAsync<List<RepositoryModel>>();
    return Results.Ok(repositories.Select(x => new { x.Name, x.Owner.Login }));
})
.WithOpenApi();


app.MapGet("/github/project/{repoName}/owner/{owner}/files", async (HttpClient client, string repoName, string owner, string extension, string authHeader, [FromServices] IHttpClientFactory httpClientFactory) =>
{
    // Authenticate to GitHub API
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authHeader);
    client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

    // Get the file tree from the repository
    var response = await client.GetAsync($"https://api.github.com/repos/{owner}/{repoName}/git/trees/master?recursive=1");
    if (!response.IsSuccessStatusCode)
    {
        return Results.BadRequest("Failed to retrieve the file tree from the repository.");
    }

    var gitTree = await response.Content.ReadFromJsonAsync<GitTreeModel>();
    var filteredFiles = gitTree.Tree.Where(file => file.Path.EndsWith(extension)).ToList();

    if (!filteredFiles.Any())
    {
        return Results.Ok("No files with the specified extension found.");
    }

    var openAiResults = new List<object>();

    // Use Task.WhenAll to process files concurrently
    var tasks = filteredFiles.Select(async file =>
    {
        // Get file content from GitHub
        var fileContentResponse = await client.GetAsync($"https://api.github.com/repos/{owner}/{repoName}/contents/{file.Path}");
        if (!fileContentResponse.IsSuccessStatusCode)
        {
            return null; // Skip files with errors
        }

        var fileContentModel = await fileContentResponse.Content.ReadFromJsonAsync<FileContentModel>();
        if (fileContentModel == null || string.IsNullOrEmpty(fileContentModel.Content))
        {
            return null; // Skip if file has no content
        }

        // Decode Base64 content from GitHub
        var fileContent = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(fileContentModel.Content));

        // Send file content to OpenAI for improvement suggestions using chat model
        var openAiClient = httpClientFactory.CreateClient();
        openAiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "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");

        var chatRequest = new
        {
            model = "gpt-4o",
            messages = new[]
            {
            new { role = "system", content = "You are a helpful assistant that provides code review suggestions." },
            new { role = "user", content = $"Analyze the following code and suggest improvements but only summary:\n\n{fileContent}" }
        },
            max_tokens = 1024,
            temperature = 0.7
        };

        var openAiResponse = await openAiClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", chatRequest);

        if (!openAiResponse.IsSuccessStatusCode)
        {
            // Read the response content for more details
            var errorDetails = await openAiResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"OpenAI API request failed: {openAiResponse.StatusCode} - {errorDetails}");
            return null; // Skip files that OpenAI fails to analyze
        }

        var openAiResult = await openAiResponse.Content.ReadFromJsonAsync<OpenAiChatCompletionModel>();
        if (openAiResult == null || !openAiResult.Choices.Any())
        {
            return null; // Skip empty results
        }

        // Return the result for this file
        return new
        {
            file.Path,
            Suggestions = openAiResult.Choices.FirstOrDefault()?.Message.Content.Trim(),
        };
    }).ToList();

    // Wait for all tasks to complete
    var results = await Task.WhenAll(tasks);

    // Filter out null results and add them to the final result list
    openAiResults.AddRange(results.Where(result => result != null));

    return Results.Ok(openAiResults);
})
.WithOpenApi();




app.Run();

