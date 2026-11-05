-- ========================================================
-- Smart Study Planner - Sample Seed Data
-- ========================================================

-- Insert Sample User (Password: Password123!)
-- Hash generated via BCrypt: $2a$11$9dY4F4Gj2XhZlS4c8uS7OeZ4G/45QhR23mUk7k9Y/FwU7xZpQYfWa
INSERT INTO Users (FullName, Email, PasswordHash, IsActive)
VALUES ('Alex Sharma', 'student@study.edu', '$2a$11$9dY4F4Gj2XhZlS4c8uS7OeZ4G/45QhR23mUk7k9Y/FwU7xZpQYfWa', 1);

-- Insert Subjects
INSERT INTO Subjects (UserId, SubjectName, IsActive) VALUES 
(1, '.NET Core & C# Programming', 1),
(1, 'Data Structures & Algorithms', 1),
(1, 'Database Management Systems', 1),
(1, 'Computer Networks', 1),
(1, 'Software Engineering', 1);

-- Insert Tasks
INSERT INTO Tasks (SubjectId, Title, Description, TaskType, Deadline, Priority, Difficulty, Status, ProgressPercentage, EstimatedHours, ActualHours, CreatedAt) VALUES
(1, 'Build Smart Study Planner ASP.NET Core Project', 'Implement MVC, EF Core, Authentication, and Dashboard charts.', 'Project', '2026-11-08', 'Urgent', 'Hard', 'In Progress', 75, 12.0, 9.0, '2026-11-01'),
(2, 'Binary Search Tree Implementation in C#', 'Code Insert, Delete, In-order, Pre-order and Post-order traversals.', 'Assignment', '2026-11-11', 'High', 'Medium', 'Pending', 30, 5.0, 1.5, '2026-11-03'),
(3, 'SQL Complex Joins & Subqueries Lab', 'Complete lab sheet 4 on multi-table JOINs, GROUP BY, and HAVING.', 'Practice', '2026-11-04', 'Medium', 'Medium', 'Completed', 100, 3.0, 2.5, '2026-10-31'),
(4, 'TCP vs UDP Flow Control Research Paper', 'Write a 3-page comparative study on reliability mechanisms.', 'Reading', '2026-11-15', 'Low', 'Easy', 'Pending', 0, 4.0, 0.0, '2026-11-04');

-- Insert Exams
INSERT INTO Exams (SubjectId, ExamName, ExamDate, ExamTime, TotalMarks, PassingMarks, Syllabus) VALUES
(1, 'Dotnet & C# End Semester Practical Exam', '2026-11-17', '09:30 AM - 12:30 PM', 100, 40, 'C# OOP, ASP.NET Core MVC, Entity Framework Core, Razor Views, Authentication.'),
(2, 'Data Structures Mid-Term Exam', '2026-11-23', '02:00 PM - 04:00 PM', 50, 20, 'Arrays, Linked Lists, Stacks, Queues, Binary Trees, Quick Sort, Merge Sort.'),
(3, 'DBMS Theory Final Exam', '2026-11-30', '10:00 AM - 01:00 PM', 100, 40, 'Relational Model, Normalization (1NF to BCNF), SQL DDL/DML, Transactions, ACID.');

-- Insert Study Sessions
INSERT INTO StudySessions (UserId, SubjectId, TaskId, Topic, StudyDate, StartTime, EndTime, DurationMinutes, Status, Notes, CreatedAt) VALUES
(1, 1, 1, 'Setting up Entity Framework Core and DbContext', '2026-11-03', '15:00', '17:00', 120, 'Completed', 'Successfully configured SQLite and models.', '2026-11-03'),
(1, 1, 1, 'Building Controllers and Razor Views', '2026-11-04', '18:30', '20:00', 90, 'Completed', 'Created CRUD views with Bootstrap 5 cards.', '2026-11-04'),
(1, 2, 2, 'Binary Search Tree Node Deletion cases', '2026-11-05', '14:00', '15:30', 90, 'Completed', 'Understood in-order successor logic.', '2026-11-05');

-- Insert Revision Plans
INSERT INTO RevisionPlans (UserId, SubjectId, ExamId, Topic, RevisionDate, StartTime, DurationMinutes, Status, Notes, CreatedAt) VALUES
(1, 1, 1, 'LINQ Queries & Entity Framework Core Performance', '2026-11-09', '10:00', 60, 'Scheduled', 'Practice Include, ThenInclude, AsNoTracking.', '2026-11-05'),
(1, 1, 1, 'ASP.NET Core Middleware & Authentication Pipeline', '2026-11-13', '11:00', 75, 'Scheduled', 'Review cookie authentication flow.', '2026-11-05');

-- Insert Daily Goals
INSERT INTO DailyGoals (UserId, GoalDate, TargetStudyMinutes, CompletedStudyMinutes, TargetTasks, CompletedTasks, Status, Notes) VALUES
(1, '2026-11-04', 120, 150, 2, 2, 'Achieved', 'Exceeded study target! Finished SQL lab.'),
(1, '2026-11-05', 150, 90, 3, 1, 'In Progress', 'Focus on Dotnet project and DSA BST assignment.');

-- Insert Notifications
INSERT INTO Notifications (UserId, Title, Message, NotificationType, NotificationDate, IsRead) VALUES
(1, 'Upcoming Exam Alert!', 'Your .NET Core & C# Programming exam is scheduled in 12 days.', 'ExamCountdown', '2026-11-05', 0),
(1, 'Assignment Deadline Approaching', 'Project Build Smart Study Planner is due in 3 days.', 'DeadlineReminder', '2026-11-05', 0),
(1, 'Daily Goal Milestone', 'Great job! You achieved your study goal yesterday with 150 mins logged.', 'DailyGoal', '2026-11-04', 1);

