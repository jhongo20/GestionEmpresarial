# Documentación Técnica de la Base de Datos

## Descripción General

Este documento describe la estructura completa de la base de datos del sistema de Gestión Empresarial, incluyendo todas las tablas, campos, relaciones, claves primarias, claves foráneas y restricciones. La base de datos está diseñada siguiendo los principios de Clean Architecture y utiliza Entity Framework Core como ORM.

## Diagrama de Entidad-Relación

El siguiente diagrama muestra las principales entidades y sus relaciones:

```
+----------------+     +----------------+     +----------------+
|      User      |<--->|    UserRole    |<--->|      Role      |
+----------------+     +----------------+     +----------------+
        |                                            |
        v                                            v
+----------------+                          +----------------+
| ActivationToken|                          | RolePermission |<---> Permission
+----------------+                          +----------------+
        |                                            |
        v                                            v
+----------------+                          +----------------+
|  RefreshToken  |                          |   RoleModule   |<---> Module
+----------------+                          +----------------+
        |                                            |
        v                                            v
+----------------+                          +----------------+
|  UserSession   |                          |   RoleRoute    |<---> Route
+----------------+                          +----------------+

+----------------+
| EmailTemplate  |
+----------------+

+----------------+
|    AuditLog    |
+----------------+
```

## Estructura de Tablas

### 1. Users

Almacena información de los usuarios del sistema.

| Campo | Tipo | Descripción | Restricciones |
|-------|------|-------------|---------------|
| Id | GUID | Identificador único del usuario | PK |
| Username | VARCHAR(100) | Nombre de usuario | NOT NULL, UNIQUE |
| Email | VARCHAR(255) | Correo electrónico | NOT NULL, UNIQUE |
| PasswordHash | VARCHAR(255) | Hash de la contraseña | NOT NULL |
| FirstName | VARCHAR(100) | Nombre | NULL |
| LastName | VARCHAR(100) | Apellido | NULL |
| PhoneNumber | VARCHAR(20) | Número de teléfono | NULL |
| LastLoginDate | DATETIME | Fecha del último inicio de sesión | NULL |
| Status | INT | Estado del usuario (Activo, Inactivo, Bloqueado) | NOT NULL |
| UserType | INT | Tipo de usuario (Interno, Externo) | NOT NULL |
| SecurityStamp | VARCHAR(255) | Estampa de seguridad | NULL |
| ConcurrencyStamp | VARCHAR(255) | Estampa de concurrencia | NULL |
| LdapDomain | VARCHAR(255) | Dominio LDAP | NULL |
| IsDeleted | BIT | Indica si el usuario está eliminado | NOT NULL |
| DeletedAt | DATETIME | Fecha de eliminación | NULL |
| IsActive | BIT | Indica si el usuario está activo | NOT NULL |
| EmailConfirmed | BIT | Indica si el correo está confirmado | NOT NULL |
| ActivationToken | VARCHAR(255) | Token de activación | NULL |
| ActivationTokenExpires | DATETIME | Fecha de expiración del token | NULL |
| IsLdapUser | BIT | Indica si es un usuario LDAP | NOT NULL |
| IsInternalUser | BIT | Indica si es un usuario interno (@mintrabajo.gov.co) | NOT NULL |
| CreatedBy | VARCHAR(255) | Usuario que creó el registro | NOT NULL |
| CreatedAt | DATETIME | Fecha de creación | NOT NULL |
| UpdatedBy | VARCHAR(255) | Usuario que actualizó el registro | NULL |
| UpdatedAt | DATETIME | Fecha de actualización | NULL |

**Índices:**
- PK_Users (Id)
- IX_Users_Username (Username)
- IX_Users_Email (Email)
- IX_Users_IsDeleted (IsDeleted)
- IX_Users_IsInternalUser (IsInternalUser)

### 2. Roles

Almacena los roles del sistema.

