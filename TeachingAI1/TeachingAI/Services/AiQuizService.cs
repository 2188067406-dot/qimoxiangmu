using System.Text;
using System.Text.Json;
using TeachingAI1.Models;

namespace TeachingAI1.Services
{
    public class AiQuizService : IAiQuizService
    {
        private readonly HttpClient _httpClient;
        private readonly DashScopeQuizOptions _quizOptions;

        public AiQuizService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _quizOptions = new DashScopeQuizOptions
            {
                Model = configuration["DashScope:Model"] ?? "qwen-turbo",
                ApiKey = configuration["DashScope:ApiKey"]
            };
        }

        // ✅ 新增：实现接口要求的通用对话方法
        public async Task<string> GetCompletionAsync(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                return "问题不能为空。";

            try
            {
                // 复用 Qwen 调用逻辑（提取为私有方法）
                var responseText = await CallQwenApiAsync(prompt);
                return responseText;
            }
            catch (Exception ex)
            {
                return $"AI 回答失败：{ex.Message}";
            }
        }

        public async Task<List<QuizQuestion>> GenerateQuizQuestionsAsync(string context, int count = 3)
        {
            var prompt = $@"
                你是一位资深教育专家，请根据以下课程内容生成{count}道高质量的单选题。
                要求：
                - 每道题有 4 个选项（A、B、C、D 开头）；
                - 提供正确答案的索引（0 表示 A，1 表示 B，以此类推）；
                - 提供简明解析；
                - 以严格的 JSON 数组格式输出，不要任何额外文字。

                课程内容：
                {context}

                输出格式示例：
                [
                {{
                    ""question"": ""什么是机器学习？"",
                    ""options"": [
                    ""A. 让计算机自动编写代码"",
                    ""B. 让计算机从数据中学习规律"",
                    ""C. 让计算机拥有情感"",
                    ""D. 让计算机运行更快""
                    ],
                    ""answerIndex"": 1,
                    ""explanation"": ""机器学习的核心是从数据中自动发现模式和规律。""
                }}
                ]
                ";

            var responseText = await CallQwenApiAsync(prompt);

            // 清理可能的 Markdown 包裹
            var cleanText = responseText.Trim();
            if (cleanText.StartsWith("```json")) cleanText = cleanText["```json".Length..];
            if (cleanText.EndsWith("```")) cleanText = cleanText[..^"```".Length];
            cleanText = cleanText.Trim();

            try
            {
                var quizQuestions = JsonSerializer.Deserialize<List<QuizQuestion>>(
                    cleanText,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true, AllowTrailingCommas = true }
                );

                if (quizQuestions == null) throw new Exception("No quiz questions returned.");

                foreach (var q in quizQuestions)
                {
                    if (q.Options?.Count != 4)
                        throw new Exception("Each question must have exactly 4 options.");
                    if (q.AnswerIndex < 0 || q.AnswerIndex > 3)
                        throw new Exception("AnswerIndex must be between 0 and 3.");
                }

                return quizQuestions.Take(count).ToList();
            }
            catch (JsonException ex)
            {
                throw new Exception($"Failed to parse AI response: {ex.Message}. Raw: {cleanText}");
            }
        }

        // ✅ 新增：私有方法，封装 Qwen API 调用
        private async Task<string> CallQwenApiAsync(string userMessage)
        {
            var requestBody = new
            {
                model = _quizOptions.Model,
                input = new { messages = new[] { new { role = "user", content = userMessage } } },
                parameters = new { temperature = 0.7 }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_quizOptions.ApiKey}");

            var response = await _httpClient.PostAsync(
                "https://dashscope.aliyuncs.com/api/v1/services/aigc/text-generation/generation",
                content
            );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"DashScope API error: {response.StatusCode}, {error}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseBody);
            return doc.RootElement
                .GetProperty("output")
                .GetProperty("text")
                .GetString() ?? "";
        }
    }

    public class DashScopeQuizOptions
    {
        public string Model { get; set; } = "qwen-turbo";
        public string ApiKey { get; set; } = string.Empty;
        public const string SectionName = "DashScope";
    }
}