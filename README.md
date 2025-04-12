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
- Sistema de menús basados en roles
- Arquitectura Limpia
- Patrón Mediator con MediatR
- Entity Framework Core para acceso a datos
- Validación de datos robusta con FluentValidation
- Paginación para endpoints que devuelven listas grandes
- Filtrado y ordenación avanzada
- Seguridad mejorada con hash de contraseñas BCrypt
- Envío de correos electrónicos para notificaciones de usuarios
- Activación de cuentas mediante tokens para verificación de correo electrónico
- Soporte para usuarios LDAP/Active Directory

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
   > **Nota**: Esta configuración permite la autenticación de usuarios LDAP creados manualmente en el sistema. Para crear un usuario LDAP, utilice el endpoint `POST /api/users/ldap`.

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
- `POST /api/auth/test-login`: Iniciar sesión de prueba (para usuarios de prueba)
- `POST /api/auth/refresh-token`: Renovar token JWT
- `POST /api/auth/revoke-token`: Revocar token JWT
- `GET /api/auth/test`: Endpoint de prueba (no requiere autenticación)
- `GET /api/auth/protected`: Endpoint protegido (requiere autenticación)

### Activación de Cuenta

- `POST /api/account/activate`: Activar cuenta de usuario con token
- `POST /api/account/resend-activation`: Reenviar correo de activación
- `POST /api/account/generate-activation-token/{userId}`: Generar nuevo token de activación (solo administradores)

### Usuarios

- `GET /api/users`: Obtener todos los usuarios
- `GET /api/users/paged`: Obtener usuarios paginados
- `GET /api/users/{id}`: Obtener usuario por ID
- `POST /api/users`: Crear usuario
- `POST /api/users/ldap`: Crear usuario LDAP
- `PUT /api/users/{id}`: Actualizar usuario
- `DELETE /api/users/{id}`: Eliminar usuario
- `POST /api/users/change-password`: Cambiar contraseña
- `GET /api/users/by-role/{roleId}`: Obtener usuarios por rol
- `GET /api/users/by-role/{roleId}/paged`: Obtener usuarios por rol paginados

### Menús y Navegación

- `GET /api/menus/my-menu`: Obtener el menú del usuario autenticado
- `GET /api/menus/by-role/{roleId}`: Obtener el menú para un rol específico
- `GET /api/modules`: Obtener todos los módulos
- `GET /api/modules/{id}`: Obtener un módulo por ID
- `POST /api/modules`: Crear un nuevo módulo
- `PUT /api/modules/{id}`: Actualizar un módulo existente
- `DELETE /api/modules/{id}`: Eliminar un módulo
- `GET /api/routes`: Obtener todas las rutas
- `GET /api/routes/{id}`: Obtener una ruta por ID
- `POST /api/routes`: Crear una nueva ruta
- `PUT /api/routes/{id}`: Actualizar una ruta existente
- `DELETE /api/routes/{id}`: Eliminar una ruta

## Pruebas

Se incluye un script PowerShell para probar la autenticación:

```bash
./test-auth.ps1
```

## Usuarios de Prueba

El sistema incluye los siguientes usuarios de prueba para facilitar las pruebas:

| Usuario    | Contraseña | Rol           | Acceso                                      |
|------------|------------|---------------|---------------------------------------------|
| admin      | Admin123!  | Administrador | Acceso completo a todos los módulos y rutas |
| supervisor | Admin123!  | Supervisor    | Acceso a Dashboard, Usuarios y Roles        |
| user       | Admin123!  | Usuario       | Acceso limitado solo al Dashboard           |
| testadmin  | test123    | Administrador | Acceso completo (usuario alternativo)       |

## Sistema de Menús Basados en Roles

El sistema implementa un mecanismo de menús dinámicos basados en roles que permite:

1. **Configuración de módulos**: Cada módulo representa una sección principal de la aplicación.
2. **Configuración de rutas**: Las rutas son elementos de navegación dentro de un módulo.
3. **Asignación de permisos por rol**: Cada rol puede tener acceso a diferentes módulos y rutas.
4. **Menús dinámicos**: El frontend puede solicitar el menú correspondiente al usuario autenticado.

### Estructura de Datos

- **Módulos**: Representan las secciones principales de la aplicación.
  - Propiedades: Id, Name, Description, Icon, Path, Order, IsActive
  
- **Rutas**: Representan las páginas o funcionalidades dentro de un módulo.
  - Propiedades: Id, Name, Path, Icon, Order, IsActive, ModuleId
  
- **RoleModules**: Asigna módulos a roles específicos.
  - Propiedades: Id, RoleId, ModuleId, IsActive
  
- **RoleRoutes**: Asigna rutas a roles específicos.
  - Propiedades: Id, RoleId, RouteId, IsActive

### Ejemplo de Respuesta de Menú

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Dashboard",
    "icon": "dashboard",
    "path": "/dashboard",
    "order": 1,
    "children": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Inicio",
        "path": "/dashboard",
        "icon": "home",
        "order": 1
      },
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Estadísticas",
        "path": "/dashboard/stats",
        "icon": "bar_chart",
        "order": 2
      }
    ]
  }
]
```

## Migración Manual de Base de Datos

Para aplicar las migraciones manuales y configurar los módulos, rutas y usuarios de prueba:

1. Ejecute los siguientes scripts SQL en orden:

```bash
# Desde la carpeta raíz del proyecto
sqlcmd -S localhost -d GestionEmpresarialNuevoDB -i "src\GestionEmpresarial.Infrastructure\Persistence\Migrations\Manual\FixedModulesAndRoutes.sql" -E
sqlcmd -S localhost -d GestionEmpresarialNuevoDB -i "src\GestionEmpresarial.Infrastructure\Persistence\Migrations\Manual\AddRoleAssignments.sql" -E
sqlcmd -S localhost -d GestionEmpresarialNuevoDB -i "src\GestionEmpresarial.Infrastructure\Persistence\Migrations\Manual\AddTestUsers.sql" -E
```

## Contribución

1. Haz un fork del repositorio
2. Crea una rama para tu característica (`git checkout -b feature/amazing-feature`)
3. Haz commit de tus cambios (`git commit -m 'Add some amazing feature'`)
4. Haz push a la rama (`git push origin feature/amazing-feature`)
5. Abre un Pull Request

## Licencia

Este proyecto está licenciado bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para más detalles.