| Campo | Tipo | Descripción | Restricciones |
|-------|------|-------------|---------------|
| Id | GUID | Identificador único del rol | PK |
| Name | VARCHAR(100) | Nombre del rol | NOT NULL, UNIQUE |
| Description | VARCHAR(255) | Descripción del rol | NOT NULL |
| IsDeleted | BIT | Indica si el rol está eliminado | NOT NULL |
| CreatedBy | VARCHAR(255) | Usuario que creó el registro | NOT NULL |
| CreatedAt | DATETIME | Fecha de creación | NOT NULL |
| UpdatedBy | VARCHAR(255) | Usuario que actualizó el registro | NULL |
| UpdatedAt | DATETIME | Fecha de actualización | NULL |

**Índices:**
- PK_Roles (Id)
- IX_Roles_Name (Name)
- IX_Roles_IsDeleted (IsDeleted)

### 3. UserRoles

Tabla de relación muchos a muchos entre usuarios y roles.

| Campo | Tipo | Descripción | Restricciones |
|-------|------|-------------|---------------|
| Id | GUID | Identificador único | PK |
| UserId | GUID | ID del usuario | FK -> Users(Id) |
| RoleId | GUID | ID del rol | FK -> Roles(Id) |
| IsActive | BIT | Indica si la asignación está activa | NOT NULL |
| IsDeleted | BIT | Indica si la asignación está eliminada | NOT NULL |
| CreatedBy | VARCHAR(255) | Usuario que creó el registro | NOT NULL |
| CreatedAt | DATETIME | Fecha de creación | NOT NULL |
| UpdatedBy | VARCHAR(255) | Usuario que actualizó el registro | NULL |
| UpdatedAt | DATETIME | Fecha de actualización | NULL |

**Índices:**
- PK_UserRoles (Id)
- IX_UserRoles_UserId (UserId)
- IX_UserRoles_RoleId (RoleId)
- IX_UserRoles_UserId_RoleId (UserId, RoleId) - UNIQUE

### 4. Permissions

Almacena los permisos disponibles en el sistema.

| Campo | Tipo | Descripción | Restricciones |
|-------|------|-------------|---------------|
| Id | GUID | Identificador único del permiso | PK |
| Name | VARCHAR(100) | Nombre del permiso | NOT NULL, UNIQUE |
| Description | VARCHAR(255) | Descripción del permiso | NOT NULL |
| IsActive | BIT | Indica si el permiso está activo | NOT NULL |
| IsDeleted | BIT | Indica si el permiso está eliminado | NOT NULL |
| CreatedBy | VARCHAR(255) | Usuario que creó el registro | NOT NULL |
| CreatedAt | DATETIME | Fecha de creación | NOT NULL |
| UpdatedBy | VARCHAR(255) | Usuario que actualizó el registro | NULL |
| UpdatedAt | DATETIME | Fecha de actualización | NULL |

**Índices:**
- PK_Permissions (Id)
- IX_Permissions_Name (Name)

### 5. RolePermissions

Tabla de relación muchos a muchos entre roles y permisos.

| Campo | Tipo | Descripción | Restricciones |
|-------|------|-------------|---------------|
| Id | GUID | Identificador único | PK |
| RoleId | GUID | ID del rol | FK -> Roles(Id) |
| PermissionId | GUID | ID del permiso | FK -> Permissions(Id) |
| IsActive | BIT | Indica si la asignación está activa | NOT NULL |
| IsDeleted | BIT | Indica si la asignación está eliminada | NOT NULL |
| CreatedBy | VARCHAR(255) | Usuario que creó el registro | NOT NULL |
| CreatedAt | DATETIME | Fecha de creación | NOT NULL |
| UpdatedBy | VARCHAR(255) | Usuario que actualizó el registro | NULL |
| UpdatedAt | DATETIME | Fecha de actualización | NULL |

**Índices:**
- PK_RolePermissions (Id)
- IX_RolePermissions_RoleId (RoleId)
- IX_RolePermissions_PermissionId (PermissionId)
- IX_RolePermissions_RoleId_PermissionId (RoleId, PermissionId) - UNIQUE

### 6. RefreshTokens

Almacena tokens de actualización para la autenticación JWT.

