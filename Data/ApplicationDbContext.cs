using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TeachingAI1.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeachingAI1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> QuizOptions)
            : base(QuizOptions)
        {
            EnsureTeacherAccountExists();
        }
        public DbSet<User>? Users { get; set; }
        public DbSet<Progress>? Progresses { get; set; }
        public DbSet<Feedback>? Feedbacks { get; set; }
        public DbSet<Lesson>? Lessons { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Course>? Courses { get; set; }
        public DbSet<Quiz>? Quizzes { get; set; }
        public DbSet<AIInteraction>? AIInteractions { get; set; }
        public DbSet<Teacher>? Teachers { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("Users");

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Teacher)
                .WithMany(t => t.Courses)
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Module)
                .WithMany(m => m.Lessons)
                .HasForeignKey(l => l.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

        }

        private void EnsureTeacherAccountExists()
        {
            try
            {
                if (this.Database.GetPendingMigrations().Any())
                    return;

                if (Users != null && !Users.Any(u => u.Role == "Teacher"))
                {
                    var teacherUser = new User
                    {
                        Name = "Test Teacher",
                        Email = "teacher@example.com",
                        PasswordHash = "5f4dcc3b5aa765d61d8327deb882cf99", // "password" 的 MD5
                        Role = "Teacher",
                        CreatedAt = DateTime.Now
                    };

                    Users.Add(teacherUser);
                    SaveChanges();

                    if (Teachers != null && !Teachers.Any())
                    {
                        var teacher = new Teacher
                        {
                            UserId = teacherUser.Id
                        };
                        Teachers.Add(teacher);
                        SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error seeding teacher account: {ex.Message}");
            }
        }
    }
}