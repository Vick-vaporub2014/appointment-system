--Devolver el nopmbre de usarios y cuantas citas tiene
SELECT AspNetUsers.UserName, COUNT(*) AS NumeroDeCitas
FROM Appointments --LA TABLA QUE CONECTA LAS DOS COSAS QUE QUEREMOS VRE
INNER JOIN AspNetUsers ON Appointments.UserId = AspNetUsers.Id
GROUP BY AspNetUsers.UserName;

--Quiero ver el nombre,email del usario junto con su rol
--Se usa DISTINCT es una opcion para enlistar, y hace que todo se muestre aunque sea null
SELECT DISTINCT AspNetRoles.Name ,
AspNetUsers.UserName ,
AspNetUsers.Email 
FROM AspNetUserRoles --LA tabla tiene el userID y su RoleId, por lo tanto esa es el puente para conectar 
INNER JOIN AspNetRoles ON AspNetRoles.Id=AspNetUserRoles.RoleId  --Conecto con AspNetRoles para poder traerme el nombre del rol
INNER JOIN AspNetUsers ON AspNetUsers.Id = AspNetUserRoles.UserId --Conecto con AspNetUsers para traerme el nombre y email que tiene esa tabla

--Muestrame el nombre de usario, su email, su rol y cauntas citas tiene cada uno
--Appointment solo tiene usarioID
SELECT AspNetUsers.UserName,
AspNetRoles.Name,
AspNetUsers.Email,
COUNT(Appointments.AppointmentId) AS NumeroDeCitas--va a contar cuantas citas tiene el usario
FROM AspNetUsers --AspNetUsers porque es la que todo lo que necesitamos se conecta aqui (usuarioId)
INNER JOIN AspNetUserRoles ON AspNetUsers.Id = AspNetUserRoles.UserId --Conecto AspNetUserRoles con AspNetUers, asi ya tengo el RoleId
INNER JOIN AspNetRoles ON AspNetRoles.Id = AspNetUserRoles.RoleId --Conecto AspNetRoles con AspNetUserRoles para poder ver el nombre del RoleId
LEFT JOIN Appointments ON Appointments.AppointmentId = AspNetUsers.Id --Conecto el userId con el Id de AppointmentId, osea cada usario con sus citas
GROUP BY AspNetUsers.UserName, AspNetUsers.Email, AspNetRoles.Name --Como se UTILIZA EL COUNT NECESITO USAR EL GROUP BY PARA AGRUPAR 
ORDER BY NumeroDeCitas DESC; --Ordeno de mayor a menor el numero de citas.