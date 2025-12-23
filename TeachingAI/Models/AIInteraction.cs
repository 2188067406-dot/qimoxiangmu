public class AIInteraction
{
    public int Id { get; set; }
    public int UserId { get; set; } // Student or teacher interacting with AI
    public User User { get; set; }
    public string Input { get; set; } // QuizQuestion or topic entered by the user
    public string AIResponse { get; set; }
    public DateTime Timestamp { get; set; }
}
