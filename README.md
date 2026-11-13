# RaceDay Event Management System

## Description

RaceDay is a full-stack web application designed specifically for the South African road running, walking, and cycling community. The platform allows Event Organisers to create and manage events, categories, and participant results, while Participants can browse upcoming events, enter events, track their personal performance history, and prepare for race day using live weather and route information.

This project is built progressively across three parts:
- Part 1 System Planning and Database
- Part 2 RESTful API (C#)

## Part 1: System Planning and Database

Part 1 contains the foundational planning documents for the project.
- **ERD:** A complete Entity Relationship Diagram showing all 6 entities and their relationships.
- **API Endpoint Plan:** A detailed plan of all 19 RESTful API endpoints, including roles required.
- **SQL Script:** A SQL Server script to create the database schema and seed it with realistic sample data.

These documents can be found in the `/docs` folder of this repository.

## Part 2: RESTful API

This is the RESTful API for the RaceDay system, built with ASP.NET Core and Entity Framework Core. It serves as the backend for the entire system.

### Features
- Authentication: JWT-based login and registration for Organisers and Participants.
- Role-Based Access: Enforced at the API level to ensure users can only access permitted functionality.
- CRUD Operations: Full management of Events, Categories, Enrolments, and Results.
- Security: Passwords are securely hashed using BCrypt.
- Swagger UI: Fully documented and testable endpoints with JWT Authorization support.
- Unit Testing: Comprehensive tests using xUnit and an in-memory database.
- CI/CD: Automated build and test pipeline using GitHub Actions.

## User Roles

### 1. Organiser
- Create, edit, and delete events
- Manage event categories
- Capture participant results
- View all event enrolments

### 2. Participant
- Create an account
- Browse events
- Enter an event by selecting a category
- View their own enrolments
- Track personal results history

## Setup and Run Instructions

### Prerequisites
- Visual Studio 2022
- .NET 8.0 SDK
- SQL Server

