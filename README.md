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
## Database Schema

RaceDay has **two database artifacts** that serve different purposes:

### 1. `docs/RaceDayDB.sql` — Planned schema (Part 1)

The formal database design submitted as part of Part 1. It contains handwritten `CREATE TABLE` statements matching the ERD exactly, along with seed data. It creates a database called **`RaceDayDB`**.

This script represents the **full domain vision** of RaceDay and includes:
- A dedicated `Roles` lookup table
- A many-to-many relationship between Events and Categories via `EventCategories`
- Enrolment status tracking (`Pending`, `Confirmed`, `Withdrawn`, `Completed`)
- Bib number assignment
- A `WeatherInfo` table for race-day preparation
- Multiple indexes for performance

### 2. `RaceDayApiDb` — Running schema (Part 2)

The actual database the API reads and writes to at runtime. It is created by Entity Framework Core migrations in `RaceDay.Api/Migrations/` and is **not** the same as `RaceDayDB`. It implements the minimum schema required by the Part 2 functional requirements.

### Deliberate differences

Because Part 2 focuses on the API layer, the EF Core schema is a **simplified subset** of the Part 1 design:

| Concern | Part 1 (`RaceDayDB`) | Part 2 (`RaceDayApiDb`) |
|---------|----------------------|-------------------------|
| Primary keys | `UserId`, `EventId`, … | `Id` |
| Roles | Separate table | `Role` string column on `User` |
| Categories | Global, many-to-many via junction | Owned by one Event, one-to-many |
| Enrolment status | `Status` with check constraint | Not modelled |
| Bib number | `BibNumber` column | Not modelled |
| Weather | `WeatherInfo` table | Not modelled |
| Route info | `RouteInfo` column | Not modelled |

These differences are **intentional** — the Part 1 script documents the full planned system, while the Part 2 EF Core schema implements the specific features required for the RESTful API (authentication, events, categories, enrolments, and results). Future parts may extend the EF Core schema to align with the Part 1 vision.

Full planned schema and seed data: `docs/RaceDayDB.sql`.  
ERD: `docs/ERD Diagram.png`.

##Workflow
<img width="1240" height="1813" alt="Screenshot_16-10-2026_23170_github com" src="https://github.com/user-attachments/assets/0f006cbd-9286-4327-a861-062d17f17e04" />
<img width="1240" height="786" alt="Screenshot_16-10-2026_231732_github com" src="https://github.com/user-attachments/assets/1960828c-cfea-49e9-a94e-d11f285fbc51" />

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
