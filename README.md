# Appointment System - Clean Architecture
## description
### Backend API for managing appointments with authentication,role-based authorization, and audit logging.
### Built using ASP.NET Core, Entity Framework, Clean Architecture. Planned Blazor frontend integration.
## Features
<ul>
  <li>User authentication with JWT</li>
  <li>Role-based access control (admin, Doctor, Patient)</li>
  <li>Appointment management with business rules (no overlapping)</li>
  <li>Audit logging for sensitive actions</li>
  <li>Clean Architecture structure</li>
  <li>Interface with Blazor</li>
</ul>

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
1. Clone the repository
  -  git clone  https://github.com/Vick-vaporub2014/appointment-system.git
2. Navigate to the project folder
  - cd appointment-system
3. Restore dependencies
  - dotnet restore
4. Run the application
  - dotnet run  
5. Access to the API
  - The API be available at: https://localhost:7013
