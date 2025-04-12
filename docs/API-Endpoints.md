# GestionEmpresarial API - Documentación de Endpoints

## Introducción

Este documento describe los endpoints implementados en la API de GestionEmpresarial, un sistema de gestión empresarial desarrollado con ASP.NET Core 8.0 siguiendo los principios de Clean Architecture. La API proporciona funcionalidades para la gestión de roles, permisos y usuarios, con autenticación JWT para garantizar la seguridad de los datos.

## Arquitectura del Sistema

GestionEmpresarial sigue los principios de Clean Architecture, dividiendo la aplicación en las siguientes capas:

- **Domain**: Contiene las entidades del negocio y la lógica de dominio.
- **Application**: Contiene la lógica de aplicación, interfaces y DTOs.
- **Infrastructure**: Implementa las interfaces definidas en la capa de aplicación.
- **API**: Expone los endpoints RESTful y maneja las solicitudes HTTP.

Esta separación de responsabilidades permite un mantenimiento más sencillo y una mejor escalabilidad del sistema.

## Mejoras Implementadas

### Validación de Datos Robusta
Se ha implementado FluentValidation para validar los DTOs de entrada, garantizando que los datos cumplen con los requisitos de negocio antes de ser procesados. Esto incluye:
- Validación de campos obligatorios
- Validación de longitud de cadenas
- Validación de formatos de correo electrónico
- Políticas de contraseñas seguras

### Paginación para Listas Grandes
Se han implementado endpoints con paginación para mejorar el rendimiento cuando se trabaja con grandes volúmenes de datos:
- Parámetros de paginación: `PageNumber` y `PageSize`
- Información de paginación en la respuesta: total de páginas, total de registros, etc.
- Endpoints específicos para obtener datos paginados

### Filtrado y Ordenación Avanzada
Se ha añadido soporte para filtrado y ordenación dinámica:
- Parámetros de ordenación: `SortBy` y `SortDirection`
- Soporte para ordenación ascendente y descendente
- Ordenación por cualquier propiedad de las entidades

### Mejoras de Seguridad
Se han implementado diversas mejoras de seguridad:
- Hash de contraseñas con BCrypt.Net para almacenamiento seguro
- Verificación segura de contraseñas durante la autenticación
- Políticas de contraseñas seguras
- Manejo seguro de excepciones
- Filtro de excepciones para respuestas de error estandarizadas

### Envío de Correos Electrónicos
Se ha implementado un servicio de correo electrónico para:
- Confirmación de registro de nuevos usuarios
- Activación de cuentas
- Restablecimiento de contraseñas
- Notificaciones de actualización de cuenta

## Configuración del Entorno

### Requisitos Previos

- .NET 8.0 SDK
- SQL Server (Local o Express)
- Un cliente HTTP como Postman o la interfaz Swagger integrada

### Pasos para Ejecutar la API

1. Clone el repositorio
2. Navegue a la carpeta del proyecto
3. Asegúrese de que la cadena de conexión en `appsettings.json` apunte a su instancia de SQL Server
4. Ejecute las migraciones (si no lo ha hecho ya):
   ```
   dotnet ef database update --project src\GestionEmpresarial.Infrastructure --startup-project src\GestionEmpresarial.API
   ```
5. Inicie la aplicación:
   ```
   cd src\GestionEmpresarial.API
   dotnet run
   ```
6. Acceda a la interfaz Swagger en: `http://localhost:5106/swagger`

## Configuración de Correo Electrónico

Para habilitar el envío de correos electrónicos, es necesario configurar las siguientes opciones en el archivo `appsettings.json`:

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

### Notas sobre la Configuración de Gmail

Si utilizas Gmail como servidor SMTP, debes tener en cuenta lo siguiente:

1. **Contraseña de Aplicación**: Debes generar una contraseña de aplicación específica en la configuración de seguridad de tu cuenta de Google.
2. **Verificación en Dos Pasos**: Es recomendable tener habilitada la verificación en dos pasos para tu cuenta de Google.
3. **Acceso a Aplicaciones Menos Seguras**: Si no utilizas contraseñas de aplicación, deberás habilitar el acceso a aplicaciones menos seguras en la configuración de tu cuenta de Google (no recomendado para entornos de producción).

