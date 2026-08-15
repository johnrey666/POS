dotnet run --project src\POSSystem.Desktop\POSSystem.Desktop.csproj
>>
# FOD POS System

A Windows desktop point-of-sale application built with .NET 10 and WPF, designed for local-only operations with SQLite persistence, role-based security, branch-aware access, and cashier-focused POS workflows.

This project is structured in layers to separate business logic, data access, and UI concerns while keeping the app easy to extend step by step.

## Overview

FOD POS System is a local-first retail and restaurant-style desktop POS built for small businesses and branch-based operations. It supports:

- secure login with role-based permissions
- branch and terminal assignment
- category and product catalog browsing
- cashier POS selling flow
- cart management with quantity updates
- subtotal, tax, and total calculations
- hold and checkout actions
- local SQLite database seeding for demo users and sample inventory

The application is intended to run without cloud infrastructure or external services, using the local Windows AppData folder for persistent storage.

## Tech stack

- C#
- .NET 10
- WPF (Windows desktop UI)
- MVVM pattern
- SQLite
- Entity Framework Core
- PBKDF2 password hashing
- Windows Local AppData storagew

## Project architecture

### Domain layer
The Domain project contains the core business entities, security rules, and service contracts.

Key responsibilities:
- entities such as users, permissions, roles, branches, products, and terminals
- permission constants and role names
- interfaces for auth, authorization, catalog, and management services

### Infrastructure layer
The Infrastructure project handles communication with SQLite, schema initialization, seeding, auth, permissions, and product/branch services.

Key responsibilities:
- EF Core DbContext setup
- local SQLite bootstrap
- seed data for roles, permissions, users, branches, terminals, and products
- password hashing and verification
- role-based authorization logic
- product catalog and branch-related services

### Desktop layer
The Desktop project contains the WPF application, views, view models, navigation shell, and startup logic.

Key responsibilities:
- app startup and database initialization
- login window
- shell navigation between dashboard, POS, products, and permissions
- UI data binding and MVVM commands
- cashier workflow and product interactions

## Database and storage

The app creates and manages a local SQLite database automatically.

Database path:
- %LOCALAPPDATA%\POSSystem\pos.db

This means the project is self-contained and does not require a separate database server or external installation.

When the app starts, the bootstrap process:
- creates the database if needed
- ensures the schema is present
- seeds demo roles, permissions, users, branches, terminals, and products

## Authentication and authorization

Authentication is handled in the Infrastructure layer and uses PBKDF2 hashing for secure password verification.

### Demo accounts
- cashier / cashier123
- cashiersupervisor / cashiersupervisor123
- admin / admin123

Permissions are granted through roles and checked at runtime before users can access a feature. This gives the system a practical permission-based model for cashier, supervisor, and admin roles.

## POS features

The current cashier flow includes:

- category filtering
- product search
- add to cart
- quantity changes
- subtotal, tax, and total calculation
- hold sale
- checkout flow
- stock deduction on purchase
- recent sales preview

## Phase roadmap

### Phase 1
- project setup
- database initialization
- local app bootstrapping

### Phase 2
- login
- users
- roles
- permissions
- access security

### Phase 3
- branches
- terminals
- branch-aware login and session context

### Phase 4
- product catalog
- cashier POS flow
- cart and transaction logic

### Planned future phases
- product CRUD management screen
- discounts and approval workflow
- reports and analytics
- sales history
- receipt printing and payment flows
- inventory replenishment
- audit logs and admin controls

## Running the app

From the repository root:

```powershell command
dotnet restore
dotnet build POSSystem.sln -nologo
dotnet run --project POSSystem.Desktop.csproj
