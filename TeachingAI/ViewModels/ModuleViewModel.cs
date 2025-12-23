// ViewModels/ModuleViewModel.cs
namespace TeachingAI1.ViewModels
{
    public class ModuleViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int LessonCount { get; set; }
        public int CompletedLessons { get; set; }
        public string Status { get; set; } = "Not Started";

        public string VideoUrl { get; set; } = "";
    }
}