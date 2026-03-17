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
1. Clone the Repository
     ```bash
     git clone https://github.com/Vick-vaporub2014/appointment-system.git
     cd appointment-system
2. Build conteiners
     ```bash
     docker compose up --build
3. The Api will be available at:
     ```bash
     http://localhost:8080
4. The SQL Server will be available at:
     ```bash
     localhost,1433
## Principals Endpoints (example)
<table>
  <thead>
    <tr>
      <th>Method</th>
      <th>Endpoint</th>
      <th>Description</th>
      <th>Authenitcation</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>POST</td>
      <td>/api/appointment</td>
      <td>Create appointment</td>
      <td>(Patient)</td>
    </tr>
    <tr>
      <td>GET</td>
      <td>/api/appointment</td>
      <td>Get all appointments</td>
      <td>(Admin and Doctor)</td>
    </tr>
  </tbody>
</table>
## Services Response
1. In  a error case, the serviceresponse with (example):
  {
  "success": false,
  "message": "Appointment overlaps with another",
  "errorType": "BusinessRule"
}