### Plantillas de Correo Electrónico

El sistema utiliza plantillas HTML para los correos electrónicos. Estas plantillas se generan dinámicamente en el código, pero pueden ser personalizadas modificando los métodos correspondientes en la clase `EmailService`.

## Autenticación

Todos los endpoints (excepto el de login) requieren autenticación mediante JWT. Para obtener un token:

1. Envíe una solicitud POST a `/api/auth/login` con las siguientes credenciales:
   ```json
   {
     "username": "admin",
     "password": "admin"
   }
   ```
2. Recibirá un token JWT en la respuesta:
   ```json
   {
     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
     "expiration": "2025-04-12T10:00:00Z"
   }
   ```
3. Incluya este token en el encabezado de autorización de todas las solicitudes posteriores:
   ```
   Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
   ```

En Swagger, puede hacer clic en el botón "Authorize" e ingresar el token para autenticar todas las solicitudes.

## Endpoints de Roles

### Obtener Todos los Roles

- **Método**: GET
- **URL**: `/api/roles`
- **Descripción**: Obtiene la lista de todos los roles disponibles en el sistema.
- **Autenticación**: Requerida
- **Ejemplo de Respuesta**:
  ```json
  [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Administrador",
      "description": "Rol con acceso completo al sistema",
      "createdAt": "2025-04-11T12:00:00Z",
      "createdBy": "System",
      "updatedAt": null,
      "updatedBy": null
    },
    {
      "id": "7fa85f64-5717-4562-b3fc-2c963f66afa7",
      "name": "Usuario",
      "description": "Rol con acceso limitado al sistema",
      "createdAt": "2025-04-11T12:00:00Z",
      "createdBy": "System",
      "updatedAt": null,
      "updatedBy": null
    }
  ]
  ```

### Obtener Rol por ID

- **Método**: GET
- **URL**: `/api/roles/{id}`
- **Descripción**: Obtiene un rol específico por su ID.
- **Autenticación**: Requerida
- **Parámetros de Ruta**:
  - `id` (GUID): ID del rol a obtener
- **Ejemplo de Respuesta**:
  ```json
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Administrador",
    "description": "Rol con acceso completo al sistema",
    "createdAt": "2025-04-11T12:00:00Z",
    "createdBy": "System",
    "updatedAt": null,
    "updatedBy": null
  }
  ```

### Crear Rol

- **Método**: POST
- **URL**: `/api/roles`
- **Descripción**: Crea un nuevo rol en el sistema.
- **Autenticación**: Requerida
- **Cuerpo de la Solicitud**:
  ```json
  {
    "name": "Supervisor",
    "description": "Rol para supervisores de área"
  }
  ```
- **Ejemplo de Respuesta**:
  ```json
  {
    "id": "5fa85f64-5717-4562-b3fc-2c963f66afa8",
    "name": "Supervisor",
    "description": "Rol para supervisores de área",
    "createdAt": "2025-04-12T15:30:00Z",
    "createdBy": "admin",
    "updatedAt": null,
    "updatedBy": null
  }
  ```

### Actualizar Rol

- **Método**: PUT
- **URL**: `/api/roles/{id}`
- **Descripción**: Actualiza un rol existente.
- **Autenticación**: Requerida
- **Parámetros de Ruta**:
  - `id` (GUID): ID del rol a actualizar
- **Cuerpo de la Solicitud**:
  ```json
  {
    "id": "5fa85f64-5717-4562-b3fc-2c963f66afa8",
    "name": "Supervisor Senior",
    "description": "Rol para supervisores senior de área"
  }
  ```
- **Ejemplo de Respuesta**:
  ```json
  {
    "id": "5fa85f64-5717-4562-b3fc-2c963f66afa8",
    "name": "Supervisor Senior",
    "description": "Rol para supervisores senior de área",
    "createdAt": "2025-04-12T15:30:00Z",
    "createdBy": "admin",
    "updatedAt": "2025-04-12T16:00:00Z",
    "updatedBy": "admin"
  }
  ```

