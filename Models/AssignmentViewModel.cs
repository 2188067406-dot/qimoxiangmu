using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeachingAI1.Models
{
    public class AssignmentViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Assignment title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "Course is required")]
        public int CourseId { get; set; }
        
        public string CourseTitle { get; set; }
        
        [Required(ErrorMessage = "Due date is required")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }
        
        public int AssignedStudents { get; set; }
        
        public int SubmittedCount { get; set; }
        
        public string Status { get; set; }
        
        [StringLength(1000, ErrorMessage = "Description cannot be longer than 1000 characters")]
        public string Description { get; set; }
        
        [StringLength(1000, ErrorMessage = "Instructions cannot be longer than 1000 characters")]
        public string Instructions { get; set; }
        
        [Range(1, 1000, ErrorMessage = "Points must be between 1 and 1000")]
        [Display(Name = "Points Possible")]
        public int PointsPossible { get; set; }
        
        // Navigation properties
        public List<SubmissionViewModel> Submissions { get; set; }
    }
    
    public class SubmissionViewModel
    {
        public int Id { get; set; }
        
        public string StudentName { get; set; }
        
        public DateTime? SubmissionDate { get; set; }
        
        public string Status { get; set; }
        
        public int? Grade { get; set; }
        
        public string Feedback { get; set; }
    }
} 