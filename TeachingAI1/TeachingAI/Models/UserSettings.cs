using System.ComponentModel.DataAnnotations;

namespace TeachingAI1.Models
{
    public class UserSettings
    {
        // Account Settings
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string StudentId { get; set; }
        
        // Password Change
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string NewPassword { get; set; }
        
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        
        // Preferences
        public string Language { get; set; } = "en";
        public string TimeZone { get; set; } = "utc-8";
        public string DateFormat { get; set; } = "mdy";
        public string TimeFormat { get; set; } = "12";
        
        // Notifications
        public bool EmailAssignments { get; set; } = true;
        public bool EmailGrades { get; set; } = true;
        public bool EmailAnnouncements { get; set; } = true;
        public bool EmailMessages { get; set; } = false;
        public bool EmailNewCourses { get; set; } = false;
        
        public bool PushAssignments { get; set; } = true;
        public bool PushGrades { get; set; } = true;
        public bool PushMessages { get; set; } = true;
        
        public string NotificationFrequency { get; set; } = "Daily Digest";
        
        // Privacy
        public string ProfileVisibility { get; set; } = "Everyone (Public)";
        public bool ShowActiveStatus { get; set; } = true;
        public bool DataCollection { get; set; } = true;
        
        // Appearance
        public string Theme { get; set; } = "Light";
        public string FontSize { get; set; } = "Medium";
        public string DashboardLayout { get; set; } = "Grid View";
        
        // Accessibility
        public bool ScreenReader { get; set; } = false;
        public bool ReduceMotion { get; set; } = false;
        public bool KeyboardNavigation { get; set; } = true;
        
        // Connected Accounts
        public bool GoogleConnected { get; set; } = true;
        public string GoogleEmail { get; set; } = "john.doe@gmail.com";
        
        public bool MicrosoftConnected { get; set; } = false;
        public string MicrosoftEmail { get; set; }
        
        public bool GitHubConnected { get; set; } = true;
        public string GitHubUsername { get; set; } = "johndoe";
        
        public bool LinkedInConnected { get; set; } = false;
        public string LinkedInUsername { get; set; }
    }
} 