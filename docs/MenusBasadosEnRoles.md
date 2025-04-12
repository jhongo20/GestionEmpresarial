# Sistema de Menús Basados en Roles

## Introducción

El sistema de menús basados en roles permite que cada rol tenga un menú personalizado con módulos y rutas específicos. Esto facilita la implementación de una interfaz de usuario dinámica que se adapta a los permisos y responsabilidades de cada usuario.

## Estructura del Sistema

### Entidades

1. **Module**: Representa un módulo o sección principal del sistema.
   - `Id`: Identificador único del módulo
   - `Name`: Nombre del módulo
   - `Description`: Descripción del módulo
   - `Icon`: Icono del módulo (para la interfaz de usuario)
   - `Path`: Ruta base del módulo
   - `Order`: Orden de visualización
   - `IsActive`: Indica si el módulo está activo
   - `IsDeleted`: Indica si el módulo ha sido eliminado lógicamente

2. **Route**: Representa una ruta o página específica dentro de un módulo.
   - `Id`: Identificador único de la ruta
   - `Name`: Nombre de la ruta
   - `Path`: Ruta URL
   - `Icon`: Icono de la ruta (para la interfaz de usuario)
   - `Order`: Orden de visualización
   - `IsActive`: Indica si la ruta está activa
   - `IsDeleted`: Indica si la ruta ha sido eliminada lógicamente
   - `ModuleId`: Identificador del módulo al que pertenece la ruta

3. **RoleModule**: Asociación entre roles y módulos.
   - `Id`: Identificador único de la asociación
   - `RoleId`: Identificador del rol
   - `ModuleId`: Identificador del módulo
   - `IsActive`: Indica si la asociación está activa
   - `IsDeleted`: Indica si la asociación ha sido eliminada lógicamente

4. **RoleRoute**: Asociación entre roles y rutas.
   - `Id`: Identificador único de la asociación
   - `RoleId`: Identificador del rol
   - `RouteId`: Identificador de la ruta
   - `IsActive`: Indica si la asociación está activa
   - `IsDeleted`: Indica si la asociación ha sido eliminada lógicamente

### Servicios

1. **ModuleService**: Gestiona las operaciones CRUD para los módulos.
   - `GetAllModulesAsync()`: Obtiene todos los módulos activos
   - `GetModulesPagedAsync()`: Obtiene módulos con paginación
   - `GetModuleByIdAsync()`: Obtiene un módulo por su ID
   - `CreateModuleAsync()`: Crea un nuevo módulo
   - `UpdateModuleAsync()`: Actualiza un módulo existente
   - `DeleteModuleAsync()`: Elimina lógicamente un módulo
   - `ToggleModuleStatusAsync()`: Activa o desactiva un módulo
   - `GetModulesByRoleAsync()`: Obtiene los módulos asignados a un rol

2. **RouteService**: Gestiona las operaciones CRUD para las rutas.
   - `GetAllRoutesAsync()`: Obtiene todas las rutas activas
   - `GetRoutesPagedAsync()`: Obtiene rutas con paginación
   - `GetRouteByIdAsync()`: Obtiene una ruta por su ID
   - `CreateRouteAsync()`: Crea una nueva ruta
   - `UpdateRouteAsync()`: Actualiza una ruta existente
   - `DeleteRouteAsync()`: Elimina lógicamente una ruta
   - `ToggleRouteStatusAsync()`: Activa o desactiva una ruta
   - `GetRoutesByModuleAsync()`: Obtiene las rutas de un módulo específico
   - `GetRoutesByRoleAsync()`: Obtiene las rutas asignadas a un rol

3. **RoleService**: Incluye métodos para asignar módulos y rutas a roles.
   - `AssignModuleToRoleAsync()`: Asigna un módulo a un rol
   - `RemoveModuleFromRoleAsync()`: Elimina un módulo de un rol
   - `AssignRouteToRoleAsync()`: Asigna una ruta a un rol
   - `RemoveRouteFromRoleAsync()`: Elimina una ruta de un rol

4. **MenuService**: Genera menús dinámicos basados en los roles del usuario.
   - `GetMenuByUserIdAsync()`: Obtiene el menú para un usuario específico
   - `GetMenuByRoleIdAsync()`: Obtiene el menú para un rol específico

