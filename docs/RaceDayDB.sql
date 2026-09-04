-- ============================================================
-- RaceDay Database Schema
-- PROG6212 POE - Part 1
-- SQL Server Management Studio (SSMS)
-- ============================================================

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'RaceDayDB')
BEGIN
    ALTER DATABASE RaceDayDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE RaceDayDB;
END
GO

-- Create the database
CREATE DATABASE RaceDayDB;
GO

USE RaceDayDB;
GO
CREATE DATABASE RaceDayDB;
GO

USE RaceDayDB;
GO

-- ============================================================
-- 1. ROLES Table
-- ============================================================
CREATE TABLE Roles (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(200) NULL
);
GO

-- ============================================================
-- 2. USERS Table
-- ============================================================
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(20) NULL,
    RoleId INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
);
GO

-- ============================================================
-- 3. EVENTS Table
-- ============================================================
CREATE TABLE Events (
    EventId INT IDENTITY(1,1) PRIMARY KEY,
    EventName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    EventDate DATETIME NOT NULL,
    Location NVARCHAR(255) NOT NULL,
    RouteInfo NVARCHAR(MAX) NULL,
    OrganiserId INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Events_Users FOREIGN KEY (OrganiserId) REFERENCES Users(UserId)
);
GO

-- ============================================================
-- 4. CATEGORIES Table
-- ============================================================
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500) NULL,
    EntryFee DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    MinAge INT NULL,
    MaxAge INT NULL,
    CONSTRAINT CHK_MinAge CHECK (MinAge >= 0),
    CONSTRAINT CHK_MaxAge CHECK (MaxAge >= 0)
);
GO

-- ============================================================
-- 5. EVENTCATEGORIES Table (Many-to-Many Junction)
-- ============================================================
CREATE TABLE EventCategories (
    EventCategoryId INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL,
    CategoryId INT NOT NULL,
    MaxEntries INT NULL,
    CurrentEntries INT NOT NULL DEFAULT 0,
    CONSTRAINT FK_EventCategories_Events FOREIGN KEY (EventId) REFERENCES Events(EventId),
    CONSTRAINT FK_EventCategories_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId),
    CONSTRAINT UQ_EventCategories_EventCategory UNIQUE (EventId, CategoryId),
    CONSTRAINT CHK_CurrentEntries CHECK (CurrentEntries >= 0)
);
GO

-- ============================================================
-- 6. ENROLMENTS Table
-- ============================================================
CREATE TABLE Enrolments (
    EnrolmentId INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL,
    CategoryId INT NOT NULL,
    ParticipantId INT NOT NULL,
    EnrolmentDate DATETIME NOT NULL DEFAULT GETDATE(),
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    BibNumber NVARCHAR(20) NULL UNIQUE,
    CONSTRAINT FK_Enrolments_Events FOREIGN KEY (EventId) REFERENCES Events(EventId),
    CONSTRAINT FK_Enrolments_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId),
    CONSTRAINT FK_Enrolments_Users FOREIGN KEY (ParticipantId) REFERENCES Users(UserId),
    CONSTRAINT UQ_Enrolments_EventParticipantCategory UNIQUE (EventId, ParticipantId, CategoryId),
    CONSTRAINT CHK_EnrolmentStatus CHECK ([Status] IN ('Pending', 'Confirmed', 'Withdrawn', 'Completed'))
);
GO

-- ============================================================
-- 7. RESULTS Table
-- ============================================================
CREATE TABLE Results (
    ResultId INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentId INT NOT NULL,
    EventId INT NOT NULL,
    ParticipantId INT NOT NULL,
    TimeTaken TIME NULL,
    [Position] INT NULL,
    CategoryRank NVARCHAR(20) NULL,
    IsCompleted BIT NOT NULL DEFAULT 0,
    RecordedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Results_Enrolments FOREIGN KEY (EnrolmentId) REFERENCES Enrolments(EnrolmentId),
    CONSTRAINT FK_Results_Events FOREIGN KEY (EventId) REFERENCES Events(EventId),
    CONSTRAINT FK_Results_Users FOREIGN KEY (ParticipantId) REFERENCES Users(UserId),
    CONSTRAINT UQ_Results_Enrolment UNIQUE (EnrolmentId)
);
GO