### Eliminar Rol

- **Método**: DELETE
- **URL**: `/api/roles/{id}`
- **Descripción**: Elimina un rol del sistema (eliminación lógica).
- **Autenticación**: Requerida
- **Parámetros de Ruta**:
  - `id` (GUID): ID del rol a eliminar
- **Respuesta**: 204 No Content (si se elimina correctamente)

## Endpoints de Roles con Paginación

### Obtener Roles Paginados

- **Método**: GET
- **URL**: `/api/roles/paged`
- **Descripción**: Obtiene roles con paginación y ordenación.
- **Autenticación**: Requerida
- **Parámetros de Consulta**:
  - `PageNumber` (int): Número de página (por defecto: 1)
  - `PageSize` (int): Tamaño de página (por defecto: 10, máximo: 50)
  - `SortBy` (string): Campo por el que ordenar (por defecto: "Id")
  - `SortDirection` (string): Dirección de ordenación ("asc" o "desc", por defecto: "asc")
- **Ejemplo de Solicitud**: `GET /api/roles/paged?PageNumber=1&PageSize=10&SortBy=Name&SortDirection=asc`
- **Ejemplo de Respuesta**:
  ```json
  {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Administrador",
        "description": "Rol con acceso completo al sistema",
        "createdAt": "2025-04-12T10:00:00Z",
        "createdBy": "System",
        "updatedAt": "2025-04-12T10:00:00Z",
        "updatedBy": "System"
      }
    ],
    "pageNumber": 1,
    "totalPages": 1,
    "totalCount": 1,
    "hasPreviousPage": false,
    "hasNextPage": false
  }
  ```

## Endpoints de Permisos

### Obtener Todos los Permisos

- **Método**: GET
- **URL**: `/api/permissions`
- **Descripción**: Obtiene la lista de todos los permisos disponibles en el sistema.
- **Autenticación**: Requerida
- **Ejemplo de Respuesta**:
  ```json
  [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "CrearUsuario",
      "description": "Permiso para crear usuarios",
      "createdAt": "2025-04-11T12:00:00Z",
      "createdBy": "System",
      "updatedAt": null,
      "updatedBy": null
    },
    {
      "id": "7fa85f64-5717-4562-b3fc-2c963f66afa7",
      "name": "EliminarUsuario",
      "description": "Permiso para eliminar usuarios",
      "createdAt": "2025-04-11T12:00:00Z",
      "createdBy": "System",
      "updatedAt": null,
      "updatedBy": null
    }
  ]
  ```

### Obtener Permiso por ID

- **Método**: GET
- **URL**: `/api/permissions/{id}`
- **Descripción**: Obtiene un permiso específico por su ID.
- **Autenticación**: Requerida
- **Parámetros de Ruta**:
  - `id` (GUID): ID del permiso a obtener
- **Ejemplo de Respuesta**:
  ```json
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "CrearUsuario",
    "description": "Permiso para crear usuarios",
    "createdAt": "2025-04-11T12:00:00Z",
    "createdBy": "System",
    "updatedAt": null,
    "updatedBy": null
  }
  ```

### Crear Permiso

- **Método**: POST
- **URL**: `/api/permissions`
- **Descripción**: Crea un nuevo permiso en el sistema.
- **Autenticación**: Requerida
- **Cuerpo de la Solicitud**:
  ```json
  {
    "name": "EditarRol",
    "description": "Permiso para editar roles"
  }
  ```
- **Ejemplo de Respuesta**:
  ```json
  {
    "id": "5fa85f64-5717-4562-b3fc-2c963f66afa8",
    "name": "EditarRol",
    "description": "Permiso para editar roles",
    "createdAt": "2025-04-12T15:30:00Z",
    "createdBy": "admin",
    "updatedAt": null,
    "updatedBy": null
  }
  ```

### Actualizar Permiso

- **Método**: PUT
- **URL**: `/api/permissions/{id}`
- **Descripción**: Actualiza un permiso existente.
- **Autenticación**: Requerida
- **Parámetros de Ruta**:
  - `id` (GUID): ID del permiso a actualizar
