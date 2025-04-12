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

-- Asignar todos los módulos al rol de Administrador
DECLARE @AdminRoleId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Roles] WHERE [Name] = 'Administrador');
IF @AdminRoleId IS NOT NULL
BEGIN
    -- Asignar módulos al rol de Administrador
    INSERT INTO [dbo].[RoleModules] ([Id], [RoleId], [ModuleId], [IsActive], [IsDeleted], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
    SELECT 
        NEWID(), 
        @AdminRoleId, 
        [Id], 
        1, 
        0, 
        'System', 
        GETDATE(), 
        'System', 
        GETDATE()
    FROM [dbo].[Modules]
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[RoleModules] 
        WHERE [RoleId] = @AdminRoleId AND [ModuleId] = [dbo].[Modules].[Id]
    );
    
    -- Asignar rutas al rol de Administrador
    INSERT INTO [dbo].[RoleRoutes] ([Id], [RoleId], [RouteId], [IsActive], [IsDeleted], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
    SELECT 
        NEWID(), 
        @AdminRoleId, 
        [Id], 
        1, 
        0, 
        'System', 
        GETDATE(), 
        'System', 
        GETDATE()
    FROM [dbo].[Routes]
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[RoleRoutes] 
        WHERE [RoleId] = @AdminRoleId AND [RouteId] = [dbo].[Routes].[Id]
    );
    
    PRINT 'Módulos y rutas asignados al rol de Administrador correctamente.';
END
ELSE
BEGIN
    PRINT 'El rol de Administrador no existe.';
END

PRINT 'Script de migración ejecutado correctamente.';

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

-- Agregar permisos por defecto si no existen
IF NOT EXISTS (SELECT * FROM [dbo].[Permissions] WHERE [Name] = 'Crear Usuario')
BEGIN
    INSERT INTO [dbo].[Permissions] ([Id], [Name], [Description], [IsDeleted], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
    VALUES 
        -- Permisos de Usuarios
        (NEWID(), 'Ver Usuarios', 'Permiso para ver la lista de usuarios', 0, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Crear Usuario', 'Permiso para crear nuevos usuarios', 0, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Editar Usuario', 'Permiso para editar usuarios existentes', 0, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Eliminar Usuario', 'Permiso para eliminar usuarios', 0, 'System', GETDATE(), 'System', GETDATE()),
        
        -- Permisos de Roles
        (NEWID(), 'Ver Roles', 'Permiso para ver la lista de roles', 0, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Crear Rol', 'Permiso para crear nuevos roles', 0, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Editar Rol', 'Permiso para editar roles existentes', 0, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Eliminar Rol', 'Permiso para eliminar roles', 0, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Asignar Permisos', 'Permiso para asignar permisos a roles', 0, 'System', GETDATE(), 'System', GETDATE()),
        
        -- Permisos de Módulos
        (NEWID(), 'Ver Módulos', 'Permiso para ver la lista de módulos', 0, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Crear Módulo', 'Permiso para crear nuevos módulos', 0, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Editar Módulo', 'Permiso para editar módulos existentes', 0, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Eliminar Módulo', 'Permiso para eliminar módulos', 0, 'System', GETDATE(), 'System', GETDATE()),
        
        -- Permisos de Rutas
        (NEWID(), 'Ver Rutas', 'Permiso para ver la lista de rutas', 0, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Crear Ruta', 'Permiso para crear nuevas rutas', 0, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Editar Ruta', 'Permiso para editar rutas existentes', 0, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Eliminar Ruta', 'Permiso para eliminar rutas', 0, 'System', GETDATE(), 'System', GETDATE()),
        
        -- Permisos de Configuración
        (NEWID(), 'Ver Configuración', 'Permiso para ver la configuración del sistema', 0, 'System', GETDATE(), 'System', GETDATE()),
        (NEWID(), 'Editar Configuración', 'Permiso para editar la configuración del sistema', 0, 'System', GETDATE(), 'System', GETDATE());
    
    PRINT 'Permisos por defecto creados correctamente.';
END
ELSE
BEGIN
    PRINT 'Los permisos por defecto ya existen.';
END

-- Asignar permisos a los roles
DECLARE @AdminRoleId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Roles] WHERE [Name] = 'Administrador');
DECLARE @SupervisorRoleId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Roles] WHERE [Name] = 'Supervisor');
DECLARE @UserRoleId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Roles] WHERE [Name] = 'Usuario');

-- Asignar todos los permisos al rol de Administrador
IF @AdminRoleId IS NOT NULL
BEGIN
    INSERT INTO [dbo].[RolePermissions] ([Id], [RoleId], [PermissionId], [IsDeleted], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
    SELECT 
        NEWID(), 
        @AdminRoleId, 
        [Id], 
        0, 
        'System', 
        GETDATE(), 
        'System', 
        GETDATE()
    FROM [dbo].[Permissions]
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[RolePermissions] 
        WHERE [RoleId] = @AdminRoleId AND [PermissionId] = [dbo].[Permissions].[Id]
    );
    
    PRINT 'Permisos asignados al rol de Administrador correctamente.';
END

