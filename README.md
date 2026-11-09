# RaceDay - PROG6212 POE Part 3

## Project Overview
**RaceDay** is a complete, production-ready Event Management System built for the PROG6212 Portfolio of Evidence (Part 3). 

The solution consists of a RESTful Web API backend and an ASP.NET Core MVC frontend. The application allows **Organisers** to create events, upload banner images to Azure Blob Storage, and capture race results. **Participants** can view upcoming events, enrol in specific categories, and track their personal race history.

The MVC application consumes the API for all data operations and contains no direct database access or replicated business logic.

## Tech Stack
* **Backend:** ASP.NET Core 9.0 Web API
* **Frontend:** ASP.NET Core 9.0 MVC
* **Database:** Entity Framework Core (SQL Server)
* **Cloud Storage:** Azure Blob Storage
* **Security:** JWT Authentication & Session Management
* **Containerisation:** Docker
* **CI/CD:** GitHub Actions

## Key Features
* **Role-Based Navigation:** Distinct interfaces for Organisers and Participants using session management.
* **Azure Integration:** Secure image uploads (event banners and profile pictures) routed through the API to Azure Blob Storage.
* **Enrolment Tracking:** Color-coded enrolment statuses (e.g., green for Confirmed, amber for Pending).
* **Results Capture:** Organisers can record finish times and positions, which Participants can view in their personal race history.

## How to Run Locally
1. Clone the repository to your local machine.
2. Update the `DefaultConnection` and `AzureBlobStorage` strings in `RaceDay.Api/appsettings.json`.
3. Open a terminal in the `RaceDay.Api` folder and apply the database migrations:
   ```bash
   dotnet ef database update
Start the API (this must be running for the MVC app to work):

 ```bash
dotnet run --project RaceDay.Api
Open a new terminal and start the MVC app:
 ```
```bash
dotnet run --project RaceDay.Mvc
 ```
How to Run with Docker
The MVC application has been containerised using a multi-stage Dockerfile. To build and run the application via Docker:

Ensure Docker Desktop is running on your machine.

Open a terminal in the root folder of the solution (where the Dockerfile is located).

Build the Docker image:
docker build -t raceday-mvc -f Dockerfile .
Run the container:

docker run -d -p 8080:8080 --name raceday-web raceday-mvc
Access the application in your browser at: http://localhost:8080

(Note: When running the MVC app inside Docker, ensure the API's BaseAddress in Program.cs is pointing to an accessible endpoint, e.g., http://host.docker.internal:5223/).