- **Cuerpo de la Solicitud**:
  ```json
  {
    "id": "5fa85f64-5717-4562-b3fc-2c963f66afa8",
    "name": "EditarRoles",
    "description": "Permiso para editar roles del sistema"
  }
  ```
- **Ejemplo de Respuesta**:
  ```json
  {
    "id": "5fa85f64-5717-4562-b3fc-2c963f66afa8",
    "name": "EditarRoles",
    "description": "Permiso para editar roles del sistema",
    "createdAt": "2025-04-12T15:30:00Z",
    "createdBy": "admin",
    "updatedAt": "2025-04-12T16:00:00Z",
    "updatedBy": "admin"
  }
  ```

### Eliminar Permiso

- **Método**: DELETE
- **URL**: `/api/permissions/{id}`
- **Descripción**: Elimina un permiso del sistema (eliminación lógica).
- **Autenticación**: Requerida
- **Parámetros de Ruta**:
  - `id` (GUID): ID del permiso a eliminar
- **Respuesta**: 204 No Content (si se elimina correctamente)

## Endpoints de Permisos con Paginación

### Obtener Permisos Paginados

- **Método**: GET
- **URL**: `/api/permissions/paged`
- **Descripción**: Obtiene permisos con paginación y ordenación.
- **Autenticación**: Requerida
- **Parámetros de Consulta**:
  - `PageNumber` (int): Número de página (por defecto: 1)
  - `PageSize` (int): Tamaño de página (por defecto: 10, máximo: 50)
  - `SortBy` (string): Campo por el que ordenar (por defecto: "Id")
  - `SortDirection` (string): Dirección de ordenación ("asc" o "desc", por defecto: "asc")
- **Ejemplo de Solicitud**: `GET /api/permissions/paged?PageNumber=1&PageSize=10&SortBy=Name&SortDirection=asc`
- **Ejemplo de Respuesta**:
  ```json
  {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "CrearUsuario",
        "description": "Permiso para crear usuarios en el sistema",
        "createdAt": "2025-04-12T10:00:00Z",
        "createdBy": "System",
        "updatedAt": "2025-04-12T10:00:00Z",
        "updatedBy": "System"
      }
    ],
    "pageNumber": 1,
    "totalPages": 1,
    "totalCount": 1,
    "hasPreviousPage": false,
    "hasNextPage": false
  }
  ```

## Endpoints de Usuarios

### Obtener Todos los Usuarios

- **Método**: GET
- **URL**: `/api/users`
- **Descripción**: Obtiene la lista de todos los usuarios registrados en el sistema.
- **Autenticación**: Requerida
- **Ejemplo de Respuesta**:
  ```json
  [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "username": "admin",
      "email": "admin@example.com",
      "firstName": "Admin",
      "lastName": "User",
      "phoneNumber": null,
      "lastLoginDate": "2025-04-12T10:00:00Z",
      "status": 1,
      "userType": 1,
      "roles": ["Administrador"],
      "createdAt": "2025-04-11T12:00:00Z",
      "createdBy": "System",
      "updatedAt": null,
      "updatedBy": null
    },
    {
      "id": "7fa85f64-5717-4562-b3fc-2c963f66afa7",
      "username": "user1",
      "email": "user1@example.com",
      "firstName": "Regular",
      "lastName": "User",
      "phoneNumber": "123456789",
      "lastLoginDate": null,
      "status": 1,
      "userType": 1,
      "roles": ["Usuario"],
      "createdAt": "2025-04-11T12:00:00Z",
      "createdBy": "System",
      "updatedAt": null,
      "updatedBy": null
    }
  ]
  ```

### Obtener Usuario por ID

- **Método**: GET
- **URL**: `/api/users/{id}`
- **Descripción**: Obtiene un usuario específico por su ID.
- **Autenticación**: Requerida
- **Parámetros de Ruta**:
  - `id` (GUID): ID del usuario a obtener
