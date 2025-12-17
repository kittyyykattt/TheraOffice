TheraOffice — Medical Office Management System

Overview

TheraOffice is a cross-platform medical office management application built with .NET MAUI and a RESTful Web API backend.
The system supports managing patients, physicians, appointments, and diagnoses, with persistent storage and a clean, user-friendly interface.

Key Features

Cross-platform desktop application (macOS / Windows)
RESTful Web API backend

CRUD operations for:
Patients
Physicians
Appointments
Diagnoses

Data persistence using JSON storage
Search and sorting functionality
Picker-based validation for entity relationships
Inline and dialog-based editing

Tech Stack

Frontend

.NET MAUI
XAML UI
Data binding & converters

Backend

ASP.NET Core Web API
RESTful endpoints
JSON file persistence


Build & Run Instructions

Run the API

cd TheraOffice.Api

dotnet run

API will start on a local development port.


Run the MAUI App (macOS)

cd Maui.TheraOffice

dotnet build -f net8.0-maccatalyst

dotnet run -f net8.0-maccatalyst
