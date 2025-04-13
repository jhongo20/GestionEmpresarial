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
- Sistema de auditoría completo
- Arquitectura Limpia
- Patrón Mediator con MediatR
- Entity Framework Core para acceso a datos
- Validación de datos robusta con FluentValidation
- Paginación para endpoints que devuelven listas grandes
- Filtrado y ordenación avanzada
- Seguridad mejorada con hash de contraseñas BCrypt
- Envío de correos electrónicos para notificaciones de usuarios
- Activación de cuentas mediante tokens y códigos numéricos para verificación de correo electrónico
- Soporte para usuarios LDAP/Active Directory
- Inicio de sesión simplificado para usuarios internos (dominio @mintrabajo.gov.co)
- Verificación de existencia en directorio activo para usuarios internos

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

## Documentación

- [Activación de Cuentas](docs/AccountActivation.md): Detalles sobre el proceso de activación de cuentas mediante tokens y códigos.
- [Usuarios Internos](docs/InternalUsers.md): Información sobre las funcionalidades especiales para usuarios con dominio @mintrabajo.gov.co.
- [Estructura de Base de Datos](docs/DatabaseStructure.md): Documentación técnica completa de la estructura de la base de datos, incluyendo tablas, campos, relaciones y consideraciones de diseño.

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

### Activación de Cuentas

- `POST /api/account-activation/activate-with-token`: Activar cuenta mediante token (enviado por correo)
- `POST /api/account-activation/activate-with-code`: Activar cuenta mediante código numérico y correo electrónico

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
- `GET /api/menus/test-menu`: Obtener un menú de ejemplo (no requiere autenticación)
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

### Auditoría

- `GET /api/audit`: Obtener registros de auditoría con filtros y paginación
- `GET /api/audit/{id}`: Obtener un registro de auditoría por ID
- `GET /api/audit/tables`: Obtener lista de tablas para filtrar auditorías
- `GET /api/audit/actions`: Obtener lista de acciones para filtrar auditorías

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

## Sistema de Auditoría

El sistema implementa un mecanismo completo de auditoría que registra automáticamente todas las acciones importantes realizadas en la aplicación:

1. **Registro automático**: Captura automáticamente las operaciones CRUD en entidades importantes.
2. **Información detallada**: Almacena información sobre el usuario, acción, tabla, valores antiguos y nuevos, IP, agente de usuario, etc.
3. **Middleware personalizado**: Utiliza un middleware para capturar información adicional de las solicitudes HTTP.
4. **Filtrado avanzado**: Permite filtrar registros por usuario, tabla, acción y rango de fechas.
5. **Visualización amigable**: Incluye una página HTML para visualizar y explorar los registros de auditoría.

### Estructura de Datos

La entidad `AuditLog` almacena los siguientes datos:

- **Id**: Identificador único del registro de auditoría
- **UserId**: ID del usuario que realizó la acción
- **UserName**: Nombre del usuario que realizó la acción
- **Type**: Tipo de operación (Create, Update, Delete, Security)
- **TableName**: Nombre de la tabla o entidad afectada
- **DateTime**: Fecha y hora de la acción
- **OldValues**: Valores anteriores (para actualizaciones)
- **NewValues**: Valores nuevos
- **AffectedColumns**: Columnas afectadas
- **PrimaryKey**: Clave primaria del registro afectado
- **Action**: Acción específica realizada
- **IpAddress**: Dirección IP desde donde se realizó la acción
- **UserAgent**: Agente de usuario del navegador

### Ejemplo de Uso

Para ver los registros de auditoría:

1. Inicie sesión como administrador
2. Acceda a la API mediante: `GET /api/audit`
3. Aplique filtros según sea necesario: `GET /api/audit?userId=admin&tableName=Users&fromDate=2023-01-01`

También puede utilizar la página de prueba HTML incluida en el proyecto:

1. Abra el archivo `tests/AuditTest.html` en su navegador
2. Inicie sesión con un usuario administrador
3. Explore y filtre los registros de auditoría

### Migración Manual de Base de Datos

Para crear la tabla de auditoría, ejecute el siguiente script SQL:

```bash
# Desde la carpeta raíz del proyecto
sqlcmd -S localhost -d GestionEmpresarialNuevoDB -i "src\GestionEmpresarial.Infrastructure\Persistence\Migrations\Manual\CreateAuditLogsTable.sql" -E
```

## Migración Manual de Base de Datos

Para aplicar las migraciones manuales y configurar los módulos, rutas, usuarios de prueba y auditoría:

1. Ejecute los siguientes scripts SQL en orden:

```bash
# Desde la carpeta raíz del proyecto
sqlcmd -S localhost -d GestionEmpresarialNuevoDB -i "src\GestionEmpresarial.Infrastructure\Persistence\Migrations\Manual\FixedModulesAndRoutes.sql" -E
sqlcmd -S localhost -d GestionEmpresarialNuevoDB -i "src\GestionEmpresarial.Infrastructure\Persistence\Migrations\Manual\AddRoleAssignments.sql" -E
sqlcmd -S localhost -d GestionEmpresarialNuevoDB -i "src\GestionEmpresarial.Infrastructure\Persistence\Migrations\Manual\AddTestUsers.sql" -E
sqlcmd -S localhost -d GestionEmpresarialNuevoDB -i "src\GestionEmpresarial.Infrastructure\Persistence\Migrations\Manual\CreateAuditLogsTable.sql" -E
```

## Contribución

1. Haz un fork del repositorio
2. Crea una rama para tu característica (`git checkout -b feature/amazing-feature`)
3. Haz commit de tus cambios (`git commit -m 'Add some amazing feature'`)
4. Haz push a la rama (`git push origin feature/amazing-feature`)
5. Abre un Pull Request

## Licencia

Este proyecto está licenciado bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para más detalles.
