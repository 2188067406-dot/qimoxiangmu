using TeachingAI1.Models;
using System;
using System.Collections.Generic;

namespace TeachingAI1.ViewModels
{
    public class CourseDetailsViewModel
    {
        public Course Course { get; set; } 
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Instructor { get; set; } = "Unknown";
        public int Progress { get; set; }
        public int LessonsCompleted { get; set; }
        public int LessonsTotal { get; set; }
        public string TimeLeft { get; set; } = "N/A";
        public string Status { get; set; } = "Not Started";
        public string VideoPath { get; set; } = string.Empty;
        public string Duration { get; set; } = "";
        public List<CourseModuleViewModel> Modules { get; set; } = new();
        public List<QuizQuestion>? Questions { get; set; }
    }
     public class CourseModuleViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }
        public int LessonCount { get; set; }
        public int CompletedLessons { get; set; }
        public string Status { get; set; }

        // ✅ 这里必须有 Lessons！
        public List<Lesson> Lessons { get; set; } = new();
    }

    public class CourseLessonViewModel
    {
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
    }
}