-- Script para crear usuarios de prueba para cada rol

-- Obtener IDs de los roles
DECLARE @AdminRoleId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Roles] WHERE [Name] = 'Administrador');
DECLARE @SupervisorRoleId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Roles] WHERE [Name] = 'Supervisor');
DECLARE @UserRoleId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Roles] WHERE [Name] = 'Usuario');

-- Crear usuarios de prueba si no existen
IF NOT EXISTS (SELECT * FROM [dbo].[Users] WHERE [Username] = 'admin')
BEGIN
    -- Contraseña: Admin123!
    -- Hash generado con BCrypt
    INSERT INTO [dbo].[Users] (
        [Id], 
        [Username], 
        [Email], 
        [PasswordHash], 
        [FirstName], 
        [LastName], 
        [IsActive], 
        [IsLdapUser], 
        [IsDeleted], 
        [CreatedBy], 
        [CreatedAt], 
        [UpdatedBy], 
        [UpdatedAt],
        [Status],
        [UserType]
    )
    VALUES 
        (
            NEWID(), 
            'admin', 
            'admin@example.com', 
            '$2a$11$ysX.rXE3Ixj.enu6QJKdGOmUDJms7.V.8BdOqj7yJHGKZ1.w9j5Iq', -- Admin123!
            'Admin', 
            'User', 
            1, 
            0, 
            0, 
            'System', 
            GETDATE(), 
            'System', 
            GETDATE(),
            0, -- Status = Active
            0  -- UserType = Internal
        ),
        (
            NEWID(), 
            'supervisor', 
            'supervisor@example.com', 
            '$2a$11$ysX.rXE3Ixj.enu6QJKdGOmUDJms7.V.8BdOqj7yJHGKZ1.w9j5Iq', -- Admin123!
            'Supervisor', 
            'User', 
            1, 
            0, 
            0, 
            'System', 
            GETDATE(), 
            'System', 
            GETDATE(),
            0, -- Status = Active
            0  -- UserType = Internal
        ),
        (
            NEWID(), 
            'user', 
            'user@example.com', 
            '$2a$11$ysX.rXE3Ixj.enu6QJKdGOmUDJms7.V.8BdOqj7yJHGKZ1.w9j5Iq', -- Admin123!
            'Regular', 
            'User', 
            1, 
            0, 
            0, 
            'System', 
            GETDATE(), 
            'System', 
            GETDATE(),
            0, -- Status = Active
            0  -- UserType = Internal
        );
    
    PRINT 'Usuarios de prueba creados correctamente.';
END
ELSE
BEGIN
    PRINT 'Los usuarios de prueba ya existen.';
END

-- Asignar roles a los usuarios
DECLARE @AdminUserId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Users] WHERE [Username] = 'admin');
DECLARE @SupervisorUserId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Users] WHERE [Username] = 'supervisor');
DECLARE @RegularUserId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Users] WHERE [Username] = 'user');

-- Asignar rol de Administrador al usuario admin
IF NOT EXISTS (SELECT * FROM [dbo].[UserRoles] WHERE [UserId] = @AdminUserId AND [RoleId] = @AdminRoleId)
BEGIN
    INSERT INTO [dbo].[UserRoles] (
        [Id], 
        [UserId], 
        [RoleId], 
        [IsActive],
        [IsDeleted], 
        [CreatedBy], 
        [CreatedAt], 
        [UpdatedBy], 
        [UpdatedAt]
    )
    VALUES (
        NEWID(), 
        @AdminUserId, 
        @AdminRoleId, 
        1,
        0, 
        'System', 
        GETDATE(), 
        'System', 
        GETDATE()
    );
    
    PRINT 'Rol de Administrador asignado al usuario admin.';
END
ELSE
BEGIN
    PRINT 'El usuario admin ya tiene asignado el rol de Administrador.';
END

-- Asignar rol de Supervisor al usuario supervisor
IF NOT EXISTS (SELECT * FROM [dbo].[UserRoles] WHERE [UserId] = @SupervisorUserId AND [RoleId] = @SupervisorRoleId)
BEGIN
    INSERT INTO [dbo].[UserRoles] (
        [Id], 
        [UserId], 
        [RoleId], 
        [IsActive],
        [IsDeleted], 
        [CreatedBy], 
        [CreatedAt], 
        [UpdatedBy], 
        [UpdatedAt]
    )
    VALUES (
        NEWID(), 
        @SupervisorUserId, 
        @SupervisorRoleId, 
        1,
        0, 
        'System', 
        GETDATE(), 
        'System', 
        GETDATE()
    );
    
    PRINT 'Rol de Supervisor asignado al usuario supervisor.';
END
ELSE
BEGIN
    PRINT 'El usuario supervisor ya tiene asignado el rol de Supervisor.';
END

-- Asignar rol de Usuario al usuario user
IF NOT EXISTS (SELECT * FROM [dbo].[UserRoles] WHERE [UserId] = @RegularUserId AND [RoleId] = @UserRoleId)
BEGIN
    INSERT INTO [dbo].[UserRoles] (
        [Id], 
        [UserId], 
        [RoleId], 
        [IsActive],
        [IsDeleted], 
        [CreatedBy], 
        [CreatedAt], 
        [UpdatedBy], 
        [UpdatedAt]
    )
    VALUES (
        NEWID(), 
        @RegularUserId, 
        @UserRoleId, 
        1,
        0, 
        'System', 
        GETDATE(), 
        'System', 
        GETDATE()
    );
    
    PRINT 'Rol de Usuario asignado al usuario user.';
END
ELSE
BEGIN
    PRINT 'El usuario user ya tiene asignado el rol de Usuario.';
END

PRINT 'Script de creación de usuarios de prueba ejecutado correctamente.';
