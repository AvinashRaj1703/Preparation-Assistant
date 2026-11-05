using Microsoft.EntityFrameworkCore;
using SmartStudyPlanner.Models;

namespace SmartStudyPlanner.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<StudyTask> Tasks => Set<StudyTask>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<StudySession> StudySessions => Set<StudySession>();
    public DbSet<RevisionPlan> RevisionPlans => Set<RevisionPlan>();
    public DbSet<DailyGoal> DailyGoals => Set<DailyGoal>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Subject
        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Subjects)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Task
        modelBuilder.Entity<StudyTask>(entity =>
        {
            entity.HasKey(e => e.TaskId);
            entity.HasOne(e => e.Subject)
                  .WithMany(s => s.Tasks)
                  .HasForeignKey(e => e.SubjectId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Exam
        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.ExamId);
            entity.HasOne(e => e.Subject)
                  .WithMany(s => s.Exams)
                  .HasForeignKey(e => e.SubjectId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // StudySession
        modelBuilder.Entity<StudySession>(entity =>
        {
            entity.HasKey(e => e.StudySessionId);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.StudySessions)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Subject)
                  .WithMany(s => s.StudySessions)
                  .HasForeignKey(e => e.SubjectId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Task)
                  .WithMany(t => t.StudySessions)
                  .HasForeignKey(e => e.TaskId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // RevisionPlan
        modelBuilder.Entity<RevisionPlan>(entity =>
        {
            entity.HasKey(e => e.RevisionPlanId);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.RevisionPlans)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Subject)
                  .WithMany(s => s.RevisionPlans)
                  .HasForeignKey(e => e.SubjectId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Exam)
                  .WithMany(ex => ex.RevisionPlans)
                  .HasForeignKey(e => e.ExamId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // DailyGoal
        modelBuilder.Entity<DailyGoal>(entity =>
        {
            entity.HasKey(e => e.GoalId);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.DailyGoals)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Notification
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Notifications)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

