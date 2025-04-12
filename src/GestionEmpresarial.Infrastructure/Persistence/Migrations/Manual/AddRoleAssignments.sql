-- Script para asignar módulos y rutas a roles

-- Obtener IDs de los roles
DECLARE @AdminRoleId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Roles] WHERE [Name] = 'Administrador');
DECLARE @SupervisorRoleId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Roles] WHERE [Name] = 'Supervisor');
DECLARE @UserRoleId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Roles] WHERE [Name] = 'Usuario');

-- Asignar todos los módulos al rol de Administrador
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

-- Para el rol de Usuario (solo Dashboard)
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
      AND NOT EXISTS (
        SELECT 1 FROM [dbo].[RoleRoutes] 
        WHERE [RoleId] = @UserRoleId AND [RouteId] = r.[Id]
    );
    
    PRINT 'Rutas asignadas al rol de Usuario correctamente.';
END

PRINT 'Script de asignación de roles ejecutado correctamente.';
