using System;
using System.Collections.Generic;
namespace TeachingAI1.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TeacherId { get; set; }
        public string Status { get; set; } = "Active";
        public string? CoverImageUrl { get; set; }
        public string ImageUrl { get; set; } = "";
        public int LessonsTotal { get; set; }
        
        public DateTime? StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; }
        public string Syllabus { get; set; } = string.Empty;
        public int LessonCount { get; set; }  
        public int CompletedLessons { get; set; }

        // Navigation
        public Teacher? Teacher { get; set; }
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

        // QuizOptional
        public string Description { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string VideoPath { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        // Computed properties
        public string Title { get; set; } = string.Empty;
        public string Instructor { get; set; }
        public int Progress { get; set; } = 0;
        public int LessonsCompleted { get; set; } = 0;
        public string TimeLeft { get; set; } = "";
        public string Duration { get; set; } = "";
        public List<Module> Modules { get; set; } = new();
    }
}