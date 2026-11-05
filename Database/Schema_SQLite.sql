-- ========================================================
-- Smart Study Planner - Database Schema (SQLite)
-- Dotnet Subject Academic Project
-- ========================================================

PRAGMA foreign_keys = ON;

-- 1. Users Table
CREATE TABLE IF NOT EXISTS Users (
    UserId INTEGER PRIMARY KEY AUTOINCREMENT,
    FullName TEXT NOT NULL,
    Email TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    IsActive INTEGER NOT NULL DEFAULT 1
);

-- 2. Subjects Table
CREATE TABLE IF NOT EXISTS Subjects (
    SubjectId INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    SubjectName TEXT NOT NULL,
    IsActive INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

-- 3. Tasks Table
CREATE TABLE IF NOT EXISTS Tasks (
    TaskId INTEGER PRIMARY KEY AUTOINCREMENT,
    SubjectId INTEGER NOT NULL,
    Title TEXT NOT NULL,
    Description TEXT,
    TaskType TEXT NOT NULL DEFAULT 'Assignment',
    Deadline TEXT NOT NULL,
    Priority TEXT NOT NULL DEFAULT 'Medium',
    Difficulty TEXT NOT NULL DEFAULT 'Medium',
    Status TEXT NOT NULL DEFAULT 'Pending',
    ProgressPercentage INTEGER NOT NULL DEFAULT 0,
    EstimatedHours REAL NOT NULL DEFAULT 1.0,
    ActualHours REAL NOT NULL DEFAULT 0.0,
    CompletedAt TEXT,
    CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (SubjectId) REFERENCES Subjects(SubjectId) ON DELETE CASCADE
);

-- 4. Exams Table
CREATE TABLE IF NOT EXISTS Exams (
    ExamId INTEGER PRIMARY KEY AUTOINCREMENT,
    SubjectId INTEGER NOT NULL,
    ExamName TEXT NOT NULL,
    ExamDate TEXT NOT NULL,
    ExamTime TEXT NOT NULL DEFAULT '10:00 AM',
    TotalMarks INTEGER NOT NULL DEFAULT 100,
    PassingMarks INTEGER NOT NULL DEFAULT 40,
    Syllabus TEXT,
    FOREIGN KEY (SubjectId) REFERENCES Subjects(SubjectId) ON DELETE CASCADE
);

-- 5. StudySessions Table
CREATE TABLE IF NOT EXISTS StudySessions (
    StudySessionId INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    SubjectId INTEGER NOT NULL,
    TaskId INTEGER,
    Topic TEXT NOT NULL,
    StudyDate TEXT NOT NULL,
    StartTime TEXT NOT NULL,
    EndTime TEXT,
    DurationMinutes INTEGER NOT NULL DEFAULT 60,
    Status TEXT NOT NULL DEFAULT 'Completed',
    Notes TEXT,
    CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (SubjectId) REFERENCES Subjects(SubjectId),
    FOREIGN KEY (TaskId) REFERENCES Tasks(TaskId) ON DELETE SET NULL
);

-- 6. RevisionPlans Table
CREATE TABLE IF NOT EXISTS RevisionPlans (
    RevisionPlanId INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    SubjectId INTEGER NOT NULL,
    ExamId INTEGER,
    Topic TEXT NOT NULL,
    RevisionDate TEXT NOT NULL,
    StartTime TEXT,
    DurationMinutes INTEGER NOT NULL DEFAULT 45,
    Status TEXT NOT NULL DEFAULT 'Scheduled',
    Notes TEXT,
    CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (SubjectId) REFERENCES Subjects(SubjectId),
    FOREIGN KEY (ExamId) REFERENCES Exams(ExamId) ON DELETE SET NULL
);

-- 7. DailyGoals Table
CREATE TABLE IF NOT EXISTS DailyGoals (
    GoalId INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    GoalDate TEXT NOT NULL,
    TargetStudyMinutes INTEGER NOT NULL DEFAULT 120,
    CompletedStudyMinutes INTEGER NOT NULL DEFAULT 0,
    TargetTasks INTEGER NOT NULL DEFAULT 3,
    CompletedTasks INTEGER NOT NULL DEFAULT 0,
    Status TEXT NOT NULL DEFAULT 'Pending',
    Notes TEXT,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

-- 8. Notifications Table
CREATE TABLE IF NOT EXISTS Notifications (
    NotificationId INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    Title TEXT NOT NULL,
    Message TEXT NOT NULL,
    NotificationType TEXT NOT NULL DEFAULT 'General',
    NotificationDate TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    IsRead INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