- **Ejemplo de Respuesta**:
  ```json
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "username": "admin",
    "email": "admin@example.com",
    "firstName": "Admin",
    "lastName": "User",
    "phoneNumber": null,
    "lastLoginDate": "2025-04-12T10:00:00Z",
    "status": 1,
    "userType": 1,
    "roles": ["Administrador"],
    "createdAt": "2025-04-11T12:00:00Z",
    "createdBy": "System",
    "updatedAt": null,
    "updatedBy": null
  }
  ```

### Crear Usuario

- **Método**: POST
- **URL**: `/api/users`
- **Descripción**: Crea un nuevo usuario en el sistema.
- **Autenticación**: Requerida
- **Cuerpo de la Solicitud**:
  ```json
  {
    "username": "newuser",
    "email": "newuser@example.com",
    "password": "Password123!",
    "firstName": "New",
    "lastName": "User",
    "phoneNumber": "987654321",
    "status": 1,
    "userType": 1,
    "roleIds": ["7fa85f64-5717-4562-b3fc-2c963f66afa7"]
  }
  ```
- **Ejemplo de Respuesta**:
  ```json
  {
    "id": "9fa85f64-5717-4562-b3fc-2c963f66afa9",
    "username": "newuser",
    "email": "newuser@example.com",
    "firstName": "New",
    "lastName": "User",
    "phoneNumber": "987654321",
    "lastLoginDate": null,
    "status": 1,
    "userType": 1,
    "roles": ["Usuario"],
    "createdAt": "2025-04-12T15:30:00Z",
    "createdBy": "admin",
    "updatedAt": null,
    "updatedBy": null
  }
  ```

### Actualizar Usuario

- **Método**: PUT
- **URL**: `/api/users/{id}`
- **Descripción**: Actualiza un usuario existente.
- **Autenticación**: Requerida
- **Parámetros de Ruta**:
  - `id` (GUID): ID del usuario a actualizar
- **Cuerpo de la Solicitud**:
  ```json
  {
    "id": "9fa85f64-5717-4562-b3fc-2c963f66afa9",
    "email": "newuser.updated@example.com",
    "firstName": "Updated",
    "lastName": "User",
    "phoneNumber": "987654321",
    "status": 1,
    "userType": 1,
    "roleIds": ["7fa85f64-5717-4562-b3fc-2c963f66afa7", "5fa85f64-5717-4562-b3fc-2c963f66afa8"]
  }
  ```
- **Ejemplo de Respuesta**:
  ```json
  {
    "id": "9fa85f64-5717-4562-b3fc-2c963f66afa9",
    "username": "newuser",
    "email": "newuser.updated@example.com",
    "firstName": "Updated",
    "lastName": "User",
    "phoneNumber": "987654321",
    "lastLoginDate": null,
    "status": 1,
    "userType": 1,
    "roles": ["Usuario", "Supervisor Senior"],
    "createdAt": "2025-04-12T15:30:00Z",
    "createdBy": "admin",
    "updatedAt": "2025-04-12T16:00:00Z",
    "updatedBy": "admin"
  }
  ```

### Eliminar Usuario

- **Método**: DELETE
- **URL**: `/api/users/{id}`
- **Descripción**: Elimina un usuario del sistema (eliminación lógica).
- **Autenticación**: Requerida
- **Parámetros de Ruta**:
  - `id` (GUID): ID del usuario a eliminar
- **Respuesta**: 204 No Content (si se elimina correctamente)

### Cambiar Contraseña

- **Método**: POST
- **URL**: `/api/users/change-password`
- **Descripción**: Cambia la contraseña de un usuario.
- **Autenticación**: Requerida
- **Cuerpo de la Solicitud**:
  ```json
  {
    "userId": "9fa85f64-5717-4562-b3fc-2c963f66afa9",
    "currentPassword": "Password123!",
    "newPassword": "NewPassword123!",
    "confirmPassword": "NewPassword123!"
  }
  ```
- **Respuesta**: 200 OK (si la contraseña se cambia correctamente)

### Obtener Usuarios por Rol

- **Método**: GET
- **URL**: `/api/users/by-role/{roleId}`
- **Descripción**: Obtiene todos los usuarios que tienen asignado un rol específico.
- **Autenticación**: Requerida
- **Parámetros de Ruta**:
  - `roleId` (GUID): ID del rol
