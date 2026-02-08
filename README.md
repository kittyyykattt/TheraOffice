# TheraOffice — Medical Office Management System

TheraOffice is a cross-platform medical office management application built with **.NET MAUI** and an **ASP.NET Core Web API** backend.  
The system enables healthcare providers to efficiently manage patients, physicians, appointments, and diagnoses through a structured data model and intuitive desktop interface.

Designed to simulate real-world clinical workflows, TheraOffice demonstrates full-stack architecture, client–server communication, and persistent data handling.

---

## Overview

TheraOffice separates presentation and data layers using a RESTful architecture:

**MAUI Client → HTTP Requests → ASP.NET Core API → JSON Storage**

This design improves maintainability, scalability, and testability while mirroring modern production systems.

---

## Key Features

- Cross-platform desktop application (macOS & Windows)
- RESTful Web API backend
- Full CRUD operations for:
  - Patients  
  - Physicians  
  - Appointments  
  - Diagnoses
- Persistent JSON-based storage
- Search and sorting capabilities
- Picker-based validation for relational data
- Inline and modal editing workflows
- Structured models enforcing entity relationships

---

## Tech Stack

### Frontend
- **.NET MAUI**
- XAML UI
- MVVM-style data binding
- Value converters

### Backend
- **ASP.NET Core Web API**
- RESTful endpoints
- JSON file persistence

---

## Architecture

TheraOffice
│
├── Maui.TheraOffice → Cross-platform desktop client
├── TheraOffice.Api → ASP.NET Core REST API
└── Shared Models → Structured domain entities


The MAUI client communicates with the API over HTTP, enabling a clean separation between UI and data management layers.

---

## Getting Started

### Prerequisites

Install the following:

- **.NET 8 SDK**
- **Visual Studio 2022+** or **JetBrains Rider**
- MAUI workload:

```bash
dotnet workload install maui

Verify installation:
dotnet --version


Build & Run Instructions:
Run the Web API
cd TheraOffice.Api
dotnet restore
dotnet build
dotnet run

Run the MAUI Desktop App (macOS):
cd Maui.TheraOffice
dotnet restore
dotnet build -f net8.0-maccatalyst
dotnet run -f net8.0-maccatalyst

Running on Windows:
dotnet build -f net8.0-windows10.0.19041.0
dotnet run -f net8.0-windows10.0.19041.0

---

Project Structure
TheraOffice/
│
├── Maui.TheraOffice/
│   ├── Views
│   ├── ViewModels
│   └── Services
│
├── TheraOffice.Api/
│   ├── Controllers
│   ├── Models
│   └── Data

Author
Katya Serechenko
Computer Science — Florida State University

GitHub:
https://github.com/kittyyykattt
