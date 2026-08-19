-- Script para asegurar los roles básicos del sistema Erdyka
IF NOT EXISTS (SELECT * FROM Roles WHERE NombreRol = 'Administrador')
    INSERT INTO Roles (NombreRol) VALUES ('Administrador');

IF NOT EXISTS (SELECT * FROM Roles WHERE NombreRol = 'Usuario')
    INSERT INTO Roles (NombreRol) VALUES ('Usuario');