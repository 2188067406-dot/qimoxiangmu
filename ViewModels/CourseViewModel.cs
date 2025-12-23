using System;
using TeachingAI1.Models;
using System.Collections.Generic;
using TeachingAI1.ViewModels; // 确保能引用 ModuleViewModel

namespace TeachingAI1.ViewModels;

public class CourseViewModel
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string CoverImageUrl { get; set; } = string.Empty;
    public double ProgressPercentage { get; set; }
    public int CompletedLessons { get; set; }
    public int TotalLessons { get; set; }
    public double? EstimatedTimeLeft { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public int StudentCount { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Grade { get; set; }
    public string ImageUrl { get; set; } = string.Empty;

    // 兼容旧视图用的 CourseName（别删！）
    public string CourseName 
    { 
        get => Title; 
        set => Title = value ?? string.Empty; 
    }

    public string Description { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Duration { get; set; } = string.Empty;
    public string Syllabus { get; set; } = string.Empty;
    
    public string Instructor { get; set; } = "Unknown";      
    public int Progress { get; set; }                        
    public int LessonsCompleted { get; set; }                
    public int LessonsTotal { get; set; }                    
    public string TimeLeft { get; set; } = "N/A";            
    public string Status { get; set; } = "Active";   
    public string? Content { get; set; }    
    public List<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>();  

    public string VideoPath { get; set; } = string.Empty;

    // 统计字段
    public int EnrolledStudents { get; set; }
    public double Attendance { get; set; }      // 出勤率 %
    public double Completion { get; set; }     // 完成率 %
    public double Rating { get; set; }         // 评分

    // 创建课程用
    public bool PublishImmediately { get; set; }

    // ✅ 唯一 Modules 列表，使用 ModuleViewModel（需确保该类存在）
    public List<ModuleViewModel> Modules { get; set; } = new();
}