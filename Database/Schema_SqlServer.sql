-- ========================================================
-- Smart Study Planner - Database Schema (Microsoft SQL Server)
-- Dotnet Subject Academic Project
-- Schema tables: Users, Subjects, Tasks, Exams, StudySessions,
--               RevisionPlans, DailyGoals, Notifications
-- ========================================================

-- Create Database if not exists
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'SmartStudyPlannerDb')
BEGIN
    CREATE DATABASE SmartStudyPlannerDb;
END
GO

USE SmartStudyPlannerDb;
GO

-- 1. Users Table
IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users (
        UserId INT IDENTITY(1,1) PRIMARY KEY,
        FullName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(150) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1
    );
END
GO

-- 2. Subjects Table
IF OBJECT_ID('dbo.Subjects', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Subjects (
        SubjectId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        SubjectName NVARCHAR(100) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CONSTRAINT FK_Subjects_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE CASCADE
    );
END
GO

-- 3. Tasks Table
IF OBJECT_ID('dbo.Tasks', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Tasks (
        TaskId INT IDENTITY(1,1) PRIMARY KEY,
        SubjectId INT NOT NULL,
        Title NVARCHAR(150) NOT NULL,
        Description NVARCHAR(1000) NULL,
        TaskType NVARCHAR(50) NOT NULL DEFAULT 'Assignment',
        Deadline DATETIME2 NOT NULL,
        Priority NVARCHAR(20) NOT NULL DEFAULT 'Medium',
        Difficulty NVARCHAR(20) NOT NULL DEFAULT 'Medium',
        Status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
        ProgressPercentage INT NOT NULL DEFAULT 0,
        EstimatedHours DECIMAL(6,2) NOT NULL DEFAULT 1.0,
        ActualHours DECIMAL(6,2) NOT NULL DEFAULT 0.0,
        CompletedAt DATETIME2 NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_Tasks_Subjects FOREIGN KEY (SubjectId) REFERENCES dbo.Subjects(SubjectId) ON DELETE CASCADE
    );
END
GO

-- 4. Exams Table
IF OBJECT_ID('dbo.Exams', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Exams (
        ExamId INT IDENTITY(1,1) PRIMARY KEY,
        SubjectId INT NOT NULL,
        ExamName NVARCHAR(150) NOT NULL,
        ExamDate DATETIME2 NOT NULL,
        ExamTime NVARCHAR(50) NOT NULL DEFAULT '10:00 AM',
        TotalMarks INT NOT NULL DEFAULT 100,
        PassingMarks INT NOT NULL DEFAULT 40,
        Syllabus NVARCHAR(2000) NULL,
        CONSTRAINT FK_Exams_Subjects FOREIGN KEY (SubjectId) REFERENCES dbo.Subjects(SubjectId) ON DELETE CASCADE
    );
END
GO

-- 5. StudySessions Table
IF OBJECT_ID('dbo.StudySessions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.StudySessions (
        StudySessionId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        SubjectId INT NOT NULL,
        TaskId INT NULL,
        Topic NVARCHAR(200) NOT NULL,
        StudyDate DATETIME2 NOT NULL,
        StartTime NVARCHAR(20) NOT NULL,
        EndTime NVARCHAR(20) NULL,
        DurationMinutes INT NOT NULL DEFAULT 60,
        Status NVARCHAR(30) NOT NULL DEFAULT 'Completed',
        Notes NVARCHAR(1000) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_StudySessions_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId),
        CONSTRAINT FK_StudySessions_Subjects FOREIGN KEY (SubjectId) REFERENCES dbo.Subjects(SubjectId),
        CONSTRAINT FK_StudySessions_Tasks FOREIGN KEY (TaskId) REFERENCES dbo.Tasks(TaskId) ON DELETE SET NULL
    );
END
GO

-- 6. RevisionPlans Table
IF OBJECT_ID('dbo.RevisionPlans', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.RevisionPlans (
        RevisionPlanId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        SubjectId INT NOT NULL,
        ExamId INT NULL,
        Topic NVARCHAR(200) NOT NULL,
        RevisionDate DATETIME2 NOT NULL,
        StartTime NVARCHAR(20) NULL,
        DurationMinutes INT NOT NULL DEFAULT 45,
        Status NVARCHAR(30) NOT NULL DEFAULT 'Scheduled',
        Notes NVARCHAR(1000) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_RevisionPlans_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId),
        CONSTRAINT FK_RevisionPlans_Subjects FOREIGN KEY (SubjectId) REFERENCES dbo.Subjects(SubjectId),
        CONSTRAINT FK_RevisionPlans_Exams FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId) ON DELETE SET NULL
    );
END
GO

-- 7. DailyGoals Table
IF OBJECT_ID('dbo.DailyGoals', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DailyGoals (
        GoalId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        GoalDate DATETIME2 NOT NULL,
        TargetStudyMinutes INT NOT NULL DEFAULT 120,
        CompletedStudyMinutes INT NOT NULL DEFAULT 0,
        TargetTasks INT NOT NULL DEFAULT 3,
        CompletedTasks INT NOT NULL DEFAULT 0,
        Status NVARCHAR(30) NOT NULL DEFAULT 'Pending',
        Notes NVARCHAR(1000) NULL,
        CONSTRAINT FK_DailyGoals_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE CASCADE
    );
END
GO

-- 8. Notifications Table
IF OBJECT_ID('dbo.Notifications', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Notifications (
        NotificationId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        Title NVARCHAR(150) NOT NULL,
        Message NVARCHAR(1000) NOT NULL,
        NotificationType NVARCHAR(50) NOT NULL DEFAULT 'General',
        NotificationDate DATETIME2 NOT NULL DEFAULT GETDATE(),
        IsRead BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_Notifications_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE CASCADE
    );
END
GO

