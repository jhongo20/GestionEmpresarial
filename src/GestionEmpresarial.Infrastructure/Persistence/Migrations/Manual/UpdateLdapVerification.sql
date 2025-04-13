-- Script para actualizar la base de datos con los cambios necesarios para la verificación LDAP de usuarios internos
-- Fecha: 2025-04-12

-- Verificar si existe la columna IsInternalUser en la tabla Users, si no existe, agregarla
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Users' 
    AND COLUMN_NAME = 'IsInternalUser'
)
BEGIN
    ALTER TABLE Users
    ADD IsInternalUser BIT NOT NULL DEFAULT 0;
    
    PRINT 'Columna IsInternalUser agregada a la tabla Users';
END
ELSE
BEGIN
    PRINT 'La columna IsInternalUser ya existe en la tabla Users';
END

-- Actualizar los usuarios existentes con correo @mintrabajo.gov.co como usuarios internos
UPDATE Users
SET IsInternalUser = 1
WHERE Email LIKE '%@mintrabajo.gov.co' AND IsInternalUser = 0;

PRINT 'Usuarios con correo @mintrabajo.gov.co actualizados como usuarios internos';

-- Crear un índice para mejorar el rendimiento de las consultas que filtran por IsInternalUser
IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Users_IsInternalUser'
    AND object_id = OBJECT_ID('Users')
)
BEGIN
    CREATE INDEX IX_Users_IsInternalUser ON Users(IsInternalUser);
    PRINT 'Índice IX_Users_IsInternalUser creado en la tabla Users';
END
ELSE
BEGIN
    PRINT 'El índice IX_Users_IsInternalUser ya existe en la tabla Users';
END
