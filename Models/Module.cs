using System.ComponentModel.DataAnnotations;

namespace TeachingAI1.Models
{
    public class Module
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int LessonCount { get; set; }
        public int CompletedLessons { get; set; }
        public string Status { get; set; } = "Not Started";

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public List<Lesson> Lessons { get; set; } = new();
    }
}