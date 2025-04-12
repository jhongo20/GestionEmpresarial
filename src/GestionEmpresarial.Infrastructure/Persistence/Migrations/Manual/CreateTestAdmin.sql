-- Script para crear un usuario de prueba con contraseña simple
-- La contraseña será: test123

-- Primero, verificamos si el usuario ya existe
IF NOT EXISTS (SELECT * FROM [dbo].[Users] WHERE [Username] = 'testadmin')
BEGIN
    -- Crear el usuario testadmin
    DECLARE @UserId UNIQUEIDENTIFIER = NEWID();
    
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
        @UserId, 
        'testadmin', 
        'testadmin@example.com', 
        -- Hash para 'test123'
        '$2a$11$K3g65rFCKMFQOGZjX/KkI.Vs19.qAB9SHGKmWQxb7.9QwiYyFMmHy', 
        'Test', 
        'Admin', 
        1, -- IsActive
        0, -- IsLdapUser
        0, -- IsDeleted
        'System', 
        GETDATE(), 
        'System', 
        GETDATE(),
        0, -- Status = Active
        0  -- UserType = Internal
    );
    
    -- Obtener el ID del rol Administrador
    DECLARE @AdminRoleId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Roles] WHERE [Name] = 'Administrador');
    
    -- Asignar el rol de Administrador al usuario testadmin
    IF @AdminRoleId IS NOT NULL
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
            @UserId, 
            @AdminRoleId, 
            1, -- IsActive
            0, -- IsDeleted
            'System', 
            GETDATE(), 
            'System', 
            GETDATE()
        );
        
        PRINT 'Rol de Administrador asignado al usuario testadmin.';
    END
    ELSE
    BEGIN
        PRINT 'No se pudo asignar el rol de Administrador porque no existe.';
    END
    
    PRINT 'Usuario testadmin creado correctamente.';
END
ELSE
BEGIN
    PRINT 'El usuario testadmin ya existe.';
END