## Endpoints API

### Módulos

- `GET /api/modules`: Obtiene todos los módulos
- `GET /api/modules/paged`: Obtiene módulos con paginación
- `GET /api/modules/{id}`: Obtiene un módulo por su ID
- `POST /api/modules`: Crea un nuevo módulo
- `PUT /api/modules/{id}`: Actualiza un módulo existente
- `DELETE /api/modules/{id}`: Elimina un módulo
- `PUT /api/modules/{id}/toggle-status`: Activa o desactiva un módulo
- `GET /api/modules/by-role/{roleId}`: Obtiene los módulos asignados a un rol

### Rutas

- `GET /api/routes`: Obtiene todas las rutas
- `GET /api/routes/paged`: Obtiene rutas con paginación
- `GET /api/routes/{id}`: Obtiene una ruta por su ID
- `POST /api/routes`: Crea una nueva ruta
- `PUT /api/routes/{id}`: Actualiza una ruta existente
- `DELETE /api/routes/{id}`: Elimina una ruta
- `PUT /api/routes/{id}/toggle-status`: Activa o desactiva una ruta
- `GET /api/routes/by-module/{moduleId}`: Obtiene las rutas de un módulo específico
- `GET /api/routes/by-role/{roleId}`: Obtiene las rutas asignadas a un rol

### Roles (Endpoints relacionados con módulos y rutas)

- `POST /api/roles/assign-module`: Asigna un módulo a un rol
- `DELETE /api/roles/remove-module/{roleId}/{moduleId}`: Elimina un módulo de un rol
- `POST /api/roles/assign-route`: Asigna una ruta a un rol
- `DELETE /api/roles/remove-route/{roleId}/{routeId}`: Elimina una ruta de un rol

### Menús

- `GET /api/menus/my-menu`: Obtiene el menú para el usuario autenticado
- `GET /api/menus/by-role/{roleId}`: Obtiene el menú para un rol específico

## Uso en el Frontend

Para implementar el menú dinámico en el frontend, siga estos pasos:

1. Autenticar al usuario y obtener su token JWT
2. Realizar una solicitud a `GET /api/menus/my-menu` con el token de autenticación
3. Utilizar la respuesta para construir el menú de navegación
4. Cada elemento del menú contendrá:
   - Módulos como elementos principales del menú
   - Rutas como elementos secundarios (submenús) dentro de cada módulo

