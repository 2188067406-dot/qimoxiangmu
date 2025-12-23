using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeachingAI1.ViewModels
{
    public class StudentViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        
        public int EnrolledCourses { get; set; }
        
        public int OverallProgress { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "Join Date")]
        public DateTime JoinDate { get; set; }
        
        // Navigation property
        public List<CourseViewModel> Courses { get; set; }
    }
} 