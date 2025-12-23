using TeachingAI1.Models;
using System.Collections.Generic;

namespace TeachingAI1.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        
        public int? UserId { get; set; }

        public string Name { get; set; } = string.Empty;
        
        // Navigation property
        public User? User { get; set; }
        
        // Collection of courses taught by this teacher
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    } 
}