- **Ejemplo de Respuesta**:
  ```json
  [
    {
      "id": "7fa85f64-5717-4562-b3fc-2c963f66afa7",
      "username": "user1",
      "email": "user1@example.com",
      "firstName": "Regular",
      "lastName": "User",
      "phoneNumber": "123456789",
      "lastLoginDate": null,
      "status": 1,
      "userType": 1,
      "roles": ["Usuario"],
      "createdAt": "2025-04-11T12:00:00Z",
      "createdBy": "System",
      "updatedAt": null,
      "updatedBy": null
    },
    {
      "id": "9fa85f64-5717-4562-b3fc-2c963f66afa9",
      "username": "newuser",
      "email": "newuser.updated@example.com",
      "firstName": "Updated",
      "lastName": "User",
      "phoneNumber": "987654321",
      "lastLoginDate": null,
      "status": 1,
      "userType": 1,
      "roles": ["Usuario", "Supervisor Senior"],
      "createdAt": "2025-04-12T15:30:00Z",
      "createdBy": "admin",
      "updatedAt": "2025-04-12T16:00:00Z",
      "updatedBy": "admin"
    }
  ]
  ```

## Endpoints de Usuarios con Paginación

### Obtener Usuarios Paginados

- **Método**: GET
- **URL**: `/api/users/paged`
- **Descripción**: Obtiene usuarios con paginación y ordenación.
- **Autenticación**: Requerida
- **Parámetros de Consulta**:
  - `PageNumber` (int): Número de página (por defecto: 1)
  - `PageSize` (int): Tamaño de página (por defecto: 10, máximo: 50)
  - `SortBy` (string): Campo por el que ordenar (por defecto: "Id")
  - `SortDirection` (string): Dirección de ordenación ("asc" o "desc", por defecto: "asc")
- **Ejemplo de Solicitud**: `GET /api/users/paged?PageNumber=1&PageSize=10&SortBy=Username&SortDirection=asc`
- **Ejemplo de Respuesta**:
  ```json
  {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "username": "admin",
        "email": "admin@example.com",
        "firstName": "Admin",
        "lastName": "User",
        "phoneNumber": null,
        "lastLoginDate": "2025-04-12T10:00:00Z",
        "status": 1,
        "userType": 1,
        "roles": ["Administrador"],
        "createdAt": "2025-04-11T12:00:00Z",
        "createdBy": "System",
        "updatedAt": null,
        "updatedBy": null
      }
    ],
    "pageNumber": 1,
    "totalPages": 1,
    "totalCount": 1,
    "hasPreviousPage": false,
    "hasNextPage": false
  }
  ```

### Obtener Usuarios por Rol Paginados

- **Método**: GET
- **URL**: `/api/users/by-role/{roleId}/paged`
- **Descripción**: Obtiene usuarios con un rol específico, con paginación y ordenación.
- **Autenticación**: Requerida
- **Parámetros de Ruta**:
  - `roleId` (GUID): ID del rol
- **Parámetros de Consulta**:
  - `PageNumber` (int): Número de página (por defecto: 1)
  - `PageSize` (int): Tamaño de página (por defecto: 10, máximo: 50)
  - `SortBy` (string): Campo por el que ordenar (por defecto: "Id")
  - `SortDirection` (string): Dirección de ordenación ("asc" o "desc", por defecto: "asc")
- **Ejemplo de Solicitud**: `GET /api/users/by-role/3fa85f64-5717-4562-b3fc-2c963f66afa6/paged?PageNumber=1&PageSize=10&SortBy=Username&SortDirection=asc`
- **Ejemplo de Respuesta**:
  ```json
  {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "username": "admin",
        "email": "admin@example.com",
        "firstName": "Admin",
        "lastName": "User",
        "phoneNumber": null,
        "lastLoginDate": "2025-04-12T10:00:00Z",
        "status": 1,
        "userType": 1,
        "roles": ["Administrador"],
        "createdAt": "2025-04-11T12:00:00Z",
        "createdBy": "System",
        "updatedAt": null,
        "updatedBy": null
      }
    ],
    "pageNumber": 1,
    "totalPages": 1,
    "totalCount": 1,
    "hasPreviousPage": false,
    "hasNextPage": false
  }
  ```