### Ejemplo de respuesta de la API de menú

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Dashboard",
    "icon": "dashboard",
    "path": "/dashboard",
    "order": 1,
    "isActive": true,
    "children": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
        "name": "Inicio",
        "icon": "home",
        "path": "/dashboard",
        "order": 1,
        "isActive": true
      },
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa8",
        "name": "Estadísticas",
        "icon": "bar_chart",
        "path": "/dashboard/stats",
        "order": 2,
        "isActive": true
      }
    ]
  },
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa9",
    "name": "Usuarios",
    "icon": "people",
    "path": "/users",
    "order": 2,
    "isActive": true,
    "children": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afb1",
        "name": "Lista de Usuarios",
        "icon": "list",
        "path": "/users/list",
        "order": 1,
        "isActive": true
      }
    ]
  }
]
```

## Migración de Base de Datos

Para aplicar los cambios en la base de datos, puede utilizar:

1. **Migración automática de Entity Framework Core**:
   ```
   dotnet ef migrations add AddModulesAndRoutes --project src/GestionEmpresarial.Infrastructure --startup-project src/GestionEmpresarial.API
   dotnet ef database update --project src/GestionEmpresarial.Infrastructure --startup-project src/GestionEmpresarial.API
   ```

2. **Script SQL manual**:
   Ejecute el script `AddModulesAndRoutes.sql` ubicado en `src/GestionEmpresarial.Infrastructure/Persistence/Migrations/Manual/` directamente en su base de datos SQL Server.

## Consideraciones de Seguridad

- Los endpoints de la API están protegidos con autenticación JWT
- Solo los usuarios autenticados pueden acceder a los menús
- Los menús se filtran según los roles del usuario
- Se implementa eliminación lógica para mantener la integridad de los datos

## Próximos Pasos

1. Implementar pruebas unitarias para los servicios de módulos, rutas y menús
2. Desarrollar componentes de frontend para renderizar el menú dinámico
3. Crear una interfaz de administración para gestionar módulos y rutas
4. Implementar caché para mejorar el rendimiento de la generación de menús

## Usuarios de Prueba

El sistema incluye los siguientes usuarios de prueba para facilitar las pruebas del sistema de menús basados en roles:

| Usuario    | Contraseña | Rol           | Acceso                                      |
|------------|------------|---------------|---------------------------------------------|
| admin      | Admin123!  | Administrador | Acceso completo a todos los módulos y rutas |
| supervisor | Admin123!  | Supervisor    | Acceso a Dashboard, Usuarios y Roles        |
| user       | Admin123!  | Usuario       | Acceso limitado solo al Dashboard           |
| testadmin  | test123    | Administrador | Acceso completo (usuario alternativo)       |

## Ejemplo de Respuesta de Menú

A continuación se muestra un ejemplo de la respuesta JSON que devuelve el endpoint `/api/menus/my-menu` para un usuario con rol de Administrador:

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
  },
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Usuarios",
    "icon": "people",
    "path": "/users",
    "order": 2,
    "children": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Lista de Usuarios",
        "path": "/users/list",
        "icon": "list",
        "order": 1
      },
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Crear Usuario",
        "path": "/users/create",
        "icon": "add",
        "order": 2
      },
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Editar Usuario",
        "path": "/users/edit",
        "icon": "edit",
        "order": 3
      }
    ]
  },
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Roles",
    "icon": "security",
    "path": "/roles",
    "order": 3,
    "children": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Lista de Roles",
        "path": "/roles/list",
        "icon": "list",
        "order": 1
      },
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Crear Rol",
        "path": "/roles/create",
        "icon": "add",
        "order": 2
      },
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Editar Rol",
        "path": "/roles/edit",
        "icon": "edit",
        "order": 3
      },
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Asignar Permisos",
        "path": "/roles/permissions",
        "icon": "security",
        "order": 4
      }
    ]
  },
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Configuración",
    "icon": "settings",
    "path": "/settings",
    "order": 4,
    "children": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "General",
        "path": "/settings/general",
        "icon": "settings",
        "order": 1
      },
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Seguridad",
        "path": "/settings/security",
        "icon": "security",
        "order": 2
      },
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Notificaciones",
        "path": "/settings/notifications",
        "icon": "notifications",
        "order": 3
      }
    ]
  }
]
```

## Cómo Probar el Sistema de Menús

### Configuración Inicial

Para configurar el sistema de menús basados en roles, siga estos pasos:

1. Ejecute los scripts SQL de migración manual:

```bash
# Desde la carpeta raíz del proyecto
sqlcmd -S localhost -d GestionEmpresarialNuevoDB -i "src\GestionEmpresarial.Infrastructure\Persistence\Migrations\Manual\FixedModulesAndRoutes.sql" -E
sqlcmd -S localhost -d GestionEmpresarialNuevoDB -i "src\GestionEmpresarial.Infrastructure\Persistence\Migrations\Manual\AddRoleAssignments.sql" -E
sqlcmd -S localhost -d GestionEmpresarialNuevoDB -i "src\GestionEmpresarial.Infrastructure\Persistence\Migrations\Manual\AddTestUsers.sql" -E
```

2. Inicie la aplicación:

```bash
cd src/GestionEmpresarial.API
dotnet run --urls=http://localhost:5108
```

### Prueba desde Swagger UI

1. Abra Swagger UI en su navegador: http://localhost:5108/swagger/index.html
2. Autentíquese usando el endpoint `/api/auth/login`:
   ```json
   {
     "username": "admin",
     "password": "Admin123!"
   }
   ```
   Si tiene problemas con este usuario, pruebe con el usuario alternativo:
   ```json
   {
     "username": "testadmin",
     "password": "test123"
   }
   ```
