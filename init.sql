CREATE DATABASE AppointmentsSystemDb;
GO

CREATE LOGIN appointments_api_user WITH PASSWORD = 'AppPassword123!';
GO

USE AppointmentsSystemDb;
GO

CREATE USER appointments_api_user FOR LOGIN appointments_api_user;
GO

ALTER ROLE db_owner ADD MEMBER appointments_api_user;
GO