| Campo | Tipo | Descripción | Restricciones |
|-------|------|-------------|---------------|
| Id | GUID | Identificador único del token | PK |
| UserId | GUID | ID del usuario | FK -> Users(Id) |
| Token | VARCHAR(255) | Token de actualización | NOT NULL |
| Expires | DATETIME | Fecha de expiración | NOT NULL |
| Created | DATETIME | Fecha de creación | NOT NULL |
| CreatedByIp | VARCHAR(50) | IP que creó el token | NOT NULL |
| Revoked | DATETIME | Fecha de revocación | NULL |
| RevokedByIp | VARCHAR(50) | IP que revocó el token | NULL |
| ReplacedByToken | VARCHAR(255) | Token de reemplazo | NULL |
| ReasonRevoked | VARCHAR(255) | Razón de revocación | NULL |
| IsExpired | BIT | Indica si el token está expirado | NOT NULL |
| IsRevoked | BIT | Indica si el token está revocado | NOT NULL |
| IsActive | BIT | Indica si el token está activo | NOT NULL |
| CreatedBy | VARCHAR(255) | Usuario que creó el registro | NOT NULL |
| CreatedAt | DATETIME | Fecha de creación | NOT NULL |
| UpdatedBy | VARCHAR(255) | Usuario que actualizó el registro | NULL |
| UpdatedAt | DATETIME | Fecha de actualización | NULL |

**Índices:**
- PK_RefreshTokens (Id)
- IX_RefreshTokens_UserId (UserId)
- IX_RefreshTokens_Token (Token)

### 7. UserSessions

Almacena información sobre las sesiones de los usuarios.

| Campo | Tipo | Descripción | Restricciones |
|-------|------|-------------|---------------|
| Id | GUID | Identificador único de la sesión | PK |
| UserId | GUID | ID del usuario | FK -> Users(Id) |
| LoginDate | DATETIME | Fecha de inicio de sesión | NOT NULL |
| LogoutDate | DATETIME | Fecha de cierre de sesión | NULL |
| IpAddress | VARCHAR(50) | Dirección IP | NOT NULL |
| UserAgent | VARCHAR(255) | Agente de usuario | NOT NULL |
| IsActive | BIT | Indica si la sesión está activa | NOT NULL |
| CreatedBy | VARCHAR(255) | Usuario que creó el registro | NOT NULL |
| CreatedAt | DATETIME | Fecha de creación | NOT NULL |
| UpdatedBy | VARCHAR(255) | Usuario que actualizó el registro | NULL |
| UpdatedAt | DATETIME | Fecha de actualización | NULL |

**Índices:**
- PK_UserSessions (Id)
- IX_UserSessions_UserId (UserId)
- IX_UserSessions_IsActive (IsActive)

### 8. ActivationTokens

Almacena tokens para la activación de cuentas de usuario.

| Campo | Tipo | Descripción | Restricciones |
|-------|------|-------------|---------------|
| Id | GUID | Identificador único del token | PK |
| UserId | GUID | ID del usuario | FK -> Users(Id) |
| Token | VARCHAR(255) | Token de activación | NOT NULL |
| ActivationCode | VARCHAR(10) | Código numérico de activación | NOT NULL |
| ExpiryDate | DATETIME | Fecha de expiración | NOT NULL |
| IsUsed | BIT | Indica si el token ha sido usado | NOT NULL |
| UsedDate | DATETIME | Fecha de uso | NULL |
| CreatedBy | VARCHAR(255) | Usuario que creó el registro | NOT NULL |
| CreatedAt | DATETIME | Fecha de creación | NOT NULL |
| UpdatedBy | VARCHAR(255) | Usuario que actualizó el registro | NULL |
| UpdatedAt | DATETIME | Fecha de actualización | NULL |

**Índices:**
- PK_ActivationTokens (Id)
- IX_ActivationTokens_UserId (UserId)
- IX_ActivationTokens_Token (Token)
- IX_ActivationTokens_ActivationCode (ActivationCode)

### 9. Modules

Almacena los módulos del sistema para la navegación.

