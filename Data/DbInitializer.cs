using BCrypt.Net;
using SmartStudyPlanner.Models;

namespace SmartStudyPlanner.Data;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Ensure database is created
        context.Database.EnsureCreated();

        // Check if any users exist
        if (context.Users.Any())
        {
            return; // DB has already been seeded
        }

        // 1. Seed Demo Student User
        var student = new User
        {
            FullName = "Alex Sharma",
            Email = "student@study.edu",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            IsActive = true
        };
        context.Users.Add(student);
        context.SaveChanges();

        // 2. Seed Subjects
        var subDotnet = new Subject { UserId = student.UserId, SubjectName = ".NET Core & C# Programming", IsActive = true };
        var subDsa = new Subject { UserId = student.UserId, SubjectName = "Data Structures & Algorithms", IsActive = true };
        var subDbms = new Subject { UserId = student.UserId, SubjectName = "Database Management Systems", IsActive = true };
        var subNetworks = new Subject { UserId = student.UserId, SubjectName = "Computer Networks", IsActive = true };
        var subSe = new Subject { UserId = student.UserId, SubjectName = "Software Engineering", IsActive = true };

        context.Subjects.AddRange(subDotnet, subDsa, subDbms, subNetworks, subSe);
        context.SaveChanges();

        // 3. Seed Tasks
        var task1 = new StudyTask
        {
            SubjectId = subDotnet.SubjectId,
            Title = "Build Smart Study Planner ASP.NET Core Project",
            Description = "Implement MVC, EF Core, Authentication, and Dashboard charts.",
            TaskType = "Project",
            Deadline = DateTime.Today.AddDays(3),
            Priority = "Urgent",
            Difficulty = "Hard",
            Status = "In Progress",
            ProgressPercentage = 75,
            EstimatedHours = 12.0m,
            ActualHours = 9.0m,
            CreatedAt = DateTime.Today.AddDays(-4)
        };

        var task2 = new StudyTask
        {
            SubjectId = subDsa.SubjectId,
            Title = "Binary Search Tree Implementation in C#",
            Description = "Code Insert, Delete, In-order, Pre-order and Post-order traversals.",
            TaskType = "Assignment",
            Deadline = DateTime.Today.AddDays(6),
            Priority = "High",
            Difficulty = "Medium",
            Status = "Pending",
            ProgressPercentage = 30,
            EstimatedHours = 5.0m,
            ActualHours = 1.5m,
            CreatedAt = DateTime.Today.AddDays(-2)
        };

        var task3 = new StudyTask
        {
            SubjectId = subDbms.SubjectId,
            Title = "SQL Complex Joins & Subqueries Lab",
            Description = "Complete lab sheet 4 on multi-table JOINs, GROUP BY, and HAVING clauses.",
            TaskType = "Practice",
            Deadline = DateTime.Today.AddDays(-1),
            Priority = "Medium",
            Difficulty = "Medium",
            Status = "Completed",
            ProgressPercentage = 100,
            EstimatedHours = 3.0m,
            ActualHours = 2.5m,
            CompletedAt = DateTime.Today.AddDays(-1),
            CreatedAt = DateTime.Today.AddDays(-5)
        };

        var task4 = new StudyTask
        {
            SubjectId = subNetworks.SubjectId,
            Title = "TCP vs UDP Flow Control Research Paper",
            Description = "Write a 3-page comparative study on reliability and congestion control mechanisms.",
            TaskType = "Reading",
            Deadline = DateTime.Today.AddDays(10),
            Priority = "Low",
            Difficulty = "Easy",
            Status = "Pending",
            ProgressPercentage = 0,
            EstimatedHours = 4.0m,
            ActualHours = 0.0m,
            CreatedAt = DateTime.Today.AddDays(-1)
        };

        context.Tasks.AddRange(task1, task2, task3, task4);
        context.SaveChanges();

        // 4. Seed Exams
        var exam1 = new Exam
        {
            SubjectId = subDotnet.SubjectId,
            ExamName = "Dotnet & C# End Semester Practical Exam",
            ExamDate = DateTime.Today.AddDays(12),
            ExamTime = "09:30 AM - 12:30 PM",
            TotalMarks = 100,
            PassingMarks = 40,
            Syllabus = "C# OOP Fundamentals, ASP.NET Core MVC Architecture, Entity Framework Core, LINQ, Dependency Injection, Razor Views, Authentication."
        };

        var exam2 = new Exam
        {
            SubjectId = subDsa.SubjectId,
            ExamName = "Data Structures Mid-Term Exam",
            ExamDate = DateTime.Today.AddDays(18),
            ExamTime = "02:00 PM - 04:00 PM",
            TotalMarks = 50,
            PassingMarks = 20,
            Syllabus = "Arrays, Linked Lists, Stacks, Queues, Binary Trees, Quick Sort, Merge Sort, Time Complexity."
        };

        var exam3 = new Exam
        {
            SubjectId = subDbms.SubjectId,
            ExamName = "DBMS Theory Final Exam",
            ExamDate = DateTime.Today.AddDays(25),
            ExamTime = "10:00 AM - 01:00 PM",
            TotalMarks = 100,
            PassingMarks = 40,
            Syllabus = "Relational Model, Normalization (1NF, 2NF, 3NF, BCNF), SQL DDL/DML, Transactions, ACID Properties, Concurrency Control."
        };

        context.Exams.AddRange(exam1, exam2, exam3);
        context.SaveChanges();

        // 5. Seed Study Sessions
        var session1 = new StudySession
        {
            UserId = student.UserId,
            SubjectId = subDotnet.SubjectId,
            TaskId = task1.TaskId,
            Topic = "Setting up Entity Framework Core and DbContext",
            StudyDate = DateTime.Today.AddDays(-2),
            StartTime = "15:00",
            EndTime = "17:00",
            DurationMinutes = 120,
            Status = "Completed",
            Notes = "Successfully configured SQLite and models. Tested database migrations.",
            CreatedAt = DateTime.Today.AddDays(-2)
        };

        var session2 = new StudySession
        {
            UserId = student.UserId,
            SubjectId = subDotnet.SubjectId,
            TaskId = task1.TaskId,
            Topic = "Building Controllers and Razor Views",
            StudyDate = DateTime.Today.AddDays(-1),
            StartTime = "18:30",
            EndTime = "20:00",
            DurationMinutes = 90,
            Status = "Completed",
            Notes = "Created CRUD views with Bootstrap 5 cards and validation.",
            CreatedAt = DateTime.Today.AddDays(-1)
        };

        var session3 = new StudySession
        {
            UserId = student.UserId,
            SubjectId = subDsa.SubjectId,
            TaskId = task2.TaskId,
            Topic = "Binary Search Tree Node Deletion cases",
            StudyDate = DateTime.Today,
            StartTime = "14:00",
            EndTime = "15:30",
            DurationMinutes = 90,
            Status = "Completed",
            Notes = "Understood in-order successor logic for node with two children.",
            CreatedAt = DateTime.Today
        };

        var session4 = new StudySession
        {
            UserId = student.UserId,
            SubjectId = subDotnet.SubjectId,
            TaskId = task1.TaskId,
            Topic = "Dashboard Analytics and Charts",
            StudyDate = DateTime.Today.AddDays(1),
            StartTime = "16:00",
            EndTime = "18:00",
            DurationMinutes = 120,
            Status = "Planned",
            Notes = "Integrate Chart.js for daily study trends and task distribution.",
            CreatedAt = DateTime.Today
        };

        context.StudySessions.AddRange(session1, session2, session3, session4);
        context.SaveChanges();

        // 6. Seed Revision Plans
        var rev1 = new RevisionPlan
        {
            UserId = student.UserId,
            SubjectId = subDotnet.SubjectId,
            ExamId = exam1.ExamId,
            Topic = "LINQ Queries & Entity Framework Core Performance",
            RevisionDate = DateTime.Today.AddDays(4),
            StartTime = "10:00",
            DurationMinutes = 60,
            Status = "Scheduled",
            Notes = "Practice Include, ThenInclude, AsNoTracking, and grouping queries.",
            CreatedAt = DateTime.Today
        };

        var rev2 = new RevisionPlan
        {
            UserId = student.UserId,
            SubjectId = subDotnet.SubjectId,
            ExamId = exam1.ExamId,
            Topic = "ASP.NET Core Middleware & Authentication Pipeline",
            RevisionDate = DateTime.Today.AddDays(8),
            StartTime = "11:00",
            DurationMinutes = 75,
            Status = "Scheduled",
            Notes = "Review cookie authentication flow and custom filter execution order.",
            CreatedAt = DateTime.Today
        };

        var rev3 = new RevisionPlan
        {
            UserId = student.UserId,
            SubjectId = subDsa.SubjectId,
            ExamId = exam2.ExamId,
            Topic = "QuickSort and MergeSort recursion trees",
            RevisionDate = DateTime.Today.AddDays(-1),
            StartTime = "16:00",
            DurationMinutes = 45,
            Status = "Completed",
            Notes = "Derived Master Theorem cases and recursive depth proofs.",
            CreatedAt = DateTime.Today.AddDays(-2)
        };

        context.RevisionPlans.AddRange(rev1, rev2, rev3);
        context.SaveChanges();

        // 7. Seed Daily Goals
        var goalYesterday = new DailyGoal
        {
            UserId = student.UserId,
            GoalDate = DateTime.Today.AddDays(-1),
            TargetStudyMinutes = 120,
            CompletedStudyMinutes = 150,
            TargetTasks = 2,
            CompletedTasks = 2,
            Status = "Achieved",
            Notes = "Exceeded study target! Finished SQL lab."
        };

        var goalToday = new DailyGoal
        {
            UserId = student.UserId,
            GoalDate = DateTime.Today,
            TargetStudyMinutes = 150,
            CompletedStudyMinutes = 90,
            TargetTasks = 3,
            CompletedTasks = 1,
            Status = "In Progress",
            Notes = "Focus on Dotnet project and DSA BST assignment."
        };

        context.DailyGoals.AddRange(goalYesterday, goalToday);
        context.SaveChanges();

        // 8. Seed Notifications
        var notif1 = new Notification
        {
            UserId = student.UserId,
            Title = "Upcoming Exam Alert!",
            Message = "Your '.NET Core & C# Programming' exam is scheduled in 12 days.",
            NotificationType = "ExamCountdown",
            NotificationDate = DateTime.Today,
            IsRead = false
        };

        var notif2 = new Notification
        {
            UserId = student.UserId,
            Title = "Assignment Deadline Approaching",
            Message = "Project 'Build Smart Study Planner' is due in 3 days. Current progress: 75%.",
            NotificationType = "DeadlineReminder",
            NotificationDate = DateTime.Today.AddHours(-3),
            IsRead = false
        };

        var notif3 = new Notification
        {
            UserId = student.UserId,
            Title = "Daily Goal Milestone",
            Message = "Great job! You achieved your study goal yesterday with 150 mins logged.",
            NotificationType = "DailyGoal",
            NotificationDate = DateTime.Today.AddDays(-1),
            IsRead = true
        };

        context.Notifications.AddRange(notif1, notif2, notif3);
        context.SaveChanges();
    }
}

