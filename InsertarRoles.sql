-- ==========================================================
-- SCRIPT DE CONFIGURACIÓN Y RESCATE - SISTEMA ERDYKA
-- ==========================================================

-- 1. Asegurar que los roles básicos existan en el sistema
IF NOT EXISTS (SELECT * FROM Roles WHERE NombreRol = 'Administrador')
    INSERT INTO Roles (NombreRol) VALUES ('Administrador');

IF NOT EXISTS (SELECT * FROM Roles WHERE NombreRol = 'Usuario')
    INSERT INTO Roles (NombreRol) VALUES ('Usuario');


-- ==========================================================
-- 2. CONSULTAS Y HERRAMIENTAS DE RESCATE (USUARIOS)
-- ==========================================================

-- Ver la lista completa de usuarios, sus correos, hashes y roles actuales
SELECT TOP (1000) [UsuarioId]
      ,[NombreUsuario]
      ,[Correo]
      ,[ContrasenaHash]
      ,[RolId]
      ,[Activo]
  FROM [ErdykaDb].[dbo].[Usuarios];


-- Eliminar un usuario específico por su ID (ej. si hay que borrarlo para volverlo a registrar)
-- (Cambia el número 8 por el ID que necesites)
DELETE FROM [ErdykaDb].[dbo].[Usuarios] 
WHERE [UsuarioId] = 8;


-- Cambiar el rol de un usuario mediante su correo electrónico
-- RolId 1 = Administrador, RolId 2 = Usuario normal
UPDATE [ErdykaDb].[dbo].[Usuarios] 
SET [RolId] = 2 
WHERE [Correo] = 'Marerdy2027@gmail.com';