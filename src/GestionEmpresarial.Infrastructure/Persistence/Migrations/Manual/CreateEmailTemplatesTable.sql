-- Script para crear la tabla de plantillas de correo electrónico
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmailTemplates]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[EmailTemplates](
        [Id] [uniqueidentifier] NOT NULL,
        [Name] [nvarchar](100) NOT NULL,
        [Subject] [nvarchar](200) NOT NULL,
        [HtmlBody] [nvarchar](max) NOT NULL,
        [PlainTextBody] [nvarchar](max) NULL,
        [Description] [nvarchar](500) NULL,
        [Type] [nvarchar](50) NOT NULL,
        [AvailableVariables] [nvarchar](max) NULL,
        [IsActive] [bit] NOT NULL DEFAULT 1,
        [IsDeleted] [bit] NOT NULL DEFAULT 0,
        [IsDefault] [bit] NOT NULL DEFAULT 0,
        [CreatedBy] [nvarchar](450) NULL,
        [CreatedAt] [datetime2](7) NOT NULL,
        [UpdatedBy] [nvarchar](450) NULL,
        [UpdatedAt] [datetime2](7) NULL,
        CONSTRAINT [PK_EmailTemplates] PRIMARY KEY CLUSTERED 
        (
            [Id] ASC
        )
    )
    
    PRINT 'Tabla EmailTemplates creada correctamente.'
END
ELSE
BEGIN
    PRINT 'La tabla EmailTemplates ya existe.'
END

-- Crear índices para mejorar el rendimiento de las consultas
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmailTemplates_Type' AND object_id = OBJECT_ID('EmailTemplates'))
BEGIN
    CREATE INDEX [IX_EmailTemplates_Type] ON [dbo].[EmailTemplates]([Type])
    PRINT 'Índice IX_EmailTemplates_Type creado correctamente.'
END
ELSE
BEGIN
    PRINT 'El índice IX_EmailTemplates_Type ya existe.'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmailTemplates_IsDefault' AND object_id = OBJECT_ID('EmailTemplates'))
BEGIN
    CREATE INDEX [IX_EmailTemplates_IsDefault] ON [dbo].[EmailTemplates]([IsDefault])
    PRINT 'Índice IX_EmailTemplates_IsDefault creado correctamente.'
END
ELSE
BEGIN
    PRINT 'El índice IX_EmailTemplates_IsDefault ya existe.'
END