| Campo | Tipo | Descripción | Restricciones |
|-------|------|-------------|---------------|
| Id | GUID | Identificador único del módulo | PK |
| Name | VARCHAR(100) | Nombre del módulo | NOT NULL |
| Description | VARCHAR(255) | Descripción del módulo | NOT NULL |
| Icon | VARCHAR(50) | Icono del módulo | NOT NULL |
| Path | VARCHAR(255) | Ruta del módulo | NOT NULL |
| Order | INT | Orden de visualización | NOT NULL |
| IsActive | BIT | Indica si el módulo está activo | NOT NULL |
| IsDeleted | BIT | Indica si el módulo está eliminado | NOT NULL |
| CreatedBy | VARCHAR(255) | Usuario que creó el registro | NOT NULL |
| CreatedAt | DATETIME | Fecha de creación | NOT NULL |
| UpdatedBy | VARCHAR(255) | Usuario que actualizó el registro | NULL |
| UpdatedAt | DATETIME | Fecha de actualización | NULL |

**Índices:**
- PK_Modules (Id)
- IX_Modules_Name (Name)
- IX_Modules_Path (Path)
- IX_Modules_Order (Order)

### 10. Routes

Almacena las rutas disponibles en el sistema.

| Campo | Tipo | Descripción | Restricciones |
|-------|------|-------------|---------------|
| Id | GUID | Identificador único de la ruta | PK |
| ModuleId | GUID | ID del módulo | FK -> Modules(Id) |
| Name | VARCHAR(100) | Nombre de la ruta | NOT NULL |
| Path | VARCHAR(255) | Ruta URL | NOT NULL |
| Description | VARCHAR(255) | Descripción de la ruta | NOT NULL |
| Icon | VARCHAR(50) | Icono de la ruta | NOT NULL |
| Order | INT | Orden de visualización | NOT NULL |
| IsActive | BIT | Indica si la ruta está activa | NOT NULL |
| IsDeleted | BIT | Indica si la ruta está eliminada | NOT NULL |
| CreatedBy | VARCHAR(255) | Usuario que creó el registro | NOT NULL |
| CreatedAt | DATETIME | Fecha de creación | NOT NULL |
| UpdatedBy | VARCHAR(255) | Usuario que actualizó el registro | NULL |
| UpdatedAt | DATETIME | Fecha de actualización | NULL |

**Índices:**
- PK_Routes (Id)
- IX_Routes_ModuleId (ModuleId)
- IX_Routes_Path (Path)
- IX_Routes_Order (Order)

### 11. RoleModules

Tabla de relación muchos a muchos entre roles y módulos.

| Campo | Tipo | Descripción | Restricciones |
|-------|------|-------------|---------------|
| Id | GUID | Identificador único | PK |
| RoleId | GUID | ID del rol | FK -> Roles(Id) |
| ModuleId | GUID | ID del módulo | FK -> Modules(Id) |
| IsActive | BIT | Indica si la asignación está activa | NOT NULL |
| IsDeleted | BIT | Indica si la asignación está eliminada | NOT NULL |
| CreatedBy | VARCHAR(255) | Usuario que creó el registro | NOT NULL |
| CreatedAt | DATETIME | Fecha de creación | NOT NULL |
| UpdatedBy | VARCHAR(255) | Usuario que actualizó el registro | NULL |
| UpdatedAt | DATETIME | Fecha de actualización | NULL |

**Índices:**
- PK_RoleModules (Id)
- IX_RoleModules_RoleId (RoleId)
- IX_RoleModules_ModuleId (ModuleId)
- IX_RoleModules_RoleId_ModuleId (RoleId, ModuleId) - UNIQUE

### 12. RoleRoutes

Tabla de relación muchos a muchos entre roles y rutas.

| Campo | Tipo | Descripción | Restricciones |
|-------|------|-------------|---------------|
| Id | GUID | Identificador único | PK |
| RoleId | GUID | ID del rol | FK -> Roles(Id) |
| RouteId | GUID | ID de la ruta | FK -> Routes(Id) |
| IsActive | BIT | Indica si la asignación está activa | NOT NULL |
| IsDeleted | BIT | Indica si la asignación está eliminada | NOT NULL |
| CreatedBy | VARCHAR(255) | Usuario que creó el registro | NOT NULL |
| CreatedAt | DATETIME | Fecha de creación | NOT NULL |
| UpdatedBy | VARCHAR(255) | Usuario que actualizó el registro | NULL |
| UpdatedAt | DATETIME | Fecha de actualización | NULL |

**Índices:**
- PK_RoleRoutes (Id)
- IX_RoleRoutes_RoleId (RoleId)
- IX_RoleRoutes_RouteId (RouteId)
- IX_RoleRoutes_RoleId_RouteId (RoleId, RouteId) - UNIQUE