-- ============================================================
-- 8. WEATHERINFO Table
-- ============================================================
CREATE TABLE WeatherInfo (
    WeatherId INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL,
    ForecastDate DATETIME NOT NULL,
    Temperature DECIMAL(5,2) NULL,
    Precipitation DECIMAL(5,2) NULL,
    WindSpeed INT NULL,
    Conditions NVARCHAR(100) NULL,
    LastUpdated DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_WeatherInfo_Events FOREIGN KEY (EventId) REFERENCES Events(EventId),
    CONSTRAINT UQ_WeatherInfo_EventForecast UNIQUE (EventId, ForecastDate)
);
GO

-- ============================================================
-- 9. INDEXES for Performance
-- ============================================================
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_RoleId ON Users(RoleId);
CREATE INDEX IX_Events_OrganiserId ON Events(OrganiserId);
CREATE INDEX IX_Events_EventDate ON Events(EventDate);
CREATE INDEX IX_EventCategories_EventId ON EventCategories(EventId);
CREATE INDEX IX_EventCategories_CategoryId ON EventCategories(CategoryId);
CREATE INDEX IX_Enrolments_EventId ON Enrolments(EventId);
CREATE INDEX IX_Enrolments_ParticipantId ON Enrolments(ParticipantId);
CREATE INDEX IX_Enrolments_CategoryId ON Enrolments(CategoryId);
CREATE INDEX IX_Results_EventId ON Results(EventId);
CREATE INDEX IX_Results_ParticipantId ON Results(ParticipantId);
CREATE INDEX IX_WeatherInfo_EventId ON WeatherInfo(EventId);
GO

-- ============================================================
-- 10. SEED DATA
-- ============================================================

INSERT INTO Roles (RoleName, Description) VALUES
    ('Organiser', 'Event organiser who can create and manage events'),
    ('Participant', 'Race participant who can enter events and track results');
GO

INSERT INTO Users (Email, PasswordHash, FirstName, LastName, PhoneNumber, RoleId) VALUES
    ('organiser1@raceday.co.za', '$2a$11$placeholderhash1234567890', 'Thabo', 'Nkosi', '0821234567', 1),
    ('organiser2@raceday.co.za', '$2a$11$placeholderhash1234567891', 'Zanele', 'Petersen', '0832345678', 1),
    ('runner1@raceday.co.za', '$2a$11$placeholderhash1234567892', 'Michael', 'Botha', '0843456789', 2),
    ('runner2@raceday.co.za', '$2a$11$placeholderhash1234567893', 'Sarah', 'van der Merwe', '0854567890', 2);
GO

INSERT INTO Categories (CategoryName, Description, EntryFee, MinAge, MaxAge) VALUES
    ('5km Walk', 'Fun walk for all ages', 50.00, NULL, NULL),
    ('10km Run', 'Standard 10km road run', 100.00, 16, NULL),
    ('21km Half Marathon', 'Half marathon distance', 200.00, 18, NULL),
    ('42km Marathon', 'Full marathon distance', 350.00, 20, NULL),
    ('50km Cycle', 'Cycling event', 150.00, 16, NULL),
    ('100km Cycle', 'Long distance cycling', 300.00, 18, NULL);
GO

