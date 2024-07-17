using LMS.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LMS.Models.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseRequest> CourseRequests { get; set; }
        public DbSet<CourseStudent> CourseStudents { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<StudentQuiz> StudentQuizzes { get; set; }
        public DbSet<Video> Videos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            #region Fluent Api

            // ApplicationUser
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.CourseRequests)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade); // User deletion cascades to CourseRequests

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.CourseStudents)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict); // User deletion restricted if in CourseStudent

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.StudentQuizzes)
                .WithOne(e => e.Student)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict); // User deletion restricted if in StudentQuizzes

            // Course
            modelBuilder.Entity<Course>()
                .HasMany(e => e.CourseStudents)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade); // Course deletion cascades to CourseStudents

            modelBuilder.Entity<Course>()
                .HasMany(e => e.CourseRequests)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade); // Course deletion cascades to CourseRequests

            modelBuilder.Entity<Course>()
                .HasMany(e => e.StudentQuizzes)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade); // Course deletion cascades to StudentQuizzes

            modelBuilder.Entity<Course>()
                .HasMany(e => e.Materials)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade); // Course deletion cascades to Materials

            modelBuilder.Entity<Course>()
                .HasMany(e => e.Videos)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade); // Course deletion cascades to Videos

            // CourseRequest
            modelBuilder.Entity<CourseRequest>()
                .HasOne(e => e.User)
                .WithMany(e => e.CourseRequests)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction); // CourseRequest deletion doesn't affect User

            modelBuilder.Entity<CourseRequest>()
                .HasOne(e => e.Course)
                .WithMany(e => e.CourseRequests)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.NoAction); // CourseRequest deletion doesn't affect Course

            // CourseStudent
            modelBuilder.Entity<CourseStudent>()
                .HasOne(e => e.User)
                .WithMany(e => e.CourseStudents)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade); // CourseStudent deletion restricted if user exists

            modelBuilder.Entity<CourseStudent>()
                .HasOne(e => e.Course)
                .WithMany(e => e.CourseStudents)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade); // Course deletion cascades to CourseStudents

            // Material
            modelBuilder.Entity<Material>()
                .HasOne(e => e.Course)
                .WithMany(e => e.Materials)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.NoAction); // Material deletion doesn't affect Course

            // Option
            modelBuilder.Entity<Option>()
                .HasOne(e => e.Question)
                .WithMany(e => e.Options)
                .HasForeignKey(e => e.QuestionId)
                .OnDelete(DeleteBehavior.Restrict); // Option deletion restricted if related to Question

            // Question
            modelBuilder.Entity<Question>()
                .HasOne(e => e.Quiz)
                .WithMany(e => e.Questions)
                .HasForeignKey(e => e.QuizId)
                .OnDelete(DeleteBehavior.Restrict); // Question deletion restricted if related to Quiz
            modelBuilder.Entity<Question>()
                .HasMany(e => e.Options)
                .WithOne(e => e.Question)
                .HasForeignKey(e => e.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quiz
            modelBuilder.Entity<Quiz>()
                .HasMany(e => e.Questions)
                .WithOne(e => e.Quiz)
                .HasForeignKey(e => e.QuizId)
                .OnDelete(DeleteBehavior.Cascade); // Quiz deletion cascades to Questions

            modelBuilder.Entity<Quiz>()
                .HasMany(e => e.StudentQuizzes)
                .WithOne(e => e.Quiz)
                .HasForeignKey(e => e.QuizId)
                .OnDelete(DeleteBehavior.Cascade); // Quiz deletion cascades to StudentQuizzes

            // StudentQuiz
            modelBuilder.Entity<StudentQuiz>()
                .HasOne(e => e.Student)
                .WithMany(e => e.StudentQuizzes)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict); // StudentQuiz deletion restricted if user exists

            modelBuilder.Entity<StudentQuiz>()
                .HasOne(e => e.Quiz)
                .WithMany(e => e.StudentQuizzes)
                .HasForeignKey(e => e.QuizId)
                .OnDelete(DeleteBehavior.Cascade); // Quiz deletion cascades to StudentQuizzes

            modelBuilder.Entity<StudentQuiz>()
                .HasOne(e => e.Course)
                .WithMany(e => e.StudentQuizzes)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.NoAction); // Course deletion cascades to StudentQuizzes

            // Video
            modelBuilder.Entity<Video>()
                .HasOne(e => e.Course)
                .WithMany(e => e.Videos)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.NoAction); // Video deletion doesn't affect Course
            #endregion

        }
    }
}


