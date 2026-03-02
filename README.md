# Devdiscourse (ASP.NET Core MVC Migration)

This repository contains the **migration of [Devdiscourse.com](https://www.devdiscourse.com/)** from the legacy MVC framework to **ASP.NET Core MVC**.  
The goal is to modernize the platform with better performance, maintainability, and scalability.

---

## 🚀 Project Overview
Devdiscourse is a global media platform covering news, analysis, and insights on development issues across the world.  
This project migrates the existing ASP.NET MVC application into **ASP.NET Core MVC**, leveraging modern .NET features.

---

## 🏗 Tech Stack
- **.NET 8 / ASP.NET Core MVC**
- **Entity Framework Core** (SQL Server / Azure SQL Database)
- **Razor Views** with Layout & Partial Views
- **Dependency Injection** (built-in)
- **Identity / JWT Authentication** (if applicable)
- **Logging & Monitoring** via `ILogger` / Serilog
- **Hosting**: Azure App Service / IIS

---


---

## ⚙️ Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server / Azure SQL Database](https://azure.microsoft.com/en-us/products/azure-sql/)
- Visual Studio 2022 / VS Code

### Setup
1. Clone the repository:
   ```bash
   git clone https://github.com/<your-org>/devdiscourse.git
   cd devdiscourse/src/DevDiscourse.Web
