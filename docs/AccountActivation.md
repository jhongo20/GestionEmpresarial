# Documentación de Activación de Cuentas

## Descripción General

El sistema de activación de cuentas permite verificar que los usuarios se registren con direcciones de correo electrónico válidas. Cuando un usuario se registra, su cuenta se crea en estado inactivo y se envía un correo electrónico de activación con dos opciones para activar la cuenta:

1. Un enlace de activación con un token único
2. Un código numérico de 6 dígitos que puede ser ingresado manualmente

## Flujo de Activación

### Registro de Usuario

1. El usuario se registra a través del endpoint `POST /api/users`
2. El sistema crea el usuario en estado inactivo (`IsActive = false` y `EmailConfirmed = false`)
3. Se genera un token de activación único (GUID) y se almacena en el campo `ActivationToken` del usuario
4. Se establece una fecha de expiración para el token (24 horas) en el campo `ActivationTokenExpires`
5. Se envía un correo electrónico al usuario con:
   - Un enlace de activación que contiene el token
   - Un código numérico de 6 dígitos generado a partir del token

### Activación mediante Token

1. El usuario hace clic en el enlace de activación en su correo electrónico
2. El enlace dirige al usuario a la página de activación con el token como parámetro
3. La aplicación frontend envía una solicitud al endpoint `POST /api/account-activation/activate-with-token`
4. El sistema valida el token y activa la cuenta si es válido

### Activación mediante Código

1. Si el enlace no funciona, el usuario puede usar el código numérico de 6 dígitos
2. El usuario visita la página de activación manual e ingresa:
   - Su dirección de correo electrónico
   - El código de activación de 6 dígitos
3. La aplicación frontend envía una solicitud al endpoint `POST /api/account-activation/activate-with-code`
4. El sistema valida el código y activa la cuenta si es válido

## Endpoints de Activación

### Activar cuenta con token

```
POST /api/account-activation/activate-with-token
```

**Cuerpo de la solicitud:**
```json
{
  "token": "string"
}
```

**Respuesta exitosa:**
```json
{
  "message": "Cuenta activada correctamente."
}
```

### Activar cuenta con código

```
POST /api/account-activation/activate-with-code
```

**Cuerpo de la solicitud:**
```json
{
  "email": "usuario@ejemplo.com",
  "code": "123456"
}
```

**Respuesta exitosa:**
```json
{
  "message": "Cuenta activada correctamente."
}
```

## Plantilla de Correo Electrónico

El sistema utiliza una plantilla HTML almacenada en la base de datos para el correo electrónico de activación. La plantilla incluye:

- Saludo personalizado con el nombre de usuario
- Instrucciones claras para ambos métodos de activación
- Enlace de activación con el token
- Código de activación de 6 dígitos destacado visualmente
- Enlace a la página de activación manual

## Generación del Código de Activación

El código de activación de 6 dígitos se genera a partir del token de activación utilizando el siguiente algoritmo:

1. Se calcula un hash SHA-256 del token
2. Se convierten los primeros bytes del hash a un número entero
3. Se toman los últimos 6 dígitos del número (asegurando que sea un número de 6 dígitos)

Este algoritmo garantiza que:
- El mismo token siempre generará el mismo código
- El código es difícil de predecir sin conocer el token original
- El código siempre tiene exactamente 6 dígitos

## Consideraciones de Seguridad

- Los tokens de activación expiran después de 24 horas
- Después de la activación exitosa, el token se elimina de la base de datos
- Se utilizan registros detallados para rastrear intentos de activación
- Se implementan límites de intentos para prevenir ataques de fuerza bruta

## Solución de Problemas

### El usuario no recibe el correo de activación

1. Verificar que la dirección de correo electrónico sea correcta
2. Comprobar la configuración SMTP en `appsettings.json`
3. Revisar los registros de la aplicación para errores de envío de correo
4. Verificar que la plantilla de correo electrónico exista en la base de datos

### Error al activar la cuenta

1. Verificar que el token o código no haya expirado
2. Comprobar que la cuenta no esté ya activada
3. Verificar que el usuario exista y no esté marcado como eliminado
4. Revisar los registros de la aplicación para errores específicos

## Tablas de Base de Datos

### Columnas Relevantes en la Tabla Users

| Columna | Tipo | Descripción |
|---------|------|-------------|
| ActivationToken | nvarchar(max) | Token único para activación de cuenta |
| ActivationTokenExpires | datetime2 | Fecha y hora de expiración del token |
| EmailConfirmed | bit | Indica si el correo electrónico ha sido confirmado |
| IsActive | bit | Indica si la cuenta está activa |

### Tabla EmailTemplates

| Columna | Tipo | Descripción |
|---------|------|-------------|
| Id | uniqueidentifier | Identificador único de la plantilla |
| Name | nvarchar(100) | Nombre de la plantilla |
| Type | nvarchar(50) | Tipo de plantilla (ej. "AccountActivation") |
| Subject | nvarchar(200) | Asunto del correo electrónico |
| HtmlBody | nvarchar(max) | Cuerpo HTML del correo electrónico |
| PlainTextBody | nvarchar(max) | Versión de texto plano del correo |
| IsDefault | bit | Indica si es la plantilla predeterminada para su tipo |
| IsActive | bit | Indica si la plantilla está activa |
