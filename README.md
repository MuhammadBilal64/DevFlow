# DevFlow

DevFlow is a backend platform for team collaboration and project management built with ASP.NET Core.

The project follows Clean Architecture and CQRS principles and is designed to evolve into a scalable foundation for workspace management, project tracking, workflow automation, notifications, and analytics.

> ⚠️ Project Status: Active Development  
> Authentication and Workspace Management modules are completed. Additional modules are planned and will be added incrementally.

---

# Vision

DevFlow aims to become a modern collaboration platform inspired by tools such as Jira, Trello, and similar project management systems.

The long-term goal is to provide:

- Workspace Management
- Project Management
- Task Tracking
- Team Collaboration
- Notifications
- Workflow Automation
- Analytics & Reporting

---

# Features

## Authentication

- User Registration
- User Login
- JWT Authentication
- Refresh Token Support
- Secure Logout
- Password Hashing

## Workspace Management

- Create Workspace
- Get Workspace By Id
- Get My Workspaces
- Get Workspace Members
- Add Workspace Members
- Remove Workspace Members
- Workspace Role Management

## Security

- JWT Protected Endpoints
- Role-Based Authorization
- Owner/Admin Permission Enforcement
- Workspace Membership Validation
- Global Exception Handling

## Application Infrastructure

- CQRS with MediatR
- FluentValidation
- Validation Pipeline Behavior
- Logging Pipeline Behavior
- Dependency Injection

---

# Architecture

DevFlow follows Clean Architecture to maintain separation of concerns and support future growth.

## Project Structure

```text
DevFlow.Api
│
├── Controllers
├── Middleware
│
DevFlow.Application
│
├── Commands
├── Queries
├── Validators
├── Behaviors
├── Interfaces
│
DevFlow.Domain
│
├── Entities
├── Enums
│
DevFlow.Infrastructure
│
├── Persistence
├── Repositories
├── Security
```

## High-Level Flow

```text
Client
   │
   ▼
API Layer
   │
   ▼
Application Layer
(CQRS + MediatR)
   │
   ▼
Domain Layer
   │
   ▼
Infrastructure Layer
(EF Core + SQL Server)
```

## Architectural Patterns

- Clean Architecture
- CQRS (Command Query Responsibility Segregation)
- Repository Pattern
- MediatR
- Dependency Injection
- Pipeline Behaviors
- Global Exception Handling

---

# Technology Stack

## Backend

- ASP.NET Core
- C#
- Entity Framework Core
- SQL Server

## Libraries

- MediatR
- FluentValidation
- JWT Bearer Authentication

## API Documentation

- OpenAPI / Swagger

---

# Current Progress

| Module | Status |
|----------|----------|
| Authentication | ✅ Completed |
| Refresh Tokens | ✅ Completed |
| Workspace Management | ✅ Completed |
| Role-Based Authorization | ✅ Completed |
| CQRS Setup | ✅ Completed |
| Validation Pipeline | ✅ Completed |
| Logging Pipeline | ✅ Completed |
| Project Management | 🚧 Planned |
| Task Management | 🚧 Planned |
| Notifications | 🚧 Planned |
| Workflow Automation | 🚧 Planned |
| Analytics | 🚧 Planned |

---

# Implemented Domain Model

## User

- Authentication
- Refresh Tokens
- Workspace Memberships

## Workspace

- Owner
- Members
- Role Management

## Workspace Member

Supported Roles:

- Owner
- Admin
- Member

---

# Getting Started

## Prerequisites

- .NET SDK
- SQL Server

## Clone Repository

```bash
git clone https://github.com/MuhammadBilal64/DevFlow.git
cd DevFlow
```

## Configure Database

Update the connection string in:

```text
appsettings.json
```

## Apply Migrations

```bash
dotnet ef database update
```

## Run Application

```bash
dotnet run
```

The API will be available locally after startup.

---

# Roadmap

## Phase 1 (Completed)

- Authentication
- JWT & Refresh Tokens
- Workspace Management
- Role-Based Authorization

## Phase 2

- Project Management
- Task Management
- Comments
- Activity Tracking

## Phase 3

- Notifications
- Real-Time Updates
- Workflow Automation

## Phase 4

- Analytics
- Reporting
- Advanced System Design Features

---

# Learning Objectives

DevFlow is also a practical learning project focused on applying modern backend engineering concepts:

- Clean Architecture
- CQRS
- MediatR
- Entity Framework Core
- Authentication & Authorization
- System Design Fundamentals
- Scalable Backend Development

---

# Contributing

DevFlow is actively under development.

Suggestions, discussions, issue reports, and future contributions are welcome.

For major changes, please open an issue first to discuss the proposed improvement.

---

# License

This project is licensed under the MIT License.
