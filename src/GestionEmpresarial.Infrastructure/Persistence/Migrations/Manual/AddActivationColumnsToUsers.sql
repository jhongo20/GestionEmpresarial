-- Script simplificado para agregar columnas de activación a la tabla Users
BEGIN TRY
    -- Agregar columna ActivationToken si no existe
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'ActivationToken')
    BEGIN
        ALTER TABLE [dbo].[Users] ADD [ActivationToken] nvarchar(450) NULL;
        PRINT 'Columna ActivationToken agregada correctamente.';
    END
    ELSE
    BEGIN
        PRINT 'La columna ActivationToken ya existe.';
    END

    -- Agregar columna ActivationTokenExpires si no existe
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'ActivationTokenExpires')
    BEGIN
        ALTER TABLE [dbo].[Users] ADD [ActivationTokenExpires] datetime2(7) NULL;
        PRINT 'Columna ActivationTokenExpires agregada correctamente.';
    END
    ELSE
    BEGIN
        PRINT 'La columna ActivationTokenExpires ya existe.';
    END

    -- Agregar columna DeletedAt si no existe
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'DeletedAt')
    BEGIN
        ALTER TABLE [dbo].[Users] ADD [DeletedAt] datetime2(7) NULL;
        PRINT 'Columna DeletedAt agregada correctamente.';
    END
    ELSE
    BEGIN
        PRINT 'La columna DeletedAt ya existe.';
    END

    -- Agregar columna EmailConfirmed si no existe
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'EmailConfirmed')
    BEGIN
        ALTER TABLE [dbo].[Users] ADD [EmailConfirmed] bit NOT NULL DEFAULT 0;
        PRINT 'Columna EmailConfirmed agregada correctamente.';
    END
    ELSE
    BEGIN
        PRINT 'La columna EmailConfirmed ya existe.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error al ejecutar el script: ' + ERROR_MESSAGE();
END CATCH
