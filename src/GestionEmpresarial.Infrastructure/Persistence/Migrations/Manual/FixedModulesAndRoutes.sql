-- Script de migración manual para agregar tablas de módulos y rutas

-- Crear tabla de Modules
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Modules]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Modules] (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [Icon] NVARCHAR(50) NULL,
        [Path] NVARCHAR(100) NULL,
        [Order] INT NOT NULL DEFAULT 0,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [CreatedBy] NVARCHAR(100) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL,
        [UpdatedBy] NVARCHAR(100) NOT NULL,
        [UpdatedAt] DATETIME2 NOT NULL,
        CONSTRAINT [PK_Modules] PRIMARY KEY ([Id])
    );
    PRINT 'Tabla Modules creada correctamente.';
END
ELSE
BEGIN
    PRINT 'La tabla Modules ya existe.';
    
    -- Verificar si la columna Path existe, si no, agregarla
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Modules]') AND name = 'Path')
    BEGIN
        ALTER TABLE [dbo].[Modules] ADD [Path] NVARCHAR(100) NULL;
        PRINT 'Columna Path agregada a la tabla Modules.';
    END
END

-- Crear tabla de Routes
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Routes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Routes] (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Path] NVARCHAR(100) NOT NULL,
        [Icon] NVARCHAR(50) NULL,
        [Order] INT NOT NULL DEFAULT 0,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [ModuleId] UNIQUEIDENTIFIER NOT NULL,
        [CreatedBy] NVARCHAR(100) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL,
        [UpdatedBy] NVARCHAR(100) NOT NULL,
        [UpdatedAt] DATETIME2 NOT NULL,
        CONSTRAINT [PK_Routes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Routes_Modules_ModuleId] FOREIGN KEY ([ModuleId]) REFERENCES [dbo].[Modules] ([Id]) ON DELETE CASCADE
    );
    PRINT 'Tabla Routes creada correctamente.';
END
ELSE
BEGIN
    PRINT 'La tabla Routes ya existe.';
END

-- Crear tabla de RoleModules
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RoleModules]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RoleModules] (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [RoleId] UNIQUEIDENTIFIER NOT NULL,
        [ModuleId] UNIQUEIDENTIFIER NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [CreatedBy] NVARCHAR(100) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL,
        [UpdatedBy] NVARCHAR(100) NOT NULL,
        [UpdatedAt] DATETIME2 NOT NULL,
        CONSTRAINT [PK_RoleModules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RoleModules_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RoleModules_Modules_ModuleId] FOREIGN KEY ([ModuleId]) REFERENCES [dbo].[Modules] ([Id]) ON DELETE CASCADE
    );
    PRINT 'Tabla RoleModules creada correctamente.';
END
ELSE
BEGIN
    PRINT 'La tabla RoleModules ya existe.';
END

-- Crear tabla de RoleRoutes
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RoleRoutes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RoleRoutes] (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [RoleId] UNIQUEIDENTIFIER NOT NULL,
        [RouteId] UNIQUEIDENTIFIER NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [CreatedBy] NVARCHAR(100) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL,
        [UpdatedBy] NVARCHAR(100) NOT NULL,
        [UpdatedAt] DATETIME2 NOT NULL,
        CONSTRAINT [PK_RoleRoutes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RoleRoutes_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RoleRoutes_Routes_RouteId] FOREIGN KEY ([RouteId]) REFERENCES [dbo].[Routes] ([Id]) ON DELETE CASCADE
    );
    PRINT 'Tabla RoleRoutes creada correctamente.';
END
ELSE
BEGIN
    PRINT 'La tabla RoleRoutes ya existe.';
END

-- Verificar si la columna IsActive existe en UserRoles, si no, agregarla
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[UserRoles]') AND name = 'IsActive')
BEGIN
    ALTER TABLE [dbo].[UserRoles] ADD [IsActive] BIT NOT NULL DEFAULT 1;
    PRINT 'Columna IsActive agregada a la tabla UserRoles.';
END
ELSE
BEGIN
    PRINT 'La columna IsActive ya existe en la tabla UserRoles.';
END

