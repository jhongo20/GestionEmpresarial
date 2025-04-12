-- Script para corregir la contraseña del usuario admin
-- La contraseña será: Admin123!

-- Actualizar la contraseña del usuario admin con un hash generado correctamente
UPDATE [dbo].[Users]
SET [PasswordHash] = '$2a$11$K3g65rFCKMFQOGZjX/KkI.Vs19.qAB9SHGKmWQxb7.9QwiYyFMmHy'
WHERE [Username] = 'admin';

PRINT 'Contraseña del usuario admin actualizada correctamente con un nuevo hash.';
