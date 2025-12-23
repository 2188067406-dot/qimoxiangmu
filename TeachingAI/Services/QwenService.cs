// Services/QwenService.cs
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace TeachingAI1.Services;

public class QwenApiSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
}

public class QwenService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public QwenService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> GetCompletionAsync(string prompt)
    {
        var apiKey = _configuration["QwenApiSettings:ApiKey"];
        var endpoint = _configuration["QwenApiSettings:Endpoint"];

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(endpoint))
            throw new InvalidOperationException("Qwen API 配置缺失，请检查 appsettings.json");

        var requestBody = new
        {
            model = "qwen-turbo",
            input = new
            {
                messages = new[]
                {
                    new { role = "user", content = prompt.Trim() }
                }
            },
            parameters = new
            {
                max_tokens = 500,
                temperature = 0.7
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        _httpClient.DefaultRequestHeaders.Add("X-DashScope-Async", "disable");

        var response = await _httpClient.PostAsync(endpoint, content);
        var responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Qwen API 调用失败 ({response.StatusCode}): {responseString}");
        }

        using var doc = JsonDocument.Parse(responseString);
        var text = doc.RootElement
            .GetProperty("output")
            .GetProperty("text")
            .GetString();

        return text ?? "AI 暂时无法回答。";
    }
}