### 13. AuditLogs

Almacena registros de auditoría de las operaciones realizadas en el sistema.

| Campo | Tipo | Descripción | Restricciones |
|-------|------|-------------|---------------|
| Id | GUID | Identificador único del registro | PK |
| UserId | VARCHAR(255) | ID del usuario | NOT NULL |
| Type | VARCHAR(50) | Tipo de operación | NOT NULL |
| TableName | VARCHAR(100) | Nombre de la tabla | NOT NULL |
| DateTime | DATETIME | Fecha y hora de la operación | NOT NULL |
| OldValues | TEXT | Valores antiguos (JSON) | NULL |
| NewValues | TEXT | Valores nuevos (JSON) | NULL |
| AffectedColumns | VARCHAR(MAX) | Columnas afectadas | NULL |
| PrimaryKey | VARCHAR(255) | Clave primaria del registro afectado | NOT NULL |
| IpAddress | VARCHAR(50) | Dirección IP | NULL |
| UserAgent | VARCHAR(255) | Agente de usuario | NULL |
| CreatedBy | VARCHAR(255) | Usuario que creó el registro | NOT NULL |
| CreatedAt | DATETIME | Fecha de creación | NOT NULL |
| UpdatedBy | VARCHAR(255) | Usuario que actualizó el registro | NULL |
| UpdatedAt | DATETIME | Fecha de actualización | NULL |

**Índices:**
- PK_AuditLogs (Id)
- IX_AuditLogs_UserId (UserId)
- IX_AuditLogs_TableName (TableName)
- IX_AuditLogs_DateTime (DateTime)
- IX_AuditLogs_Type (Type)

### 14. EmailTemplates

Almacena plantillas de correo electrónico utilizadas por el sistema.

| Campo | Tipo | Descripción | Restricciones |
|-------|------|-------------|---------------|
| Id | GUID | Identificador único de la plantilla | PK |
| Name | VARCHAR(100) | Nombre de la plantilla | NOT NULL, UNIQUE |
| Subject | VARCHAR(255) | Asunto del correo | NOT NULL |
| HtmlBody | TEXT | Cuerpo HTML del correo | NOT NULL |
| PlainTextBody | TEXT | Cuerpo de texto plano del correo | NOT NULL |
| Description | VARCHAR(255) | Descripción de la plantilla | NOT NULL |
| Type | VARCHAR(50) | Tipo de plantilla | NOT NULL |
| IsActive | BIT | Indica si la plantilla está activa | NOT NULL |
| IsDeleted | BIT | Indica si la plantilla está eliminada | NOT NULL |
| IsDefault | BIT | Indica si es la plantilla predeterminada | NOT NULL |
| AvailableVariables | TEXT | Variables disponibles (JSON) | NOT NULL |
| CreatedBy | VARCHAR(255) | Usuario que creó el registro | NOT NULL |
| CreatedAt | DATETIME | Fecha de creación | NOT NULL |
| UpdatedBy | VARCHAR(255) | Usuario que actualizó el registro | NULL |
| UpdatedAt | DATETIME | Fecha de actualización | NULL |

**Índices:**
- PK_EmailTemplates (Id)
- IX_EmailTemplates_Name (Name)
- IX_EmailTemplates_Type (Type)

## Relaciones entre Tablas

### Relaciones de Usuario

- **Users -> UserRoles**: Un usuario puede tener múltiples roles (1:N)
- **Users -> RefreshTokens**: Un usuario puede tener múltiples tokens de actualización (1:N)
- **Users -> UserSessions**: Un usuario puede tener múltiples sesiones (1:N)
- **Users -> ActivationTokens**: Un usuario puede tener múltiples tokens de activación (1:N)

### Relaciones de Rol

- **Roles -> UserRoles**: Un rol puede estar asignado a múltiples usuarios (1:N)
- **Roles -> RolePermissions**: Un rol puede tener múltiples permisos (1:N)
- **Roles -> RoleModules**: Un rol puede tener acceso a múltiples módulos (1:N)
- **Roles -> RoleRoutes**: Un rol puede tener acceso a múltiples rutas (1:N)

