-- Verificar si existe la plantilla de activación de cuenta
IF NOT EXISTS (SELECT 1 FROM EmailTemplates WHERE Type = 'AccountActivation' AND IsDefault = 1)
BEGIN
    -- Insertar la plantilla de activación de cuenta
    INSERT INTO EmailTemplates (
        Id, 
        Name, 
        Subject, 
        HtmlBody, 
        PlainTextBody, 
        Description, 
        Type, 
        IsActive, 
        IsDefault, 
        AvailableVariables,
        IsDeleted,
        CreatedBy,
        CreatedAt,
        UpdatedBy,
        UpdatedAt
    )
    VALUES (
        NEWID(), 
        'Activación de Cuenta', 
        'Activa tu cuenta en GestionEmpresarial', 
        N'<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>Activación de Cuenta</title>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background-color: #0056b3; color: white; padding: 10px 20px; text-align: center; }
        .content { padding: 20px; background-color: #f9f9f9; }
        .footer { text-align: center; margin-top: 20px; font-size: 12px; color: #666; }
        .button { display: inline-block; background-color: #0056b3; color: white; padding: 10px 20px; text-decoration: none; border-radius: 4px; }
        .code-box { background-color: #e9ecef; padding: 10px; text-align: center; font-size: 24px; font-weight: bold; letter-spacing: 5px; margin: 20px 0; border-radius: 4px; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>Bienvenido a GestionEmpresarial</h1>
        </div>
        <div class="content">
            <h2>¡Hola {{Username}}!</h2>
            <p>Gracias por registrarte en nuestro sistema. Para activar tu cuenta, tienes dos opciones:</p>
            
            <h3>Opción 1: Haz clic en el siguiente enlace</h3>
            <p><a href="{{ActivationUrl}}" class="button">Activar mi cuenta</a></p>
            
            <h3>Opción 2: Usa el código de activación</h3>
            <p>Si el enlace no funciona, puedes usar el siguiente código de activación:</p>
            <div class="code-box">{{ActivationCode}}</div>
            <p>Para activar tu cuenta con este código, visita <a href="{{ActivationCodeUrl}}">nuestra página de activación</a> e ingresa tu correo electrónico y el código.</p>
            
            <p>Si no solicitaste esta cuenta, puedes ignorar este correo electrónico.</p>
        </div>
        <div class="footer">
            <p>Este es un correo electrónico automático, por favor no respondas a este mensaje.</p>
            <p>&copy; 2025 GestionEmpresarial. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>', 
        'Hola {{Username}},

Gracias por registrarte en GestionEmpresarial. Para activar tu cuenta, tienes dos opciones:

Opción 1: Visita el siguiente enlace:
{{ActivationUrl}}

Opción 2: Usa el siguiente código de activación: {{ActivationCode}}
Para activar tu cuenta con este código, visita {{ActivationCodeUrl}} e ingresa tu correo electrónico y el código.

Si no solicitaste esta cuenta, puedes ignorar este correo electrónico.

Este es un correo electrónico automático, por favor no respondas a este mensaje.

© 2025 GestionEmpresarial. Todos los derechos reservados.', 
        'Plantilla para correos de activación de cuenta', 
        'AccountActivation', 
        1, 
        1, 
        'Username, ActivationToken, ActivationUrl, ActivationCode, ActivationCodeUrl',
        0,
        'System',
        GETDATE(),
        'System',
        GETDATE()
    )
    PRINT 'Plantilla de activación de cuenta creada correctamente.'
END
ELSE
BEGIN
    -- Actualizar la plantilla existente
    UPDATE EmailTemplates
    SET 
        Subject = 'Activa tu cuenta en GestionEmpresarial',
        HtmlBody = N'<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>Activación de Cuenta</title>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background-color: #0056b3; color: white; padding: 10px 20px; text-align: center; }
        .content { padding: 20px; background-color: #f9f9f9; }
        .footer { text-align: center; margin-top: 20px; font-size: 12px; color: #666; }
        .button { display: inline-block; background-color: #0056b3; color: white; padding: 10px 20px; text-decoration: none; border-radius: 4px; }
        .code-box { background-color: #e9ecef; padding: 10px; text-align: center; font-size: 24px; font-weight: bold; letter-spacing: 5px; margin: 20px 0; border-radius: 4px; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>Bienvenido a GestionEmpresarial</h1>
        </div>
        <div class="content">
            <h2>¡Hola {{Username}}!</h2>
            <p>Gracias por registrarte en nuestro sistema. Para activar tu cuenta, tienes dos opciones:</p>
            
            <h3>Opción 1: Haz clic en el siguiente enlace</h3>
            <p><a href="{{ActivationUrl}}" class="button">Activar mi cuenta</a></p>
            
            <h3>Opción 2: Usa el código de activación</h3>
            <p>Si el enlace no funciona, puedes usar el siguiente código de activación:</p>
            <div class="code-box">{{ActivationCode}}</div>
            <p>Para activar tu cuenta con este código, visita <a href="{{ActivationCodeUrl}}">nuestra página de activación</a> e ingresa tu correo electrónico y el código.</p>
            
            <p>Si no solicitaste esta cuenta, puedes ignorar este correo electrónico.</p>
        </div>
        <div class="footer">
            <p>Este es un correo electrónico automático, por favor no respondas a este mensaje.</p>
            <p>&copy; 2025 GestionEmpresarial. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>',
        PlainTextBody = 'Hola {{Username}},

Gracias por registrarte en GestionEmpresarial. Para activar tu cuenta, tienes dos opciones:

Opción 1: Visita el siguiente enlace:
{{ActivationUrl}}

Opción 2: Usa el siguiente código de activación: {{ActivationCode}}
Para activar tu cuenta con este código, visita {{ActivationCodeUrl}} e ingresa tu correo electrónico y el código.

Si no solicitaste esta cuenta, puedes ignorar este correo electrónico.

Este es un correo electrónico automático, por favor no respondas a este mensaje.

© 2025 GestionEmpresarial. Todos los derechos reservados.',
        AvailableVariables = 'Username, ActivationToken, ActivationUrl, ActivationCode, ActivationCodeUrl',
        UpdatedBy = 'System',
        UpdatedAt = GETDATE()
    WHERE Type = 'AccountActivation' AND IsDefault = 1
    PRINT 'Plantilla de activación de cuenta actualizada correctamente.'
END
