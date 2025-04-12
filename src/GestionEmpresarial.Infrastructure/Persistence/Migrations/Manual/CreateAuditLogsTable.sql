-- Script para crear la tabla de auditoría
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuditLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AuditLogs](
        [Id] [uniqueidentifier] NOT NULL,
        [UserId] [nvarchar](50) NULL,
        [UserName] [nvarchar](100) NULL,
        [Type] [nvarchar](50) NULL,
        [TableName] [nvarchar](100) NULL,
        [DateTime] [datetime2](7) NOT NULL,
        [OldValues] [nvarchar](max) NULL,
        [NewValues] [nvarchar](max) NULL,
        [AffectedColumns] [nvarchar](max) NULL,
        [PrimaryKey] [nvarchar](100) NULL,
        [Action] [nvarchar](50) NULL,
        [IpAddress] [nvarchar](50) NULL,
        [UserAgent] [nvarchar](500) NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY CLUSTERED 
        (
            [Id] ASC
        )
    )
    
    PRINT 'Tabla AuditLogs creada correctamente.'
END
ELSE
BEGIN
    PRINT 'La tabla AuditLogs ya existe.'
END

-- Crear índices para mejorar el rendimiento de las consultas
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_UserId' AND object_id = OBJECT_ID('AuditLogs'))
BEGIN
    CREATE INDEX [IX_AuditLogs_UserId] ON [dbo].[AuditLogs]([UserId])
    PRINT 'Índice IX_AuditLogs_UserId creado correctamente.'
END
ELSE
BEGIN
    PRINT 'El índice IX_AuditLogs_UserId ya existe.'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_TableName' AND object_id = OBJECT_ID('AuditLogs'))
BEGIN
    CREATE INDEX [IX_AuditLogs_TableName] ON [dbo].[AuditLogs]([TableName])
    PRINT 'Índice IX_AuditLogs_TableName creado correctamente.'
END
ELSE
BEGIN
    PRINT 'El índice IX_AuditLogs_TableName ya existe.'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_DateTime' AND object_id = OBJECT_ID('AuditLogs'))
BEGIN
    CREATE INDEX [IX_AuditLogs_DateTime] ON [dbo].[AuditLogs]([DateTime])
    PRINT 'Índice IX_AuditLogs_DateTime creado correctamente.'
END
ELSE
BEGIN
    PRINT 'El índice IX_AuditLogs_DateTime ya existe.'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_Action' AND object_id = OBJECT_ID('AuditLogs'))
BEGIN
    CREATE INDEX [IX_AuditLogs_Action] ON [dbo].[AuditLogs]([Action])
    PRINT 'Índice IX_AuditLogs_Action creado correctamente.'
END
ELSE
BEGIN
    PRINT 'El índice IX_AuditLogs_Action ya existe.'
END

-- Insertar algunos registros de auditoría de ejemplo
IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[AuditLogs])
BEGIN
    INSERT INTO [dbo].[AuditLogs] ([Id], [UserId], [UserName], [Type], [TableName], [DateTime], [OldValues], [NewValues], [AffectedColumns], [PrimaryKey], [Action], [IpAddress], [UserAgent])
    VALUES 
    (NEWID(), 'System', 'System', 'System', 'System', GETDATE(), NULL, 'Sistema de auditoría inicializado', NULL, NULL, 'Initialize', '127.0.0.1', 'System'),
    (NEWID(), 'admin', 'Administrador', 'Security', 'auth', DATEADD(MINUTE, -5, GETDATE()), NULL, '{"success":true}', NULL, NULL, 'Login', '127.0.0.1', 'Mozilla/5.0'),
    (NEWID(), 'admin', 'Administrador', 'Create', 'Users', DATEADD(MINUTE, -4, GETDATE()), NULL, '{"id":"00000000-0000-0000-0000-000000000001","username":"testuser","email":"test@example.com"}', NULL, 'testuser', 'Create', '127.0.0.1', 'Mozilla/5.0')
    
    PRINT 'Registros de ejemplo insertados correctamente.'
END
ELSE
BEGIN
    PRINT 'Ya existen registros en la tabla AuditLogs.'
END