-- Asignar permisos de visualización y edición al rol de Supervisor (excepto eliminar y configuración)
IF @SupervisorRoleId IS NOT NULL
BEGIN
    INSERT INTO [dbo].[RolePermissions] ([Id], [RoleId], [PermissionId], [IsDeleted], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
    SELECT 
        NEWID(), 
        @SupervisorRoleId, 
        [Id], 
        0, 
        'System', 
        GETDATE(), 
        'System', 
        GETDATE()
    FROM [dbo].[Permissions]
    WHERE [Name] NOT LIKE 'Eliminar%' 
      AND [Name] NOT LIKE '%Configuración'
      AND [Name] NOT LIKE 'Crear Rol'
      AND [Name] NOT LIKE 'Editar Rol'
      AND [Name] NOT LIKE 'Asignar Permisos'
      AND [Name] NOT LIKE 'Crear Módulo'
      AND [Name] NOT LIKE 'Editar Módulo'
      AND [Name] NOT LIKE 'Crear Ruta'
      AND [Name] NOT LIKE 'Editar Ruta'
      AND NOT EXISTS (
        SELECT 1 FROM [dbo].[RolePermissions] 
        WHERE [RoleId] = @SupervisorRoleId AND [PermissionId] = [dbo].[Permissions].[Id]
    );
    
    PRINT 'Permisos asignados al rol de Supervisor correctamente.';
END

-- Asignar permisos básicos al rol de Usuario (solo visualización)
IF @UserRoleId IS NOT NULL
BEGIN
    INSERT INTO [dbo].[RolePermissions] ([Id], [RoleId], [PermissionId], [IsDeleted], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
    SELECT 
        NEWID(), 
        @UserRoleId, 
        [Id], 
        0, 
        'System', 
        GETDATE(), 
        'System', 
        GETDATE()
    FROM [dbo].[Permissions]
    WHERE [Name] LIKE 'Ver%' 
      AND [Name] <> 'Ver Configuración'
      AND NOT EXISTS (
        SELECT 1 FROM [dbo].[RolePermissions] 
        WHERE [RoleId] = @UserRoleId AND [PermissionId] = [dbo].[Permissions].[Id]
    );
    
    PRINT 'Permisos asignados al rol de Usuario correctamente.';
END

-- Asignar módulos a los roles
-- Para el rol de Supervisor (todos excepto Configuración)
IF @SupervisorRoleId IS NOT NULL
BEGIN
    INSERT INTO [dbo].[RoleModules] ([Id], [RoleId], [ModuleId], [IsActive], [IsDeleted], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
    SELECT 
        NEWID(), 
        @SupervisorRoleId, 
        [Id], 
        1, 
        0, 
        'System', 
        GETDATE(), 
        'System', 
        GETDATE()
    FROM [dbo].[Modules]
    WHERE [Name] <> 'Configuración'
      AND NOT EXISTS (
        SELECT 1 FROM [dbo].[RoleModules] 
        WHERE [RoleId] = @SupervisorRoleId AND [ModuleId] = [dbo].[Modules].[Id]
    );
    
    PRINT 'Módulos asignados al rol de Supervisor correctamente.';
END

-- Para el rol de Usuario (solo Dashboard y limitado)
IF @UserRoleId IS NOT NULL
BEGIN
    INSERT INTO [dbo].[RoleModules] ([Id], [RoleId], [ModuleId], [IsActive], [IsDeleted], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
    SELECT 
        NEWID(), 
        @UserRoleId, 
        [Id], 
        1, 
        0, 
        'System', 
        GETDATE(), 
        'System', 
        GETDATE()
    FROM [dbo].[Modules]
    WHERE [Name] = 'Dashboard'
      AND NOT EXISTS (
        SELECT 1 FROM [dbo].[RoleModules] 
        WHERE [RoleId] = @UserRoleId AND [ModuleId] = [dbo].[Modules].[Id]
    );
    
    PRINT 'Módulos asignados al rol de Usuario correctamente.';
END

-- Asignar rutas a los roles
-- Para el rol de Supervisor (todas las rutas de los módulos asignados)
IF @SupervisorRoleId IS NOT NULL
BEGIN
    INSERT INTO [dbo].[RoleRoutes] ([Id], [RoleId], [RouteId], [IsActive], [IsDeleted], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
    SELECT 
        NEWID(), 
        @SupervisorRoleId, 
        r.[Id], 
        1, 
        0, 
        'System', 
        GETDATE(), 
        'System', 
        GETDATE()
    FROM [dbo].[Routes] r
    INNER JOIN [dbo].[Modules] m ON r.[ModuleId] = m.[Id]
    INNER JOIN [dbo].[RoleModules] rm ON m.[Id] = rm.[ModuleId] AND rm.[RoleId] = @SupervisorRoleId
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[RoleRoutes] 
        WHERE [RoleId] = @SupervisorRoleId AND [RouteId] = r.[Id]
    );
    
    PRINT 'Rutas asignadas al rol de Supervisor correctamente.';
END

-- Para el rol de Usuario (solo rutas básicas)
IF @UserRoleId IS NOT NULL
BEGIN
    INSERT INTO [dbo].[RoleRoutes] ([Id], [RoleId], [RouteId], [IsActive], [IsDeleted], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
    SELECT 
        NEWID(), 
        @UserRoleId, 
        r.[Id], 
        1, 
        0, 
        'System', 
        GETDATE(), 
        'System', 
        GETDATE()
    FROM [dbo].[Routes] r
    INNER JOIN [dbo].[Modules] m ON r.[ModuleId] = m.[Id]
    WHERE m.[Name] = 'Dashboard'
      AND r.[Name] = 'Inicio'
      AND NOT EXISTS (
        SELECT 1 FROM [dbo].[RoleRoutes] 
        WHERE [RoleId] = @UserRoleId AND [RouteId] = r.[Id]
    );
    
    PRINT 'Rutas asignadas al rol de Usuario correctamente.';
END

PRINT 'Script de migración con roles y permisos ejecutado correctamente.';