-- Agregar roles por defecto si no existen
IF NOT EXISTS (SELECT * FROM [dbo].[Roles] WHERE [Name] = 'Administrador')
BEGIN
    INSERT INTO [dbo].[Roles] ([Id], [Name], [Description], [IsDeleted], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
    VALUES 
        (NEWID(), 'Administrador', 'Rol con acceso completo al sistema', 0, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Supervisor', 'Rol con acceso a funciones de supervisión', 0, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Usuario', 'Rol con acceso básico al sistema', 0, 'System', GETDATE(), 'System', GETDATE());
    
    PRINT 'Roles por defecto creados correctamente.';
END
ELSE
BEGIN
    PRINT 'Los roles por defecto ya existen.';
END

-- Insertar datos de ejemplo para módulos
IF NOT EXISTS (SELECT * FROM [dbo].[Modules] WHERE [Name] = 'Dashboard')
BEGIN
    INSERT INTO [dbo].[Modules] ([Id], [Name], [Description], [Icon], [Path], [Order], [IsActive], [IsDeleted], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
    VALUES 
        (NEWID(), 'Dashboard', 'Panel principal del sistema', 'dashboard', '/dashboard', 1, 1, 0, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Usuarios', 'Gestión de usuarios', 'people', '/users', 2, 1, 0, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Roles', 'Gestión de roles y permisos', 'security', '/roles', 3, 1, 0, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Configuración', 'Configuración del sistema', 'settings', '/settings', 4, 1, 0, 'System', GETDATE(), 'System', GETDATE());
    PRINT 'Datos de ejemplo para módulos insertados correctamente.';
END
ELSE
BEGIN
    PRINT 'Los datos de ejemplo para módulos ya existen.';
END

-- Insertar rutas para el módulo Dashboard
DECLARE @DashboardModuleId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Modules] WHERE [Name] = 'Dashboard');
IF @DashboardModuleId IS NOT NULL AND NOT EXISTS (SELECT * FROM [dbo].[Routes] WHERE [Name] = 'Inicio' AND [ModuleId] = @DashboardModuleId)
BEGIN
    INSERT INTO [dbo].[Routes] ([Id], [Name], [Path], [Icon], [Order], [IsActive], [IsDeleted], [ModuleId], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
    VALUES 
        (NEWID(), 'Inicio', '/dashboard', 'home', 1, 1, 0, @DashboardModuleId, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Estadísticas', '/dashboard/stats', 'bar_chart', 2, 1, 0, @DashboardModuleId, 'System', GETDATE(), 'System', GETDATE());
    PRINT 'Rutas para el módulo Dashboard insertadas correctamente.';
END
ELSE
BEGIN
    PRINT 'Las rutas para el módulo Dashboard ya existen o el módulo no existe.';
END

-- Insertar rutas para el módulo Usuarios
DECLARE @UsersModuleId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Modules] WHERE [Name] = 'Usuarios');
IF @UsersModuleId IS NOT NULL AND NOT EXISTS (SELECT * FROM [dbo].[Routes] WHERE [Name] = 'Lista de Usuarios' AND [ModuleId] = @UsersModuleId)
BEGIN
    INSERT INTO [dbo].[Routes] ([Id], [Name], [Path], [Icon], [Order], [IsActive], [IsDeleted], [ModuleId], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
    VALUES 
        (NEWID(), 'Lista de Usuarios', '/users/list', 'list', 1, 1, 0, @UsersModuleId, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Crear Usuario', '/users/create', 'add', 2, 1, 0, @UsersModuleId, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Editar Usuario', '/users/edit', 'edit', 3, 1, 0, @UsersModuleId, 'System', GETDATE(), 'System', GETDATE());
    PRINT 'Rutas para el módulo Usuarios insertadas correctamente.';
END
ELSE
BEGIN
    PRINT 'Las rutas para el módulo Usuarios ya existen o el módulo no existe.';
END

-- Insertar rutas para el módulo Roles
DECLARE @RolesModuleId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Modules] WHERE [Name] = 'Roles');
IF @RolesModuleId IS NOT NULL AND NOT EXISTS (SELECT * FROM [dbo].[Routes] WHERE [Name] = 'Lista de Roles' AND [ModuleId] = @RolesModuleId)
BEGIN
    INSERT INTO [dbo].[Routes] ([Id], [Name], [Path], [Icon], [Order], [IsActive], [IsDeleted], [ModuleId], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
    VALUES 
        (NEWID(), 'Lista de Roles', '/roles/list', 'list', 1, 1, 0, @RolesModuleId, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Crear Rol', '/roles/create', 'add', 2, 1, 0, @RolesModuleId, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Editar Rol', '/roles/edit', 'edit', 3, 1, 0, @RolesModuleId, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Asignar Permisos', '/roles/permissions', 'security', 4, 1, 0, @RolesModuleId, 'System', GETDATE(), 'System', GETDATE());
    PRINT 'Rutas para el módulo Roles insertadas correctamente.';
END
ELSE
BEGIN
    PRINT 'Las rutas para el módulo Roles ya existen o el módulo no existe.';
END

-- Insertar rutas para el módulo Configuración
DECLARE @ConfigModuleId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Modules] WHERE [Name] = 'Configuración');
IF @ConfigModuleId IS NOT NULL AND NOT EXISTS (SELECT * FROM [dbo].[Routes] WHERE [Name] = 'General' AND [ModuleId] = @ConfigModuleId)
BEGIN
    INSERT INTO [dbo].[Routes] ([Id], [Name], [Path], [Icon], [Order], [IsActive], [IsDeleted], [ModuleId], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
    VALUES 
        (NEWID(), 'General', '/settings/general', 'settings', 1, 1, 0, @ConfigModuleId, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Seguridad', '/settings/security', 'security', 2, 1, 0, @ConfigModuleId, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Notificaciones', '/settings/notifications', 'notifications', 3, 1, 0, @ConfigModuleId, 'System', GETDATE(), 'System', GETDATE());
    PRINT 'Rutas para el módulo Configuración insertadas correctamente.';
END
ELSE
BEGIN
    PRINT 'Las rutas para el módulo Configuración ya existen o el módulo no existe.';
END

PRINT 'Script de migración ejecutado correctamente.';