### Relaciones de Módulo

- **Modules -> Routes**: Un módulo puede contener múltiples rutas (1:N)
- **Modules -> RoleModules**: Un módulo puede estar asignado a múltiples roles (1:N)

### Relaciones de Ruta

- **Routes -> RoleRoutes**: Una ruta puede estar asignada a múltiples roles (1:N)
- **Routes -> Modules**: Una ruta pertenece a un módulo (N:1)

### Relaciones de Permiso

- **Permissions -> RolePermissions**: Un permiso puede estar asignado a múltiples roles (1:N)

## Herencia y Estructura Común

Todas las entidades heredan de la clase base `AuditableEntity`, que proporciona los siguientes campos:

- **CreatedBy**: Usuario que creó el registro
- **CreatedAt**: Fecha de creación
- **UpdatedBy**: Usuario que actualizó el registro
- **UpdatedAt**: Fecha de actualización

## Consideraciones de Diseño

1. **Soft Delete**: La mayoría de las entidades implementan eliminación lógica mediante el campo `IsDeleted` en lugar de eliminar físicamente los registros.

2. **Auditoría**: Todos los cambios en las entidades se registran automáticamente en la tabla `AuditLogs`, capturando el usuario, la fecha, los valores antiguos y nuevos, y otra información relevante.

3. **Seguridad**: La autenticación se maneja mediante JWT con tokens de actualización, y las contraseñas se almacenan como hashes seguros.

4. **Activación de Cuentas**: Los usuarios pueden activar sus cuentas mediante tokens o códigos numéricos enviados por correo electrónico.

5. **Integración LDAP**: El sistema soporta autenticación contra directorios LDAP/Active Directory, con funcionalidades especiales para usuarios internos.

6. **Menús Dinámicos**: La estructura de módulos y rutas permite generar menús dinámicos basados en los roles del usuario.

## Índices y Optimización

Se han creado índices en campos clave para optimizar las consultas más frecuentes:

1. **Claves Primarias**: Todas las tablas tienen índices en sus claves primarias.
2. **Claves Foráneas**: Todas las relaciones tienen índices en sus claves foráneas.
3. **Campos de Búsqueda**: Se han indexado campos como `Username`, `Email`, `Name`, etc.
4. **Filtros Comunes**: Se han indexado campos como `IsDeleted`, `IsActive`, etc.
5. **Relaciones Únicas**: Se han creado índices únicos compuestos para relaciones muchos a muchos.

## Scripts de Migración

Los scripts de migración se encuentran en la carpeta `src/GestionEmpresarial.Infrastructure/Persistence/Migrations/Manual` y deben ejecutarse en el siguiente orden:

1. Scripts de creación de tablas (generados por Entity Framework Core)
2. Scripts de datos iniciales (usuarios, roles, permisos, etc.)
3. Scripts de actualización (como `AddInternalUserColumn.sql` y `UpdateLdapVerification.sql`)

## Consideraciones de Rendimiento

1. **Paginación**: Todas las consultas que devuelven listas grandes implementan paginación para evitar problemas de rendimiento.

2. **Lazy Loading**: Se utiliza carga diferida para las relaciones para evitar cargar datos innecesarios.

3. **Consultas Optimizadas**: Se utilizan consultas optimizadas con proyecciones para devolver solo los datos necesarios.

4. **Caché**: Se implementa caché para consultas frecuentes y datos que cambian con poca frecuencia.

## Mantenimiento y Escalabilidad

1. **Backups**: Se recomienda configurar backups diarios de la base de datos.

2. **Monitoreo**: Se debe monitorear el rendimiento de las consultas y el crecimiento de las tablas.

3. **Archivado**: Para datos históricos, se recomienda implementar una estrategia de archivado.

4. **Particionamiento**: Para tablas que crecen rápidamente, como `AuditLogs`, se puede considerar el particionamiento.

## Conclusión

La estructura de la base de datos está diseñada para soportar un sistema de gestión empresarial completo, con funcionalidades robustas de autenticación, autorización, auditoría y gestión de usuarios. La arquitectura permite una fácil extensión para agregar nuevas funcionalidades sin comprometer la integridad y seguridad de los datos.
