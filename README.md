# Sistema de Gestión Empresarial

Este proyecto es un sistema de gestión empresarial desarrollado con ASP.NET Core, siguiendo los principios de Arquitectura Limpia (Clean Architecture).

## Estructura del Proyecto

El proyecto está organizado en capas siguiendo los principios de Arquitectura Limpia:

- **GestionEmpresarial.Domain**: Contiene las entidades de dominio y lógica de negocio central.
- **GestionEmpresarial.Application**: Contiene la lógica de aplicación, interfaces y casos de uso.
- **GestionEmpresarial.Infrastructure**: Implementa las interfaces definidas en la capa de aplicación.
- **GestionEmpresarial.API**: Capa de presentación que expone la API REST.

## Características Implementadas

- Autenticación JWT
- Gestión de usuarios y roles
- Arquitectura Limpia
- Patrón Mediator con MediatR
- Entity Framework Core para acceso a datos
- Validación de datos robusta con FluentValidation
- Paginación para endpoints que devuelven listas grandes
- Filtrado y ordenación avanzada
- Seguridad mejorada con hash de contraseñas BCrypt
- Envío de correos electrónicos para notificaciones de usuarios
- Activación de cuentas mediante tokens para verificación de correo electrónico
- Autenticación híbrida con soporte para LDAP/Active Directory

## Requisitos Previos

- .NET 9.0 SDK
- SQL Server (LocalDB o instancia completa)
- Visual Studio 2022 o Visual Studio Code

## Configuración

1. Clonar el repositorio
2. Configurar la cadena de conexión en `appsettings.json`
3. Configurar las credenciales de correo electrónico en `appsettings.json`:
   ```json
   "EmailSettings": {
     "SmtpServer": "smtp.gmail.com",
     "SmtpPort": 587,
     "EnableSsl": true,
     "SmtpUsername": "tu-correo@gmail.com",
     "SmtpPassword": "tu-contraseña-de-aplicación",
     "FromEmail": "tu-correo@gmail.com",
     "FromName": "Sistema de Gestión Empresarial"
   }
   ```
   > **Nota**: Para Gmail, debes usar una contraseña de aplicación generada en la configuración de seguridad de Google.

4. **Configuración de LDAP/Active Directory** (opcional):
   ```json
   "LdapSettings": {
     "Server": "mintrabajo.loc",
     "Port": 389,
     "UseSSL": false,
     "BindDN": "CN=ServiceAccount,OU=ServiceAccounts,DC=mintrabajo,DC=loc",
     "BindPassword": "ServiceAccountPassword",
     "SearchBase": "DC=mintrabajo,DC=loc",
     "SearchFilter": "(&(objectClass=user)(sAMAccountName={0}))",
     "Enabled": true,
     "DefaultRole": "Usuario",
     "EmailAttribute": "mail",
     "DisplayNameAttribute": "displayName",
     "UsernameSuffix": "@mintrabajo.loc"
   }
   ```
   > **Nota**: Si no deseas utilizar la autenticación LDAP, establece `Enabled` en `false`.

5. Ejecutar migraciones de Entity Framework Core:

```bash
dotnet ef database update
```

## Ejecución

Para ejecutar el proyecto:

```bash
cd src/GestionEmpresarial.API
dotnet run
```

La API estará disponible en: http://localhost:5000

## Endpoints de la API

### Autenticación

- `POST /api/auth/login`: Iniciar sesión (soporta autenticación local y LDAP)
- `POST /api/auth/refresh-token`: Renovar token JWT
- `POST /api/auth/revoke-token`: Revocar token JWT
- `GET /api/auth/test`: Endpoint de prueba (no requiere autenticación)
- `GET /api/auth/protected`: Endpoint protegido (requiere autenticación)

### Activación de Cuenta

- `POST /api/account/activate`: Activar cuenta de usuario con token
- `POST /api/account/resend-activation`: Reenviar correo de activación
- `POST /api/account/generate-activation-token/{userId}`: Generar nuevo token de activación (solo administradores)

## Pruebas

Se incluye un script PowerShell para probar la autenticación:

```bash
./test-auth.ps1
```

## Próximos Pasos

- Implementar módulo de empresas
- Implementar módulo de sucursales
- Implementar módulo de empleados
- Agregar validación de datos
- Implementar pruebas unitarias y de integración
- Mejorar la interfaz de usuario para la activación de cuentas
- Implementar sincronización periódica de usuarios LDAP
