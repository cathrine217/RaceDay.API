# RaceDay Event Management System

## Description

RaceDay is a full-stack web application designed specifically for the South African road running, walking, and cycling community. The platform allows Event Organisers to create and manage events, categories, and participant results, while Participants can browse upcoming events, enter events, track their personal performance history, and prepare for race day using live weather and route information.

This project is built progressively across three parts:
Part 1 System Planning and Database
Part 2 RESTful API (C#)
Part 3 MVC Web Application (with Azure Blob Storage and Docker)

## Part 2: RESTful API

This is the RESTful API for the RaceDay system, built with ASP.NET Core and Entity Framework Core.

### Features
Authentication: JWT-based login and registration for Organisers and Participants.
Role-Based Access: Enforced at the API level using Authorize(Roles) attributes.
CRUD Operations: Full management of Events, Categories, Enrolments, and Results.
Security: Passwords are securely hashed using BCrypt.
Swagger UI: Fully documented and testable endpoints with JWT Authorization support.
Unit Testing: Comprehensive tests using xUnit and an in-memory database.
CI/CD: Automated build and test pipeline using GitHub Actions.

## User Roles

### 1. Organiser
Create, edit, and delete events
Manage event categories
Capture participant results
View all event enrolments

### 2. Participant
Create an account
Browse events
Enter an event by selecting a category
View their own enrolments
Track personal results history

## Setup and Run Instructions

### Prerequisites
Visual Studio 2022
.NET 8.0 SDK
SQL Server (LocalDB or full instance)

### Step 1: Clone the Repository
git clone https://github.com/cathrine217/RaceDay.API.git
cd RaceDay.API

### Step 2: Create the Database
1. Open the Package Manager Console in Visual Studio.
2. Ensure the default project is set to RaceDay.API.
3. Run: Add-Migration InitialCreate
4. Run: Update-Database

### Step 3: Run the API
1. Set RaceDay.API as the Startup Project.
2. Press F5 to run the API.
3. Swagger UI will open at https://localhost:7204/swagger
4. Click Authorize to log in with a JWT token.

### Step 4: Run Unit Tests
1. Open Test Explorer.
2. Click Run All Tests.
3. All tests should pass with green checkmarks.

## CI/CD Green Build

The GitHub Actions workflow automatically builds the project and runs all unit tests on every push.

![Green Build](screenshots/green-build.png)


