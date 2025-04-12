-- Script para actualizar la tabla Users con los campos necesarios para la activación de cuentas con código
-- Verificar si la columna ActivationToken existe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'ActivationToken')
BEGIN
    ALTER TABLE [dbo].[Users] ADD [ActivationToken] nvarchar(450) NULL;
    PRINT 'Columna ActivationToken agregada correctamente.';
END
ELSE
BEGIN
    PRINT 'La columna ActivationToken ya existe.';
END

-- Verificar si la columna ActivationTokenExpires existe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'ActivationTokenExpires')
BEGIN
    ALTER TABLE [dbo].[Users] ADD [ActivationTokenExpires] datetime2(7) NULL;
    PRINT 'Columna ActivationTokenExpires agregada correctamente.';
END
ELSE
BEGIN
    PRINT 'La columna ActivationTokenExpires ya existe.';
END

-- Verificar si la columna DeletedAt existe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'DeletedAt')
BEGIN
    ALTER TABLE [dbo].[Users] ADD [DeletedAt] datetime2(7) NULL;
    PRINT 'Columna DeletedAt agregada correctamente.';
END
ELSE
BEGIN
    PRINT 'La columna DeletedAt ya existe.';
END

-- Renombrar IsEmailConfirmed a EmailConfirmed si existe
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'IsEmailConfirmed')
    AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'EmailConfirmed')
BEGIN
    EXEC sp_rename 'dbo.Users.IsEmailConfirmed', 'EmailConfirmed', 'COLUMN';
    PRINT 'Columna IsEmailConfirmed renombrada a EmailConfirmed correctamente.';
END
ELSE
BEGIN
    -- Si no existe IsEmailConfirmed pero tampoco EmailConfirmed, crear EmailConfirmed
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'EmailConfirmed')
    BEGIN
        ALTER TABLE [dbo].[Users] ADD [EmailConfirmed] bit NOT NULL DEFAULT 0;
        PRINT 'Columna EmailConfirmed agregada correctamente.';
    END
    ELSE
    BEGIN
        PRINT 'La columna EmailConfirmed ya existe.';
    END
END

-- Eliminar la columna EmailConfirmedAt si existe
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'EmailConfirmedAt')
BEGIN
    -- Eliminar la columna directamente
    ALTER TABLE [dbo].[Users] DROP COLUMN [EmailConfirmedAt];
    
    PRINT 'Columna EmailConfirmedAt eliminada correctamente.';
END
ELSE
BEGIN
    PRINT 'La columna EmailConfirmedAt no existe.';
END

-- Crear índice para ActivationToken si no existe
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'ActivationToken')
    AND NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_ActivationToken' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE INDEX [IX_Users_ActivationToken] ON [dbo].[Users]([ActivationToken]) WHERE [ActivationToken] IS NOT NULL;
    PRINT 'Índice IX_Users_ActivationToken creado correctamente.';
END
ELSE
BEGIN
    PRINT 'El índice IX_Users_ActivationToken ya existe o la columna ActivationToken no existe.';
END
