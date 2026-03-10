# Appointment System - Clean Architecture
## description
### Backend API for managing appointments with authentication,role-based authorization, and audit logging.
### Built using ASP.NET Core, Entity Framework, Clean Architecture. Planned Blazor frontend integration.
## Features

- User authentication with JWT
- Role-based access control (admin, Doctor, Patient)
- Appointment management with business rules (no overlapping)
- Audit logging for sensitive actions
- Clean Architecture structure
- Interface with Blazor

## Architecture
<ul>
  <li><b>UI (User interfaces)</b><br>
    <p>Frontend (Blazor) and controllers in ASP.NET Core WebApi</p>
  </li>
  <li><b>Application</b><br>
    <p>Business logic and orchestration(Services, Interfaces,DTOs and ServiceResponse)</p>
  </li>
  <li><b>Domain</b><br>
  <p>It is the core of the system, where the purest entities and rules reside.(Entities, etc.)</p>
  </li>
  <li><b>Infrastructure</b><br>
  <p>It is the layer that connects to external resources (Repositories, DbContext, etc.</p>
  </li>
</ul>

## Getting started
```bash
1. Clone the repository
  - git clone  https://github.com/Vick-vaporub2014/appointment-system.git
2. Navigate to the project folder
  - cd appointment-system
3. Restore dependencies
  - dotnet restore
4. Run the application
  - dotnet run  
5. Access to the API the API be available at:
  - https://localhost:7013
