# Sistema de Auditoría

## Introducción

El sistema de auditoría implementado en GestionEmpresarial permite registrar y monitorear todas las acciones importantes realizadas en la aplicación. Esto proporciona un registro completo de actividades para fines de seguridad, cumplimiento normativo y resolución de problemas.

## Características Principales

1. **Registro Automático de Acciones**:
   - Captura automáticamente operaciones CRUD en entidades importantes
   - Registra acciones de autenticación (inicio de sesión, cierre de sesión)
   - Almacena cambios en permisos y roles

2. **Información Detallada**:
   - Usuario que realizó la acción
   - Fecha y hora exacta
   - Tipo de acción realizada
   - Entidad o tabla afectada
   - Valores anteriores y nuevos (para actualizaciones)
   - Dirección IP y agente de usuario

3. **Middleware Personalizado**:
   - Captura información adicional de las solicitudes HTTP
   - Registra automáticamente solicitudes POST, PUT y DELETE
   - Captura información de autenticación

4. **Filtrado Avanzado**:
   - Por usuario
   - Por tabla o entidad
   - Por tipo de acción
   - Por rango de fechas

5. **Visualización Amigable**:
   - Interfaz HTML para explorar registros
   - Paginación de resultados
   - Visualización formateada de valores JSON

## Arquitectura

### Componentes Principales

1. **Entidad AuditLog**:
   - Modelo de datos para almacenar registros de auditoría
   - Incluye todos los campos necesarios para un registro completo

2. **AuditService**:
   - Servicio para registrar acciones en la base de datos
   - Proporciona métodos para consultar y filtrar registros

3. **AuditMiddleware**:
   - Middleware para capturar información de solicitudes HTTP
   - Registra automáticamente acciones importantes

4. **AuditController**:
   - Endpoints para consultar registros de auditoría
   - Proporciona filtrado y paginación

### Flujo de Trabajo

1. Una solicitud HTTP llega al servidor
2. El middleware de auditoría intercepta la solicitud
3. Si es una acción importante (POST, PUT, DELETE), se captura información
4. La solicitud continúa su procesamiento normal
5. Después de que se completa la solicitud, se registra la acción en la base de datos
6. Los administradores pueden consultar los registros a través de la API

## Modelo de Datos

La entidad `AuditLog` contiene los siguientes campos:

| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | Guid | Identificador único del registro |
| UserId | string | ID del usuario que realizó la acción |
| UserName | string | Nombre del usuario que realizó la acción |
| Type | string | Tipo de operación (Create, Update, Delete, Security) |
| TableName | string | Nombre de la tabla o entidad afectada |
| DateTime | DateTime | Fecha y hora de la acción |
| OldValues | string | Valores anteriores en formato JSON (para actualizaciones) |
| NewValues | string | Valores nuevos en formato JSON |
| AffectedColumns | string | Columnas afectadas por la acción |
| PrimaryKey | string | Clave primaria del registro afectado |
| Action | string | Acción específica realizada |
| IpAddress | string | Dirección IP desde donde se realizó la acción |
| UserAgent | string | Agente de usuario del navegador |

## API de Auditoría

### Endpoints Disponibles

1. **GET /api/audit**
   - Obtiene registros de auditoría con filtros y paginación
   - Parámetros:
     - `userId`: Filtrar por ID de usuario
     - `tableName`: Filtrar por nombre de tabla
     - `fromDate`: Fecha de inicio del rango
     - `toDate`: Fecha de fin del rango
     - `page`: Número de página (predeterminado: 1)
     - `pageSize`: Tamaño de página (predeterminado: 10)

2. **GET /api/audit/{id}**
   - Obtiene un registro de auditoría específico por su ID

3. **GET /api/audit/tables**
   - Obtiene la lista de tablas disponibles para filtrar

4. **GET /api/audit/actions**
   - Obtiene la lista de acciones disponibles para filtrar

### Ejemplos de Uso

#### Obtener Todos los Registros (Paginados)

```http
GET /api/audit?page=1&pageSize=10
```

#### Filtrar por Usuario

```http
GET /api/audit?userId=admin&page=1&pageSize=10
```

#### Filtrar por Tabla y Rango de Fechas

```http
GET /api/audit?tableName=Users&fromDate=2023-01-01T00:00:00Z&toDate=2023-12-31T23:59:59Z&page=1&pageSize=10
```

## Visualización de Registros

El sistema incluye una página HTML para visualizar y explorar los registros de auditoría:

1. Abra el archivo `tests/AuditTest.html` en su navegador
2. Inicie sesión con un usuario administrador
3. Utilice los filtros para encontrar registros específicos
4. Explore los detalles de cada acción registrada

## Configuración y Personalización

### Registrar Acciones Adicionales

Para registrar acciones personalizadas, puede utilizar el servicio de auditoría directamente:

```csharp
await _auditService.LogActionAsync(
    userId: "user123",
    userName: "John Doe",
    action: "CustomAction",
    tableName: "CustomEntity",
    primaryKey: "123",
    oldValues: null,
    newValues: "{\"property\":\"value\"}",
    affectedColumns: null,
    ipAddress: "127.0.0.1",
    userAgent: "Mozilla/5.0"
);
```

### Personalizar el Middleware

El middleware de auditoría se puede personalizar para capturar información adicional o modificar su comportamiento:

1. Edite la clase `AuditMiddleware.cs`
2. Ajuste la lógica de captura según sus necesidades
3. Modifique los tipos de solicitudes que se registran automáticamente

## Migración de Base de Datos

Para crear la tabla de auditoría en la base de datos, ejecute el siguiente script SQL:

```bash
sqlcmd -S localhost -d GestionEmpresarialNuevoDB -i "src\GestionEmpresarial.Infrastructure\Persistence\Migrations\Manual\CreateAuditLogsTable.sql" -E
```

## Mejores Prácticas

1. **Seguridad de Datos**:
   - Asegúrese de no registrar información sensible como contraseñas
   - Considere la posibilidad de enmascarar datos personales sensibles

2. **Rendimiento**:
   - Monitoree el tamaño de la tabla de auditoría
   - Implemente políticas de retención para datos antiguos
   - Considere archivar registros antiguos en una tabla separada

3. **Cumplimiento Normativo**:
   - Asegúrese de que los registros de auditoría cumplan con los requisitos regulatorios aplicables
   - Documente las políticas de retención de datos

4. **Monitoreo**:
   - Revise periódicamente los registros de auditoría para detectar actividades sospechosas
   - Configure alertas para acciones críticas

## Conclusión

El sistema de auditoría proporciona una visibilidad completa de todas las acciones importantes realizadas en la aplicación. Esto mejora la seguridad, facilita la resolución de problemas y ayuda a cumplir con los requisitos regulatorios.

La implementación flexible permite personalizar el sistema según las necesidades específicas del proyecto, mientras que la interfaz de usuario intuitiva facilita la exploración y análisis de los registros de auditoría.
