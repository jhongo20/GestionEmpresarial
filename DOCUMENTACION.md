# Sistema de Gestión Empresarial - Documentación Técnica

## Descripción General

El Sistema de Gestión Empresarial es una aplicación web desarrollada con ASP.NET Core 8.0 siguiendo los principios de Arquitectura Limpia (Clean Architecture). Esta aplicación proporciona una API RESTful para la gestión de empresas, sucursales, empleados y usuarios, con un sistema de autenticación basado en JWT (JSON Web Tokens).

## Estructura del Proyecto

El proyecto está organizado en cuatro capas principales, siguiendo los principios de Arquitectura Limpia:

### 1. GestionEmpresarial.Domain

Contiene las entidades de dominio, enumeraciones y lógica de negocio central. Esta capa no tiene dependencias externas.

**Entidades principales:**
- `User`: Representa a los usuarios del sistema
- `Role`: Representa los roles de usuario
- `Permission`: Representa los permisos del sistema
- `UserRole`: Relación entre usuarios y roles
- `RolePermission`: Relación entre roles y permisos
- `RefreshToken`: Gestiona los tokens de actualización para la autenticación
- `UserSession`: Registra las sesiones de usuario

**Enumeraciones:**
- `UserStatus`: Define los estados posibles de un usuario (Active, Inactive, Locked, Suspended)
- `UserType`: Define los tipos de usuario (Internal, External, LdapUser)

**Clases base:**
- `AuditableEntity`: Clase base para entidades que requieren auditoría (CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)

### 2. GestionEmpresarial.Application

Contiene la lógica de aplicación, interfaces y casos de uso. Esta capa depende únicamente de la capa de dominio.

**Interfaces principales:**
- `IApplicationDbContext`: Define las operaciones de acceso a datos
- `ICurrentUserService`: Proporciona información sobre el usuario actual
- `IIdentityService`: Define los métodos de autenticación y gestión de usuarios
- `IDateTime`: Proporciona la fecha y hora actual

**Modelos:**
- `Result`: Encapsula el resultado de una operación
- `AuthenticationResponse`: Encapsula la información de autenticación devuelta al cliente

### 3. GestionEmpresarial.Infrastructure

Implementa las interfaces definidas en la capa de aplicación. Esta capa depende de las capas de dominio y aplicación.

**Componentes principales:**
- `ApplicationDbContext`: Implementación de Entity Framework Core para acceso a datos
- `IdentityService`: Implementación de los servicios de autenticación y gestión de usuarios
- `JwtGenerator`: Generador de tokens JWT
- `DateTimeService`: Proporciona la fecha y hora actual

**Configuraciones:**
- Configuraciones de Entity Framework Core para cada entidad
- Configuración de autenticación JWT
- Inicialización de la base de datos con datos semilla

### 4. GestionEmpresarial.API

Capa de presentación que expone la API RESTful. Esta capa depende de todas las demás capas.

**Controladores:**
- `AuthController`: Gestiona la autenticación y autorización

**Configuración:**
- Configuración de Swagger con soporte para autenticación JWT
- Configuración de CORS
- Configuración de inyección de dependencias

## Autenticación y Autorización

El sistema utiliza autenticación basada en JWT (JSON Web Tokens) para proteger los endpoints de la API.

### Flujo de Autenticación

1. El usuario envía sus credenciales (nombre de usuario y contraseña) al endpoint `/api/auth/login`
2. El sistema valida las credenciales y, si son correctas, genera un token JWT y un token de actualización
3. El token JWT se utiliza para autenticar solicitudes posteriores incluyéndolo en el encabezado `Authorization`
4. Cuando el token JWT expira, el usuario puede obtener un nuevo token utilizando el token de actualización en el endpoint `/api/auth/refresh-token`
5. El usuario puede cerrar sesión utilizando el endpoint `/api/auth/revoke-token`, lo que invalida el token de actualización

### Configuración JWT

La configuración de JWT se encuentra en el archivo `appsettings.json`:

```json
"JwtSettings": {
  "Key": "S3cr3tK3yF0rG3st10nEmpr3s4r14lApl1c4t10n",
  "Issuer": "GestionEmpresarial",
  "Audience": "GestionEmpresarialClients",
  "ExpiryMinutes": 60
}
```

## Base de Datos

El sistema utiliza Entity Framework Core como ORM (Object-Relational Mapping) para interactuar con la base de datos SQL Server.

### Migraciones

Las migraciones de Entity Framework Core se utilizan para crear y actualizar el esquema de la base de datos. Las migraciones se encuentran en el directorio `GestionEmpresarial.Infrastructure/Persistence/Migrations`.

### Datos Semilla

El sistema incluye datos semilla para inicializar la base de datos con:
- Roles predefinidos (Admin, Manager, Employee)
- Permisos para diferentes operaciones (ver, crear, editar, eliminar)
- Usuarios de prueba con diferentes roles

## Cómo Ejecutar el Proyecto

### Requisitos Previos

- .NET 8.0 SDK
- SQL Server (LocalDB o instancia completa)
- Visual Studio 2022 o Visual Studio Code

### Pasos para Ejecutar

1. Clonar el repositorio
2. Configurar la cadena de conexión en `appsettings.json` si es necesario
3. Abrir una terminal en el directorio del proyecto
4. Ejecutar las migraciones de Entity Framework Core:
   ```
   dotnet ef database update --project src\GestionEmpresarial.Infrastructure --startup-project src\GestionEmpresarial.API
   ```
5. Iniciar la aplicación:
   ```
   cd src\GestionEmpresarial.API
   dotnet run
   ```
6. Acceder a la API a través de Swagger: http://localhost:5106/swagger

## Uso de la API

### Autenticación

1. Utilizar el endpoint `POST /api/auth/login` con las siguientes credenciales de prueba:
   ```json
   {
     "username": "admin",
     "password": "Admin123!"
   }
   ```
2. Copiar el token JWT recibido en la respuesta
3. Hacer clic en el botón "Authorize" en la interfaz de Swagger
4. Introducir el token en el formato: `Bearer {token}`
5. Hacer clic en "Authorize" y luego en "Close"

Ahora todas las solicitudes a endpoints protegidos incluirán automáticamente el token JWT en el encabezado de autorización.

### Endpoints Disponibles

- `POST /api/auth/login`: Iniciar sesión
- `POST /api/auth/refresh-token`: Renovar token JWT
- `POST /api/auth/revoke-token`: Revocar token JWT
- `GET /api/auth/test`: Endpoint de prueba (no requiere autenticación)
- `GET /api/auth/protected`: Endpoint protegido (requiere autenticación)

## Próximos Pasos

- Implementar módulo de empresas
- Implementar módulo de sucursales
- Implementar módulo de empleados
- Agregar validación de datos
- Implementar pruebas unitarias y de integración

## Tecnologías Utilizadas

- ASP.NET Core 8.0
- Entity Framework Core 8.0
- SQL Server
- JWT para autenticación
- Swagger/OpenAPI para documentación de la API
- Arquitectura Limpia (Clean Architecture)
- Patrón Mediator con MediatR
- Inyección de dependencias