## Códigos de Estado HTTP

La API utiliza los siguientes códigos de estado HTTP:

- **200 OK**: La solicitud se ha procesado correctamente.
- **201 Created**: El recurso se ha creado correctamente.
- **204 No Content**: La solicitud se ha procesada correctamente, pero no hay contenido para devolver.
- **400 Bad Request**: La solicitud es incorrecta o contiene datos inválidos.
- **401 Unauthorized**: No se ha proporcionado un token de autenticación válido.
- **403 Forbidden**: El usuario no tiene permiso para acceder al recurso.
- **404 Not Found**: El recurso solicitado no existe.
- **500 Internal Server Error**: Error interno del servidor.

## Manejo de Errores

La API devuelve errores en un formato consistente:

```json
{
  "error": "Mensaje de error descriptivo"
}
```

Para errores de validación, se devuelve una lista de errores:

```json
{
  "errors": [
    "El nombre es obligatorio",
    "El correo electrónico no tiene un formato válido"
  ]
}
```

## Flujo de Trabajo Recomendado para Pruebas

1. **Autenticación**:
   - Obtenga un token JWT mediante el endpoint de login
   - Configure Swagger o su cliente HTTP para incluir el token en todas las solicitudes

2. **Gestión de Roles**:
   - Cree varios roles con diferentes nombres y descripciones
   - Obtenga la lista de roles para verificar que se hayan creado correctamente
   - Actualice un rol existente
   - Intente eliminar un rol

3. **Gestión de Permisos**:
   - Cree varios permisos con diferentes nombres y descripciones
   - Obtenga la lista de permisos para verificar que se hayan creado correctamente
   - Actualice un permiso existente
   - Intente eliminar un permiso

4. **Gestión de Usuarios**:
   - Cree varios usuarios con diferentes roles
   - Obtenga la lista de usuarios para verificar que se hayan creado correctamente
   - Actualice un usuario existente, cambiando sus roles
   - Cambie la contraseña de un usuario
   - Obtenga usuarios por rol para verificar la asignación correcta
   - Intente eliminar un usuario

## Consideraciones de Seguridad

- Todos los endpoints requieren autenticación mediante JWT
- Las contraseñas se almacenan en texto plano en esta versión de desarrollo (en producción deberían estar hasheadas)
- La eliminación de registros es lógica, no física, para mantener la integridad referencial
- Los tokens JWT tienen un tiempo de expiración configurado en `appsettings.json`
- Se recomienda utilizar HTTPS en entornos de producción para proteger la transmisión de datos

## Limitaciones Actuales

- No se implementa paginación para endpoints que devuelven listas grandes
- Las contraseñas se almacenan en texto plano (esto debe cambiarse en producción)
- No hay validación exhaustiva de datos de entrada
- No se implementa rate limiting para prevenir ataques de fuerza bruta

## Solución de Problemas

Si encuentra algún problema al utilizar la API, verifique lo siguiente:

1. Asegúrese de que la base de datos esté correctamente configurada y las migraciones aplicadas
2. Verifique que está utilizando un token JWT válido y no expirado
3. Compruebe que los IDs utilizados en las solicitudes existen en la base de datos
4. Revise los mensajes de error devueltos por la API para obtener más información

## Próximos Pasos

- Implementar validación de datos más robusta
- Añadir paginación para endpoints que devuelven listas grandes
- Implementar filtrado y ordenación avanzada
- Mejorar la seguridad con hash de contraseñas y políticas de contraseñas
- Implementar auditoría detallada de operaciones

## Ejemplos de Uso con Curl

### Autenticación

```bash
curl -X POST "http://localhost:5106/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin"}'
```

### Obtener Todos los Roles (con token)

```bash
curl -X GET "http://localhost:5106/api/roles" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

### Crear un Nuevo Rol

```bash
curl -X POST "http://localhost:5106/api/roles" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{"name":"Supervisor","description":"Rol para supervisores de área"}'
```

```

```