3. Copie el token JWT de la respuesta
4. Haga clic en el botón "Authorize" en la parte superior de la página de Swagger
5. Pegue el token en el campo "Value" con el formato: `Bearer {su-token-jwt}`
6. Pruebe el endpoint `/api/menus/my-menu` para obtener el menú del usuario autenticado

### Prueba desde la Página de Prueba HTML

También puede utilizar la página de prueba HTML incluida en el proyecto:

1. Abra el archivo `tests/MenuTest.html` en su navegador
2. Seleccione un usuario de prueba y haga clic en "Iniciar Sesión"
3. Haga clic en "Obtener Menú" para ver el menú del usuario

### Prueba de Diferentes Roles

Para probar cómo cambia el menú según el rol del usuario:

1. Autentíquese con diferentes usuarios (admin, supervisor, user)
2. Compare las diferencias en los menús que se muestran
3. Observe cómo el usuario "admin" tiene acceso a todos los módulos, mientras que "user" solo tiene acceso al Dashboard

## Solución de Problemas

### Problemas de Autenticación

Si experimenta problemas al autenticarse con los usuarios de prueba:

1. Verifique que los scripts SQL se hayan ejecutado correctamente
2. Intente con el usuario alternativo "testadmin" y la contraseña "test123"
3. Ejecute el script `FixAdminPassword.sql` para restablecer la contraseña del usuario admin:
   ```bash
   sqlcmd -S localhost -d GestionEmpresarialNuevoDB -i "src\GestionEmpresarial.Infrastructure\Persistence\Migrations\Manual\FixAdminPassword.sql" -E
   ```

### Problemas con los Menús

Si los menús no se muestran correctamente:

1. Verifique que el usuario tenga roles asignados
2. Compruebe que los roles tengan módulos y rutas asignados
3. Asegúrese de que los módulos y rutas estén marcados como activos (IsActive = true)
4. Revise los registros de la aplicación para detectar posibles errores

## Integración con Frontend

Para integrar el sistema de menús basados en roles con una aplicación frontend:

1. Autentique al usuario y obtenga el token JWT
2. Realice una solicitud GET a `/api/menus/my-menu` con el token JWT en el encabezado de autorización
3. Utilice la respuesta JSON para generar dinámicamente el menú de navegación
4. Implemente la lógica de enrutamiento en el frontend para manejar las rutas definidas en el menú

### Ejemplo de Integración con React

```jsx
import React, { useState, useEffect } from 'react';
import axios from 'axios';

const Menu = () => {
  const [menuItems, setMenuItems] = useState([]);
  const [loading, setLoading] = useState(true);
  
  useEffect(() => {
    const fetchMenu = async () => {
      try {
        const token = localStorage.getItem('token');
        const response = await axios.get('http://localhost:5108/api/menus/my-menu', {
          headers: {
            Authorization: `Bearer ${token}`
          }
        });
        setMenuItems(response.data);
        setLoading(false);
      } catch (error) {
        console.error('Error fetching menu:', error);
        setLoading(false);
      }
    };
    
    fetchMenu();
  }, []);
  
  if (loading) {
    return <div>Cargando menú...</div>;
  }
  
  return (
    <nav className="sidebar">
      <ul>
        {menuItems.map(module => (
          <li key={module.id}>
            <a href={module.path}>
              <i className={`icon-${module.icon}`}></i>
              <span>{module.name}</span>
            </a>
            {module.children && module.children.length > 0 && (
              <ul className="submenu">
                {module.children.map(route => (
                  <li key={route.id}>
                    <a href={route.path}>
                      <i className={`icon-${route.icon}`}></i>
                      <span>{route.name}</span>
                    </a>
                  </li>
                ))}
              </ul>
            )}
          </li>
        ))}
      </ul>
    </nav>
  );
};

export default Menu;
```

## Conclusión

El sistema de menús basados en roles proporciona una forma flexible y segura de controlar qué partes de la aplicación son accesibles para cada usuario según su rol. Esto mejora la seguridad y la experiencia del usuario al mostrar solo las opciones relevantes para sus responsabilidades.

La arquitectura modular permite una fácil extensión y personalización del sistema, facilitando la adición de nuevos módulos, rutas y roles según sea necesario para adaptarse a los requisitos cambiantes de la aplicación.
