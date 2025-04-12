-- Script para actualizar la contraseña del usuario admin
-- La nueva contraseña será: Admin123!

-- Actualizar la contraseña del usuario admin
UPDATE [dbo].[Users]
SET [PasswordHash] = '$2a$11$ysX.rXE3Ixj.enu6QJKdGOmUDJms7.V.8BdOqj7yJHGKZ1.w9j5Iq'
WHERE [Username] = 'admin';

PRINT 'Contraseña del usuario admin actualizada correctamente.';
