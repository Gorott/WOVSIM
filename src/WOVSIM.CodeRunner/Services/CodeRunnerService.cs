using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WOVSIM.CodeRunner.Interfaces;
using WOVSIM.CodeRunner.Models;

namespace WOVSIM.CodeRunner.Services;

public class CodeRunnerService : ICodeRunnerService
{
    private readonly HttpClient _httpClient;

    private readonly Dictionary<ProgrammingLanguage, int> _judge0Languages = new()
    {
        { ProgrammingLanguage.Python, 71 },      // Python 3.11
        { ProgrammingLanguage.JavaScript, 63 },  // Node.js 18
        { ProgrammingLanguage.CSharp, 51 }       // .NET 6 (C#)
    };

    public CodeRunnerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://localhost:2358/");
        _httpClient.Timeout = TimeSpan.FromSeconds(15); // prevent hanging
    }

    public async Task<CodeRunResult> RunCodeAsync(ProgrammingLanguage language, string? code, string? input = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("No code provided.");

        if (!_judge0Languages.TryGetValue(language, out var langId))
            throw new NotSupportedException($"Language {language} is not supported.");

        // Prepare request body
        var requestBody = new
        {
            language_id = langId,
            source_code = Convert.ToBase64String(Encoding.UTF8.GetBytes(code)),
            stdin = input != null ? Convert.ToBase64String(Encoding.UTF8.GetBytes(input)) : null
        };

        // Create request to Judge0 (synchronous execution)
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://judge0-ce.p.rapidapi.com/submissions?base64_encoded=true&wait=true&fields=*"),
            Headers =
            {
                { "x-rapidapi-key", "ab5c02c362mshffb590f26067052p1cabe5jsnac184585bf5c" },
                { "x-rapidapi-host", "judge0-ce.p.rapidapi.com" },
            },
            Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        };

        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(responseBody).RootElement;
        
        string? DecodeBase64(string? base64)
        {
            if (string.IsNullOrEmpty(base64)) return null;
            try
            {
                var bytes = Convert.FromBase64String(base64);
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return base64;
            }
        }
        
        string? stdout = DecodeBase64(doc.GetPropertyOrDefault("stdout")?.GetString())?.Trim();
        string? stderr = DecodeBase64(doc.GetPropertyOrDefault("stderr")?.GetString())?.Trim();
        string? compileOutput = DecodeBase64(doc.GetPropertyOrDefault("compile_output")?.GetString())?.Trim();

        string status = doc.TryGetProperty("status", out var statusElem) &&
                        statusElem.TryGetProperty("description", out var descElem)
            ? descElem.GetString() ?? "Unknown"
            : "Unknown";

        return new CodeRunResult
        {
            Output = stdout,
            Error = string.Join("\n", new[] { compileOutput, stderr }.Where(s => !string.IsNullOrEmpty(s))),
            IsSuccess = status.Equals("Accepted", StringComparison.OrdinalIgnoreCase)
        };
    }
}

// Small helper to avoid crashes when JSON fields are missing
internal static class JsonExtensions
{
    public static JsonElement? GetPropertyOrDefault(this JsonElement element, string name)
    {
        return element.TryGetProperty(name, out var value) ? value : null;
    }
}
