# RaceDay

A full-stack web-based event management system for the South African road running, walking, and cycling community.

RaceDay allows event organisers to create and manage events, categories, and participant results, while participants can browse upcoming events, enter events, view their own enrolments, and track their personal performance history.

**Module:** PROG6212 — Portfolio of Evidence
**System:** RaceDay
**Architecture:** ASP.NET Core Web API + SQL Server + Entity Framework Core

---

## Project Status

| Part | Description | Status |
|------|-------------|--------|
| Part 1 | System Planning and Database (ERD, API plan, SQL script) | ✅ Complete |
| Part 2 | RESTful API Development | ✅ Complete |
| Part 3 | MVC Web Front-End | ⏳ Upcoming |

---

## User Roles

### Organiser
- Register and log in
- Create, update, and delete events
- Define categories per event (age group + distance)
- View all enrolments for their events
- Capture participant results (finish time and position)

### Participant
- Register and log in
- Browse upcoming events (public)
- Enrol in an event by selecting a category
- View their own enrolments
- View their own results

Role-based access is enforced at the API level using JWT bearer authentication and `[Authorize(Roles = "...")]` attributes.

---

## Technology Stack

| Layer | Technology |
|-------|-----------|
| Language | C# 12 |
| Framework | ASP.NET Core Web API (.NET 10) |
| ORM | Entity Framework Core 10 |
| Database | SQL Server (LocalDB for development) |
| Authentication | JWT Bearer tokens + BCrypt password hashing |
| API Docs | Swagger / OpenAPI |
| Tests | xUnit v3 + EF Core InMemory |
| CI/CD | GitHub Actions |

---

## Repository Structure

```text
Part-1-Programming/
├── .github/
│   └── workflows/
│       ├── validate-docs.yml       # Part 1: validates /docs contents
│       └── dotnet-ci.yml           # Part 2: builds + tests on every push
├── docs/
│   ├── ERD Diagram.png             # Entity Relationship Diagram
│   ├── API Endpoint Reference Specification.md
│   ├── RaceDayDB.sql               # Database schema + seed data
│   ├── ci-docs-screenshot.png      # Part 1 CI green build
│   └── ci-dotnet-screenshot.png    # Part 2 CI green build
├── RaceDay.Api/
│   ├── Controllers/                # Auth, Users, Events, Categories, Enrolments, Results
│   ├── Data/                       # RaceDayDbContext
│   ├── DTOs/                       # Request/response models
│   ├── Migrations/                 # EF Core migrations
│   ├── Models/                     # User, Event, Category, Enrolment, Result
│   ├── Services/                   # TokenService (JWT generation)
│   ├── Program.cs
│   └── appsettings.json
├── RaceDay.Api.Tests/              # xUnit test project
└── README.md
```

---

## Setup and Run Instructions

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (preview is fine)
- SQL Server LocalDB (installed with Visual Studio) or SQL Server Express
- Git

### 1. Clone the repository

```bash
git clone https://github.com/Lele0658/Part-1-Programming.git
cd Part-1-Programming
```
````