-- Insertar plantillas predeterminadas si no existen
IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[EmailTemplates] WHERE [Type] = 'AccountActivation' AND [IsDefault] = 1)
BEGIN
    INSERT INTO [dbo].[EmailTemplates] (
        [Id], [Name], [Subject], [HtmlBody], [PlainTextBody], [Description], [Type], 
        [AvailableVariables], [IsActive], [IsDeleted], [IsDefault], [CreatedBy], [CreatedAt]
    )
    VALUES (
        NEWID(),
        'Activación de Cuenta',
        'Activa tu cuenta en GestionEmpresarial',
        N'<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Activación de Cuenta</title>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background-color: #4CAF50; color: white; padding: 10px; text-align: center; }
        .content { padding: 20px; background-color: #f9f9f9; }
        .button { display: inline-block; background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; }
        .code-box { background-color: #f1f1f1; border: 1px solid #ddd; padding: 10px; text-align: center; font-size: 24px; letter-spacing: 5px; margin: 20px 0; }
        .footer { margin-top: 20px; text-align: center; font-size: 12px; color: #777; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>GestionEmpresarial</h1>
        </div>
        <div class="content">
            <h2>¡Bienvenido a GestionEmpresarial, {{Username}}!</h2>
            <p>Gracias por registrarte. Para activar tu cuenta, haz clic en el siguiente botón:</p>
            <p style="text-align: center;">
                <a href="{{ActivationUrl}}" class="button">Activar mi cuenta</a>
            </p>
            <p>Si el botón no funciona, copia y pega el siguiente enlace en tu navegador:</p>
            <p>{{ActivationUrl}}</p>
            
            <h3>¿Problemas con el enlace?</h3>
            <p>También puedes activar tu cuenta usando el siguiente código de activación:</p>
            <div class="code-box">{{ActivationCode}}</div>
            <p>Para usar este código, visita <a href="{{ActivationCodeUrl}}">{{ActivationCodeUrl}}</a> e ingresa el código mostrado arriba.</p>
            
            <p>Si no has solicitado esta cuenta, puedes ignorar este correo.</p>
        </div>
        <div class="footer">
            <p>Este es un correo automático, por favor no respondas a este mensaje.</p>
            <p>&copy; 2025 GestionEmpresarial. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>',
        'Bienvenido a GestionEmpresarial, {{Username}}! Para activar tu cuenta, visita: {{ActivationUrl}} o usa el código de activación: {{ActivationCode}}',
        'Plantilla para enviar correos de activación de cuenta a nuevos usuarios',
        'AccountActivation',
        '{"Username":"Nombre de usuario","ActivationToken":"Token de activación","ActivationUrl":"URL completa para activar la cuenta","ActivationCode":"Código de 6 dígitos para activar la cuenta","ActivationCodeUrl":"URL para activar la cuenta con código"}',
        1, 0, 1, 'System', GETDATE()
    );
    
    PRINT 'Plantilla de Activación de Cuenta creada correctamente.'
END
ELSE
BEGIN
    PRINT 'La plantilla de Activación de Cuenta ya existe.'
END

IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[EmailTemplates] WHERE [Type] = 'RegistrationConfirmation' AND [IsDefault] = 1)
BEGIN
    INSERT INTO [dbo].[EmailTemplates] (
        [Id], [Name], [Subject], [HtmlBody], [PlainTextBody], [Description], [Type], 
        [AvailableVariables], [IsActive], [IsDeleted], [IsDefault], [CreatedBy], [CreatedAt]
    )
    VALUES (
        NEWID(),
        'Confirmación de Registro',
        'Registro exitoso en GestionEmpresarial',
        N'<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Registro Exitoso</title>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background-color: #2196F3; color: white; padding: 10px; text-align: center; }
        .content { padding: 20px; background-color: #f9f9f9; }
        .button { display: inline-block; background-color: #2196F3; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; }
        .footer { margin-top: 20px; text-align: center; font-size: 12px; color: #777; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>GestionEmpresarial</h1>
        </div>
        <div class="content">
            <h2>¡Bienvenido a GestionEmpresarial, {{Username}}!</h2>
            <p>Tu cuenta ha sido creada exitosamente.</p>
            <p>Ya puedes iniciar sesión y comenzar a utilizar nuestro sistema.</p>
            <p style="text-align: center;">
                <a href="{{LoginUrl}}" class="button">Iniciar Sesión</a>
            </p>
            <p>Gracias por confiar en nosotros.</p>
        </div>
        <div class="footer">
            <p>Este es un correo automático, por favor no respondas a este mensaje.</p>
            <p>&copy; 2025 GestionEmpresarial. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>',
        'Bienvenido a GestionEmpresarial, {{Username}}! Tu cuenta ha sido creada exitosamente. Ya puedes iniciar sesión y comenzar a utilizar nuestro sistema.',
        'Plantilla para enviar correos de confirmación de registro a usuarios',
        'RegistrationConfirmation',
        '{"Username":"Nombre de usuario","LoginUrl":"URL para iniciar sesión"}',
        1, 0, 1, 'System', GETDATE()
    );
    
    PRINT 'Plantilla de Confirmación de Registro creada correctamente.'
END
ELSE
BEGIN
    PRINT 'La plantilla de Confirmación de Registro ya existe.'
END

IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[EmailTemplates] WHERE [Type] = 'PasswordReset' AND [IsDefault] = 1)
BEGIN
    INSERT INTO [dbo].[EmailTemplates] (
        [Id], [Name], [Subject], [HtmlBody], [PlainTextBody], [Description], [Type], 
        [AvailableVariables], [IsActive], [IsDeleted], [IsDefault], [CreatedBy], [CreatedAt]
    )
    VALUES (
        NEWID(),
        'Restablecimiento de Contraseña',
        'Restablecimiento de contraseña en GestionEmpresarial',
        N'<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Restablecimiento de Contraseña</title>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background-color: #FF9800; color: white; padding: 10px; text-align: center; }
        .content { padding: 20px; background-color: #f9f9f9; }
        .button { display: inline-block; background-color: #FF9800; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; }
        .footer { margin-top: 20px; text-align: center; font-size: 12px; color: #777; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>GestionEmpresarial</h1>
        </div>
        <div class="content">
            <h2>Hola, {{Username}}</h2>
            <p>Has solicitado restablecer tu contraseña. Haz clic en el siguiente botón para crear una nueva contraseña:</p>
            <p style="text-align: center;">
                <a href="{{ResetUrl}}" class="button">Restablecer mi contraseña</a>
            </p>
            <p>Si el botón no funciona, copia y pega el siguiente enlace en tu navegador:</p>
            <p>{{ResetUrl}}</p>
            <p>Si no has solicitado este cambio, puedes ignorar este correo.</p>
            <p>Este enlace expirará en 24 horas por razones de seguridad.</p>
        </div>
        <div class="footer">
            <p>Este es un correo automático, por favor no respondas a este mensaje.</p>
            <p>&copy; 2025 GestionEmpresarial. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>',
        'Hola, {{Username}}. Has solicitado restablecer tu contraseña. Visita: {{ResetUrl}} para crear una nueva contraseña. Este enlace expirará en 24 horas.',
        'Plantilla para enviar correos de restablecimiento de contraseña',
        'PasswordReset',
        '{"Username":"Nombre de usuario","ResetToken":"Token de restablecimiento","ResetUrl":"URL completa para restablecer la contraseña"}',
        1, 0, 1, 'System', GETDATE()
    );
    
    PRINT 'Plantilla de Restablecimiento de Contraseña creada correctamente.'
END
ELSE
BEGIN
    PRINT 'La plantilla de Restablecimiento de Contraseña ya existe.'
END

IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[EmailTemplates] WHERE [Type] = 'AccountUpdated' AND [IsDefault] = 1)
BEGIN
    INSERT INTO [dbo].[EmailTemplates] (
        [Id], [Name], [Subject], [HtmlBody], [PlainTextBody], [Description], [Type], 
        [AvailableVariables], [IsActive], [IsDeleted], [IsDefault], [CreatedBy], [CreatedAt]
    )
    VALUES (
        NEWID(),
        'Actualización de Cuenta',
        'Actualización de cuenta en GestionEmpresarial',
        N'<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Actualización de Cuenta</title>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background-color: #673AB7; color: white; padding: 10px; text-align: center; }
        .content { padding: 20px; background-color: #f9f9f9; }
        .button { display: inline-block; background-color: #673AB7; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; }
        .footer { margin-top: 20px; text-align: center; font-size: 12px; color: #777; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>GestionEmpresarial</h1>
        </div>
        <div class="content">
            <h2>Hola, {{Username}}</h2>
            <p>Te informamos que tu cuenta ha sido actualizada recientemente.</p>
            <p>Si no has realizado estos cambios, por favor contacta con nuestro equipo de soporte inmediatamente.</p>
            <p style="text-align: center;">
                <a href="{{SupportUrl}}" class="button">Contactar Soporte</a>
            </p>
            <p>Gracias por confiar en nosotros.</p>
        </div>
        <div class="footer">
            <p>Este es un correo automático, por favor no respondas a este mensaje.</p>
            <p>&copy; 2025 GestionEmpresarial. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>',
        'Hola, {{Username}}. Te informamos que tu cuenta ha sido actualizada recientemente. Si no has realizado estos cambios, por favor contacta con nuestro equipo de soporte inmediatamente.',
        'Plantilla para notificar a los usuarios sobre actualizaciones en su cuenta',
        'AccountUpdated',
        '{"Username":"Nombre de usuario","SupportUrl":"URL para contactar al soporte"}',
        1, 0, 1, 'System', GETDATE()
    );
    
    PRINT 'Plantilla de Actualización de Cuenta creada correctamente.'
END
ELSE
BEGIN
    PRINT 'La plantilla de Actualización de Cuenta ya existe.'
END
