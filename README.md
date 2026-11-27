# 🎓 Smart Study Planner (Preparation Assistant)
> **Academic Project for .NET / Dotnet Subject**  
> Built with **ASP.NET Core 8.0 MVC**, **Entity Framework Core**, **Bootstrap 5**, **Chart.js**, **FullCalendar**, **SQLite**, **SQL Server**, and **MongoDB Atlas**.
> 
> 🌐 **Live Web Application**: [https://studysphere.runasp.net](https://studysphere.runasp.net)  
> 🔗 **GitHub Repository**: [https://github.com/AvinashRaj1703/Preparation-Assistant](https://github.com/AvinashRaj1703/Preparation-Assistant)

---

## 🌐 Live Website URL & Demo Credentials

The web application is live, deployed, and secured with HTTPS:
* 👉 **Live Website**: [https://studysphere.runasp.net](https://studysphere.runasp.net)
* **Pre-Seeded Demo Account**:
  * **Email**: `student@study.edu`
  * **Password**: `Password123!`
  * *(Or click the green **"One-Click Demo Student Sign In"** button on the home page).*

---

## 📖 Table of Contents
1. [🌐 Live Website URL & Demo Credentials](#-live-website-url--demo-credentials)
2. [🌟 What is Smart Study Planner?](#-what-is-smart-study-planner)
3. [🚶 Non-Technical User Flow: Step-by-Step Walkthrough](#-non-technical-user-flow-step-by-step-walkthrough)
4. [📊 How Progress Bars & Total Study Time Work](#-how-progress-bars--total-study-time-work)
5. [🗄️ Database Schema & Relational Architecture](#️-database-schema--relational-architecture)
6. [📋 Dummy Data Tables (All 8 Database Tables with Sample Records)](#-dummy-data-tables-all-8-database-tables-with-sample-records)
7. [🚀 How to Run the Project (Zero Configuration)](#-how-to-run-the-project-zero-configuration)
8. [🍃 Hybrid Multi-Database Architecture (SQLite + SQL Server + MongoDB)](#-hybrid-multi-database-architecture)
9. [💡 Viva & Faculty Examination Questions & Answers](#-viva--faculty-examination-questions--answers)

---

## 🌟 What is Smart Study Planner?

Students often struggle to balance multiple subjects, assignments, upcoming exam deadlines, and daily revision. Normally, they have to use 4 or 5 different apps:
- A calendar app for exam dates.
- A to-do list app for homework and assignments.
- A stopwatch or Pomodoro timer for study blocks.
- A notebook or diary to track daily revision.

**Smart Study Planner** brings all of these academic needs into **one easy-to-use web application**. It automatically connects your subjects, tasks, exams, and daily goals so you can focus on studying instead of managing schedules.

---

## 🚶 Non-Technical User Flow: Step-by-Step Walkthrough

Even if you have never used a study app or written a line of code, here is how the application works from start to finish:

```mermaid
flowchart TD
    A["1. Sign In / Register"] --> B["2. Add Your Subjects"]
    B --> C["3. Create Tasks & Assignments"]
    B --> D["4. Add Upcoming Exams"]
    C --> E["5. Set Daily Goals & Focus Tasks"]
    D --> F["6. Schedule Revision Plans"]
    E --> G["7. Log Study Sessions"]
    G --> H["8. Automatic Progress Bar & Time Update"]
    H --> I["9. View Dashboard & Visual Analytics"]
```

---

### Step 1: Sign In or Register an Account
* When you open the website, you are greeted with a clean login screen.
* **New Students**: Click **Register** to create an account with your Name, Email, and Password.
* **Instant Demo**: Click the green **"One-Click Demo Student Sign In"** button to log in instantly as `student@study.edu`.
* **Security**: Your password is automatically scrambled and encrypted with military-grade BCrypt encryption so nobody can steal it.

---

### Step 2: Add Your Academic Subjects
* Click on **Subjects** in the top navigation bar.
* Click **"+ Add New Subject"** and enter your courses (for example: *Data Structures & Algorithms*, *Database Management Systems*, *Computer Networks*).
* Every subject acts as a primary folder that will hold all your tasks, exams, and revision sessions.

---

### Step 3: Add Tasks, Assignments & Homework
* Go to **Tasks ➔ Add New Task**.
* Choose your subject, give the task a title (e.g., *"Implement Binary Search Tree"*), choose priority (*High / Medium / Low*), difficulty, and set a deadline.
* **Conflict Prevention**: To stop you from creating duplicate tasks with the exact same name by mistake, the system has a built-in search picker and duplicate name blocker.
* As you work on a task, you can move its slider from 0% to 100% or click the checkmark to mark it done.

---

### Step 4: Add Upcoming Exams & Countdown Timers
* Go to **Exams ➔ Add Exam**.
* Enter the subject, exam title (e.g., *"Midterm Exam"*), exam date, time, total marks (e.g., 100), passing marks (e.g., 40), and syllabus details.
* The system calculates a live **Exam Countdown** (e.g., *"14 days remaining"*).
* If an exam is within 14 days, the system automatically creates a yellow notification alert.

---

### Step 5: Schedule Revision Plans
* Go to **Revision Plans ➔ Add Revision**.
* Link your revision directly to an upcoming exam and subject.
* Set the topic to revise (e.g., *"Tree Traversals & Balancing"*), date, and duration.
* This ensures you don't cram the night before the exam, practicing spaced repetition.

---

### Step 6: Set Your Daily Goals
* Go to **Daily Goals**.
* Set your target for today: e.g., **Target Study Time: 120 Minutes** and **Target Tasks: 3 Tasks**.
* **Review Specific Task Titles**: Instead of showing just numbers, the screen displays the actual titles of the tasks scheduled for today with direct one-click completion checkboxes.
* **Quick Selector**: You can pick any existing task from your subjects directly from the dropdown without typing anything new.

---

### Step 7: Record Focused Study Sessions (Pomodoro / Deep Work)
* When you sit down to study, go to **Study Sessions ➔ Log Session**.
* Select the subject, the task you are working on, the topic, and how many minutes you studied (e.g., 60 minutes).
* You can add personal study notes or formulas to remember.

---

### Step 8: Watch Your Progress Bars & Total Time Increase Automatically
* As soon as you mark a study session complete or check off a task, the system updates everything in real time:
  1. The **Task Milestone Progress Bar** moves closer to 100%.
  2. The **Study Time Progress Bar** fills up with your completed minutes.
  3. Once both reach 100%, your daily goal status turns from **"In Progress"** to a bright green **"Achieved"** badge!

---

### Step 9: Monitor Your Dashboard & Analytics
* The **Dashboard** is your personal command center:
  * **Top Metrics**: Total Study Hours, Active Tasks, Upcoming Exams, and Current Daily Streak.
  * **Weekly Study Trends Chart**: A visual bar chart showing how many hours you studied Monday through Sunday.
  * **Subject Distribution Doughnut Chart**: Shows which subjects you spend the most time on.
  * **Today's Focus Card**: Lists today's tasks and progress bars.

---

### Step 10: Check Notifications & Master Calendar
* **Notifications**: The bell icon in the navigation bar alerts you when an exam is coming up within 14 days or when an assignment is due within 3 days.
* **Unified Calendar**: Open the **Calendar** tab to view your full month timetable showing exams, tasks, study sessions, and revision plans color-coded on the calendar.

---

## 📊 How Progress Bars & Total Study Time Work

For project evaluation and viva defense, here is the exact math and logic behind the numbers:

### 1. Task Milestone Progress Bar
$$\text{Task Progress \%} = \min\left(100, \left(\frac{\text{Completed Tasks}}{\text{Target Tasks}}\right) \times 100\right)$$
* **Example**: If your target is 4 tasks for today and you check off 2 tasks, the progress bar displays $\frac{2}{4} \times 100 = 50\%$.
* **Status**: When `Completed Tasks == Target Tasks`, the milestone is marked complete.

### 2. Study Time Milestone Progress Bar
$$\text{Study Time Progress \%} = \min\left(100, \left(\frac{\text{Completed Study Minutes}}{\text{Target Study Minutes}}\right) \times 100\right)$$
* **Example**: If your goal is 120 minutes of study and you complete a 60-minute study session, the bar displays $\frac{60}{120} \times 100 = 50\%$.
* **Goal Achieved Trigger**: When both Tasks and Study Minutes reach 100%, the `DailyGoals.Status` automatically switches from `"In Progress"` to `"Achieved"`.

### 3. How Total Study Time Increases
* Every time a study session is saved with `Status = "Completed"`, its `DurationMinutes` is saved into the database.
* The system runs a real-time LINQ summation:
  $$\text{Total Hours} = \frac{\sum \text{DurationMinutes of Completed Sessions}}{60}$$
* **Task Actual Hours**: If the study session was linked to a specific task, that task's `ActualHours` field automatically increases by $\frac{\text{DurationMinutes}}{60}$, allowing students to compare their **Estimated vs. Actual study effort**.

---

## 🗄️ Database Schema & Relational Architecture

The system is built on **8 interconnected tables** designed to show clean primary key / foreign key relationships:

```mermaid
erDiagram
    Users ||--o{ Subjects : "creates (1:M)"
    Users ||--o{ StudySessions : "logs (1:M)"
    Users ||--o{ RevisionPlans : "schedules (1:M)"
    Users ||--o{ DailyGoals : "tracks (1:M)"
    Users ||--o{ Notifications : "receives (1:M)"
    
    Subjects ||--o{ Tasks : "contains (1:M)"
    Subjects ||--o{ Exams : "schedules (1:M)"
    Subjects ||--o{ StudySessions : "categorizes (1:M)"
    Subjects ||--o{ RevisionPlans : "categorizes (1:M)"
    
    Tasks ||--o{ StudySessions : "tracks_time_for (1:M)"
    Exams ||--o{ RevisionPlans : "targets (1:M)"
```

---

## 📋 Dummy Data Tables (All 8 Database Tables with Sample Records)

Below are the **exact 8 database tables** populated with realistic dummy student data demonstrating how the database stores information:

### Table 1: `Users`
Stores student accounts, login credentials, and account activation flags.

| UserId | FullName | Email | PasswordHash | IsActive |
| :---: | :--- | :--- | :--- | :---: |
| **1** | Alex Mercer | `student@study.edu` | `$2a$11$N9qo8uLOickgx2ZMRZoMye...` (Salted BCrypt) | `true` |
| **2** | Keyuri Joshi | `keyuri@university.edu` | `$2a$11$zE6b1W4xVqKlmT3YpRt8ue...` (Salted BCrypt) | `true` |
| **3** | Rahul Sharma | `rahul.s@tech.edu` | `$2a$11$aL9k7P2qW8xVbN4mO1zYve...` (Salted BCrypt) | `true` |

---

### Table 2: `Subjects`
Stores the academic courses belonging to each student (`UserId` foreign key).

| SubjectId | UserId | SubjectName | IsActive |
| :---: | :---: | :--- | :---: |
| **1** | 1 | Data Structures & Algorithms | `true` |
| **2** | 1 | Database Management Systems | `true` |
| **3** | 1 | Computer Networks | `true` |
| **4** | 1 | Operating Systems | `true` |
| **5** | 1 | .NET Application Development | `true` |

---

### Table 3: `Tasks`
Stores assignments, coding labs, and project tasks linked to a specific subject (`SubjectId` foreign key).

| TaskId | SubjectId | Title | Description | TaskType | Deadline | Priority | Difficulty | Status | Progress % | Est. Hours | Act. Hours | CompletedAt | CreatedAt |
| :---: | :---: | :--- | :--- | :--- | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: |
| **1** | 1 | Implement Binary Search Tree | Write insert, delete, and traversal in C# | Assignment | 2026-11-28 | High | Hard | In Progress | 60% | 4.0 | 2.5 | *NULL* | 2026-11-20 |
| **2** | 1 | Dijkstra Algorithm Lab | Solve shortest path problem in graph | Lab Work | 2026-11-26 | Medium | Medium | Completed | 100% | 3.0 | 3.0 | 2026-11-26 | 2026-11-21 |
| **3** | 2 | SQL Normalization Assignment | Normalize student database from 1NF to 3NF | Assignment | 2026-11-26 | High | Medium | Completed | 100% | 2.5 | 2.0 | 2026-11-26 | 2026-11-22 |
| **4** | 3 | Subnetting Practice Problems | Calculate CIDR masks for Class B networks | Practice | 2026-11-30 | Low | Easy | Pending | 0% | 2.0 | 0.0 | *NULL* | 2026-11-23 |
| **5** | 5 | ASP.NET Core MVC Mini Project | Build full CRUD application with EF Core | Project | 2026-12-05 | High | Hard | In Progress | 80% | 15.0 | 12.0 | *NULL* | 2026-11-15 |

---

### Table 4: `Exams`
Stores upcoming university examinations, passing criteria, and syllabus breakdown.

| ExamId | SubjectId | ExamName | ExamDate | ExamTime | TotalMarks | PassingMarks | Syllabus |
| :---: | :---: | :--- | :---: | :---: | :---: | :---: | :--- |
| **1** | 1 | DSA Midterm Examination | 2026-12-10 | 10:00 AM | 100 | 40 | Arrays, Stacks, Queues, Trees, Graphs, Sorting |
| **2** | 2 | DBMS Semester Practical Exam | 2026-12-15 | 02:00 PM | 50 | 20 | SQL queries, PL/SQL stored procedures, Indexing |
| **3** | 3 | Computer Networks Final Exam | 2026-12-20 | 09:30 AM | 100 | 40 | OSI Model, TCP/IP, Routing Protocols, DNS, HTTP |
| **4** | 5 | .NET Technologies Viva & Exam | 2026-12-01 | 11:00 AM | 70 | 28 | C# Basics, ASP.NET Core, EF Core, Dependency Injection |

---

### Table 5: `StudySessions`
Stores focused study blocks logged by students, linking time spent to subjects and tasks.

| StudySessionId | UserId | SubjectId | TaskId | Topic | StudyDate | StartTime | EndTime | Duration (Mins) | Status | Notes | CreatedAt |
| :---: | :---: | :---: | :---: | :--- | :---: | :---: | :---: | :---: | :---: | :--- | :---: |
| **1** | 1 | 1 | 1 | BST Insertion & Deletion Logic | 2026-11-26 | 09:00 AM | 10:30 AM | 90 | Completed | Understood in-order predecessor deletion | 2026-11-26 |
| **2** | 1 | 2 | 3 | Boyce-Codd Normal Form Rules | 2026-11-26 | 02:00 PM | 03:00 PM | 60 | Completed | Solved 5 functional dependency questions | 2026-11-26 |
| **3** | 1 | 5 | 5 | Setting up Entity Framework DbContext | 2026-11-25 | 04:00 PM | 06:00 PM | 120 | Completed | Configured OnModelCreating and Foreign Keys | 2026-11-25 |
| **4** | 1 | 3 | 4 | IP Subnetting & CIDR Notation | 2026-11-27 | 10:00 AM | 11:30 AM | 90 | Planned | Need to revise /24 to /28 subnet masks | 2026-11-26 |

---

### Table 6: `RevisionPlans`
Stores spaced repetition revision schedules targeted directly towards upcoming exams.

| RevisionPlanId | UserId | SubjectId | ExamId | Topic | RevisionDate | StartTime | Duration (Mins) | Status | Notes | CreatedAt |
| :---: | :---: | :---: | :---: | :--- | :---: | :---: | :---: | :---: | :--- | :---: |
| **1** | 1 | 1 | 1 | AVL Tree Rotations (LL, RR, LR, RL) | 2026-11-27 | 08:00 AM | 45 | Pending | Revise balance factor formulas | 2026-11-25 |
| **2** | 1 | 2 | 2 | ACID Properties & Transaction Isolation | 2026-11-28 | 05:00 PM | 60 | Pending | Practice dirty read and phantom read examples | 2026-11-25 |
| **3** | 1 | 5 | 4 | MVC Lifecycle & Middleware Pipeline | 2026-11-26 | 07:00 PM | 60 | Completed | Revised UseRouting, UseAuthentication, UseAuthorization | 2026-11-24 |

---

### Table 7: `DailyGoals`
Stores daily study commitments, targets, completed statistics, and goal achievement statuses.

| GoalId | UserId | GoalDate | Target Study Mins | Completed Study Mins | Target Tasks | Completed Tasks | Status | Notes |
| :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :--- |
| **1** | 1 | 2026-11-25 | 120 | 120 | 2 | 2 | **Achieved** | Met all goals! Finished EF Core setup and lab practice. |
| **2** | 1 | 2026-11-26 | 150 | 150 | 3 | 2 | **In Progress** | 1 task remaining for today's milestone. |
| **3** | 1 | 2026-11-27 | 120 | 0 | 2 | 0 | **Pending** | Focus on AVL Tree revision and Subnetting. |

---

### Table 8: `Notifications`
Stores system-generated alerts and countdown reminders for the student.

| NotificationId | UserId | Title | Message | NotificationType | NotificationDate | IsRead |
| :---: | :---: | :--- | :--- | :---: | :---: | :---: |
| **1** | 1 | Upcoming Exam Alert | 'DSA Midterm Examination' is scheduled in 14 days! | ExamAlert | 2026-11-26 | `false` |
| **2** | 1 | Task Deadline Approaching | 'Implement Binary Search Tree' is due in 2 days! | TaskDeadline | 2026-11-26 | `false` |
| **3** | 1 | Goal Accomplished! | Congratulations! You achieved your daily study goal for 2026-11-25. | Achievement | 2026-11-25 | `true` |

---

## 🚀 How to Run the Project (Zero Configuration)

You do **not** need to install or configure any database server to run and evaluate this project. SQLite works out-of-the-box on Windows, Mac, and Linux.

### 1. Prerequisites
* Install [.NET 8.0 SDK](https://dotnet.microsoft.com/download) (Free from Microsoft).

### 2. Run the Application
Open your terminal or command prompt inside the project folder (`/Users/avinash/Desktop/DNT`) and execute:

```bash
dotnet run --launch-profile http
```

### 3. Open in Your Browser
Navigate to:
👉 **`http://localhost:5186`**

### 4. Instant Demo Sign-In
* Click the green **"One-Click Demo Student Sign In"** button on the home page, OR use:
  * **Email**: `student@study.edu`
  * **Password**: `Password123!`

The database, tables, and sample data are created and seeded **automatically** on your first run!

---

## 🍃 Hybrid Multi-Database Architecture

This project was engineered to demonstrate multi-database enterprise readiness:

| Database Engine | Type | Primary Purpose in this Project |
| :--- | :--- | :--- |
| **SQLite** | Local Relational | Default, zero-configuration local development on macOS / Linux / Windows. |
| **SQL Server** | Enterprise Relational | Production evaluation in college labs and Windows environments using SSMS. |
| **MongoDB Atlas** | Cloud NoSQL | Real-time cloud backup, disaster recovery, and high-availability replica set storage. |

### How to Inspect the Cloud Database
1. Launch the web app and click **Cloud DB** in the top navigation bar (or visit `http://localhost:5186/DatabaseStatus`).
2. The page displays:
   - Live ping response latency to the MongoDB cloud cluster (`cluster0.mio42uj.mongodb.net`).
   - Total document counts across all 8 cloud collections.
   - A **"Sync Local SQLite Data to MongoDB Cloud"** button that synchronizes all relational records into cloud collections with 1 click.

---

## 🌐 Cloud Deployment Guide (Render / Railway / Azure)

### Why Render / Railway instead of Vercel?
* **Vercel** is designed specifically for frontend JavaScript/TypeScript frameworks (Next.js, React, Vue) and serverless functions. It does **not** natively support the Microsoft .NET 8.0 SDK or long-running ASP.NET Core Kestrel web servers.
* **Render (render.com)** and **Railway (railway.app)** are modern cloud platforms that natively support Docker containers and ASP.NET Core apps with **free tiers** and automatic deployment directly from GitHub.

### 🛡️ Will Deploying this Affect My Other Projects (e.g. `saarthi`)?
> **NO, ABSOLUTELY NOT!**  
> Every project you deploy in an organization or team group is **completely isolated**:
> - **Separate Git Repository**: `Preparation-Assistant` is an independent repository.
> - **Separate Subdomain & URLs**: It receives its own distinct URL (e.g. `preparation-assistant.onrender.com`), completely separate from `saarthi`.
> - **Separate Compute & Containers**: It runs inside its own isolated Linux container. It shares zero memory, zero processes, and zero database files with `saarthi`.
> - **Zero Risk**: As long as you create it as a **New Project** and import the `Preparation-Assistant` repository, your existing `saarthi` project is 100% untouched and safe.

### 🚀 2-Minute Deployment on Render (Step-by-Step):
1. Go to [render.com](https://render.com) and sign in with GitHub.
2. Click **New +** ➔ Select **Web Service**.
3. Choose **"Build and deploy from a Git repository"** and select `Preparation-Assistant` (or paste `https://github.com/AvinashRaj1703/Preparation-Assistant`).
4. In the setup form:
   - **Name**: `smart-study-planner` (or any name you like)
   - **Language / Runtime**: **Docker** (Render will automatically detect the included `Dockerfile`).
   - **Instance Type**: **Free**.
5. *(Optional)* Under **Environment Variables**, you can add:
   - `ConnectionStrings__MongoDbConnection`: `mongodb+srv://keyurijjoshi_db_user:2dG0e99lpgjMF68h@cluster0.mio42uj.mongodb.net/`
   - `MongoDbDatabaseName`: `SmartStudyPlanner_Cloud`
6. Click **Create Web Service**. Render will automatically build the Docker image and give you a live HTTPS web address!

---

## 💡 Viva & Faculty Examination Questions & Answers

Be completely prepared for questions asked by internal and external evaluators:

#### Q1: What architecture does this project follow?
> **Answer**: It follows the **Model-View-Controller (MVC)** architectural pattern using **ASP.NET Core 8.0**. 
> - **Model**: Encapsulates entity classes (`StudyTask`, `Subject`, `DailyGoal`, etc.) and database context (`ApplicationDbContext`).
> - **View**: Razor syntax (`.cshtml`) generating responsive HTML5 with Bootstrap 5 and Chart.js.
> - **Controller**: Handles user HTTP requests, performs validation, interacts with EF Core, and returns Views or JSON.

#### Q2: What ORM is used and how does it prevent SQL Injection?
> **Answer**: We use **Entity Framework Core 8.0**. EF Core automatically parameterizes all queries generated through LINQ (Language Integrated Query), completely neutralizing SQL Injection attacks.

#### Q3: How are passwords stored securely?
> **Answer**: Passwords are never stored as plain text. We utilize `BCrypt.Net-Next` with automatic cryptographic salting and a high work-factor cost, protecting against dictionary and rainbow-table attacks.

#### Q4: How is referential integrity maintained between tables?
> **Answer**: Foreign key constraints are explicitly declared using Entity Framework Core's **Fluent API** in `ApplicationDbContext.cs`. Delete behaviors are configured with `DeleteBehavior.Restrict` on recursive user paths to avoid SQL Server multiple cascade cycle exceptions.

#### Q5: How do the Daily Goal progress bars update?
> **Answer**: The task progress is calculated as $\frac{\text{CompletedTasks}}{\text{TargetTasks}} \times 100$, and study progress as $\frac{\text{CompletedStudyMinutes}}{\text{TargetStudyMinutes}} \times 100$. When study sessions or tasks are marked completed, the database updates and the status automatically transitions from `"In Progress"` to `"Achieved"`.

---

### 👨‍💻 Project Information
* **Project Name**: Smart Study Planner (Preparation Assistant)
* **Subject**: Dotnet / .NET Application Development
* **Framework**: ASP.NET Core 8.0 (C#)
* **Live Website**: [https://studysphere.runasp.net](https://studysphere.runasp.net)
* **Code Repository**: [GitHub - Preparation-Assistant](https://github.com/AvinashRaj1703/Preparation-Assistant)