INSERT INTO Events (EventName, Description, EventDate, Location, RouteInfo, OrganiserId) VALUES
    ('Cape Town Cycle Tour', 'Annual cycling event through Cape Town', '2026-11-15 06:00:00', 'Cape Town, Western Cape', 'Route starts at Green Point Stadium, goes along the coast to Muizenberg and back.', 1),
    ('Soweto Marathon', 'Iconic marathon through Soweto', '2026-10-05 05:30:00', 'Soweto, Gauteng', 'Starts at Orlando Stadium, winds through the streets of Soweto.', 2),
    ('Durban 10km Challenge', 'Coastal 10km road run', '2026-09-20 07:00:00', 'Durban, KwaZulu-Natal', 'Flat route along the Durban beachfront from Suncoast to uShaka Marine World.', 1);
GO

INSERT INTO EventCategories (EventId, CategoryId, MaxEntries, CurrentEntries) VALUES
    -- Cape Town Cycle Tour
    (1, 5, 500, 120),
    (1, 6, 200, 45),
    -- Soweto Marathon
    (2, 2, 300, 80),
    (2, 3, 200, 55),
    (2, 4, 100, 30),
    -- Durban 10km Challenge
    (3, 1, 150, 60),
    (3, 2, 200, 75);
GO

INSERT INTO Enrolments (EventId, CategoryId, ParticipantId, EnrolmentDate, [Status], BibNumber) VALUES
    (1, 5, 3, '2026-08-01 10:30:00', 'Confirmed', 'C001'),
    (1, 6, 4, '2026-08-02 14:15:00', 'Confirmed', 'C002'),
    (2, 3, 3, '2026-08-05 09:00:00', 'Pending', 'S001'),
    (2, 2, 4, '2026-08-06 11:45:00', 'Confirmed', 'S002'),
    (3, 2, 3, '2026-08-10 16:20:00', 'Confirmed', 'D001'),
    (3, 1, 4, '2026-08-12 08:30:00', 'Pending', 'D002');
GO

INSERT INTO Results (EnrolmentId, EventId, ParticipantId, TimeTaken, [Position], CategoryRank, IsCompleted, RecordedAt) VALUES
    (1, 1, 3, '02:15:30', 15, '5th', 1, GETDATE()),
    (2, 1, 4, '03:45:12', 8, '2nd', 1, GETDATE()),
    (5, 3, 3, '00:45:22', 3, '1st', 1, GETDATE());
GO

INSERT INTO WeatherInfo (EventId, ForecastDate, Temperature, Precipitation, WindSpeed, Conditions) VALUES
    (1, '2026-11-15 06:00:00', 18.5, 0.0, 12, 'Clear skies, light breeze'),
    (2, '2026-10-05 05:30:00', 16.0, 0.5, 8, 'Partly cloudy, mild'),
    (3, '2026-09-20 07:00:00', 22.0, 0.0, 15, 'Sunny, warm');
GO

-- ============================================================
-- 11. VERIFICATION QUERIES
-- ============================================================
SELECT 'Users' AS TableName, COUNT(*) AS "RowCount" FROM Users
UNION ALL SELECT 'Roles', COUNT(*) FROM Roles
UNION ALL SELECT 'Events', COUNT(*) FROM Events
UNION ALL SELECT 'Categories', COUNT(*) FROM Categories
UNION ALL SELECT 'EventCategories', COUNT(*) FROM EventCategories
UNION ALL SELECT 'Enrolments', COUNT(*) FROM Enrolments
UNION ALL SELECT 'Results', COUNT(*) FROM Results
UNION ALL SELECT 'WeatherInfo', COUNT(*) FROM WeatherInfo;
GO

SELECT 
    e.EventName,
    e.EventDate,
    e.Location,
    CONCAT(u.FirstName, ' ', u.LastName) AS Organiser,
    c.CategoryName,
    ec.MaxEntries,
    ec.CurrentEntries
FROM Events e
INNER JOIN Users u ON e.OrganiserId = u.UserId
LEFT JOIN EventCategories ec ON e.EventId = ec.EventId
LEFT JOIN Categories c ON ec.CategoryId = c.CategoryId
ORDER BY e.EventDate;
GO