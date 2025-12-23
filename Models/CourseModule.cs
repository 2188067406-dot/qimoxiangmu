namespace TeachingAI1.Models
{
    public class CourseModule
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int LessonCount { get; set; }        // 模块内的课时数
        public int CompletedLessons { get; set; }   // 学生已完成的课时数（可选）
        public string Status { get; set; }          // 如 "Completed", "InProgress"
        public string VideoUrl { get; set; }        // 👈 必须加上这一行！
        public List<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}