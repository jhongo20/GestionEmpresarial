# Guía de Integración con Sistemas Externos

## Introducción

Este documento proporciona una guía completa para que sistemas externos puedan integrarse con el Sistema de Gestión Empresarial. La API está diseñada siguiendo principios RESTful y utiliza autenticación JWT para garantizar la seguridad de las comunicaciones.

## Requisitos Previos

Para integrar correctamente con nuestra API, los sistemas externos deben cumplir con los siguientes requisitos:

1. Capacidad para realizar solicitudes HTTP (GET, POST, PUT, DELETE)
2. Soporte para autenticación mediante tokens JWT
3. Capacidad para enviar y recibir datos en formato JSON
4. Soporte para HTTPS

## Configuración de Acceso

### 1. Configuración de CORS

Actualmente, la API está configurada para permitir solicitudes desde orígenes específicos. Si necesita integrar un nuevo sistema, debe solicitar que su dominio sea agregado a la lista de orígenes permitidos.

```csharp
// Configuración actual de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("http://localhost:4200") // Frontend origin
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});
```

Para entornos de producción, se recomienda:

1. Especificar exactamente los orígenes permitidos (no usar `AllowAnyOrigin()`)
2. Limitar los métodos HTTP permitidos a los necesarios
3. Especificar los encabezados permitidos en lugar de permitir cualquiera

### 2. Autenticación y Autorización

La API utiliza autenticación basada en tokens JWT. Para acceder a los endpoints protegidos, los sistemas externos deben:

1. Obtener un token de autenticación mediante el endpoint de login
2. Incluir el token en el encabezado `Authorization` de todas las solicitudes posteriores

## Proceso de Integración

### Paso 1: Obtener un Token de Autenticación

Para obtener un token JWT, envíe una solicitud POST al endpoint de autenticación:

```http
POST /api/Auth/login
Content-Type: application/json

{
  "username": "usuario_sistema",
  "password": "contraseña_segura"
}
```

Respuesta exitosa:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "6ce32558-4d85-4e50-9e57-4f2bd1f67c3a",
  "expiration": "2025-04-13T01:10:51Z",
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "username": "usuario_sistema",
    "email": "sistema@ejemplo.com",
    "roles": ["Sistema"]
  }
}
```

### Paso 2: Usar el Token en Solicitudes Posteriores

Incluya el token JWT en el encabezado `Authorization` de todas las solicitudes:

```http
GET /api/Users
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Paso 3: Renovar el Token

Cuando el token JWT expire, puede obtener uno nuevo utilizando el token de actualización:

```http
POST /api/Auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "6ce32558-4d85-4e50-9e57-4f2bd1f67c3a"
}
```

## Endpoints Principales

### Usuarios

- `GET /api/Users`: Obtener lista de usuarios
- `GET /api/Users/{id}`: Obtener usuario por ID
- `POST /api/Users`: Crear nuevo usuario
- `PUT /api/Users/{id}`: Actualizar usuario existente
- `DELETE /api/Users/{id}`: Eliminar usuario

### Roles

- `GET /api/Roles`: Obtener lista de roles
- `GET /api/Roles/{id}`: Obtener rol por ID
- `POST /api/Roles`: Crear nuevo rol
- `PUT /api/Roles/{id}`: Actualizar rol existente
- `DELETE /api/Roles/{id}`: Eliminar rol

### Módulos

- `GET /api/Modules`: Obtener lista de módulos
- `GET /api/Modules/{id}`: Obtener módulo por ID
- `POST /api/Modules`: Crear nuevo módulo
- `PUT /api/Modules/{id}`: Actualizar módulo existente
- `DELETE /api/Modules/{id}`: Eliminar módulo

## Consideraciones para la Integración

### 1. Creación de Usuarios de Sistema

Para integraciones entre sistemas, se recomienda crear usuarios específicos con el rol "Sistema" que tengan permisos limitados a las operaciones necesarias. Estos usuarios deben:

- Tener contraseñas fuertes y rotadas periódicamente
- Tener permisos estrictamente limitados a las funcionalidades requeridas
- Ser monitoreados para detectar actividades sospechosas

### 2. Manejo de Errores

La API devuelve códigos de estado HTTP estándar:

- `200 OK`: Operación exitosa
- `201 Created`: Recurso creado exitosamente
- `400 Bad Request`: Solicitud incorrecta
- `401 Unauthorized`: Autenticación requerida
- `403 Forbidden`: Sin permisos para acceder al recurso
- `404 Not Found`: Recurso no encontrado
- `500 Internal Server Error`: Error del servidor

Ejemplo de respuesta de error:

```json
{
  "errors": [
    "El nombre de usuario ya está en uso."
  ]
}
```

### 3. Paginación

Para endpoints que devuelven listas grandes, la API implementa paginación:

```http
GET /api/Users?pageNumber=1&pageSize=10
```

Respuesta:

```json
{
  "items": [...],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 57,
  "totalPages": 6,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

### 4. Filtrado y Ordenación

La API soporta filtrado y ordenación:

```http
GET /api/Users?filter=email:contains:example.com&sort=lastName:asc,firstName:asc
```

## Mejores Prácticas para la Integración

### 1. Seguridad

- **No almacenar tokens JWT en almacenamiento local sin cifrar**
- **Implementar HTTPS en todas las comunicaciones**
- **Validar todos los datos de entrada y salida**
- **Limitar el número de intentos de autenticación**
- **Implementar registro de auditoría para todas las operaciones críticas**

### 2. Rendimiento

- **Implementar caché para datos que no cambian frecuentemente**
- **Utilizar paginación para conjuntos de datos grandes**
- **Minimizar el número de solicitudes mediante la selección de campos específicos**
- **Implementar manejo de errores robusto con reintentos para errores transitorios**

### 3. Resiliencia

- **Implementar circuit breakers para evitar cascadas de fallos**
- **Utilizar timeouts adecuados para las solicitudes**
- **Implementar reintentos con backoff exponencial para errores transitorios**
- **Monitorear la salud de la integración**

## Configuración para Diferentes Entornos

### Desarrollo

```
API URL: http://localhost:5106
```

### Pruebas

```
API URL: https://test-api.gestionempresarial.com
```

### Producción

```
API URL: https://api.gestionempresarial.com
```

## Ejemplos de Integración

### Ejemplo 1: Autenticación y Obtención de Usuarios (C#)

```csharp
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class GestionEmpresarialClient
{
    private readonly HttpClient _httpClient;
    private string _token;

    public GestionEmpresarialClient(string baseUrl)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        };
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var loginData = new
        {
            username,
            password
        };

        var content = new StringContent(
            JsonSerializer.Serialize(loginData),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync("/api/Auth/login", content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var authResult = JsonSerializer.Deserialize<AuthResult>(responseContent);
            _token = authResult.Token;
            
            // Configurar el token para futuras solicitudes
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _token);
            
            return true;
        }

        return false;
    }

    public async Task<string> GetUsersAsync()
    {
        var response = await _httpClient.GetAsync("/api/Users");
        
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        
        throw new Exception($"Error: {response.StatusCode}");
    }
}

public class AuthResult
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime Expiration { get; set; }
    public UserDto User { get; set; }
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; }
}
```

### Ejemplo 2: Autenticación y Obtención de Usuarios (JavaScript)

```javascript
class GestionEmpresarialClient {
  constructor(baseUrl) {
    this.baseUrl = baseUrl;
    this.token = null;
  }

  async login(username, password) {
    try {
      const response = await fetch(`${this.baseUrl}/api/Auth/login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ username, password })
      });

      if (!response.ok) {
        throw new Error(`Error: ${response.status}`);
      }

      const authResult = await response.json();
      this.token = authResult.token;
      return true;
    } catch (error) {
      console.error('Login failed:', error);
      return false;
    }
  }

  async getUsers() {
    if (!this.token) {
      throw new Error('Not authenticated');
    }

    try {
      const response = await fetch(`${this.baseUrl}/api/Users`, {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${this.token}`
        }
      });

      if (!response.ok) {
        throw new Error(`Error: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Failed to get users:', error);
      throw error;
    }
  }
}

// Uso
async function example() {
  const client = new GestionEmpresarialClient('https://api.gestionempresarial.com');
  const loggedIn = await client.login('sistema_externo', 'password');
  
  if (loggedIn) {
    const users = await client.getUsers();
    console.log(users);
  }
}
```

## Soporte y Contacto

Para obtener ayuda con la integración, contacte al equipo de soporte:

- Email: soporte@gestionempresarial.com
- Teléfono: +57 1 234 5678
- Horario de atención: Lunes a Viernes, 8:00 AM - 6:00 PM (GMT-5)

## Registro de Cambios en la API

| Versión | Fecha | Cambios |
|---------|-------|---------|
| 1.0.0 | 2025-01-15 | Versión inicial de la API |
| 1.1.0 | 2025-03-10 | Agregados endpoints para gestión de módulos |
| 1.2.0 | 2025-04-12 | Implementada verificación de usuarios internos en directorio activo |

## Preguntas Frecuentes

### ¿Cómo puedo solicitar acceso a la API?

Para solicitar acceso, envíe un correo electrónico a api-access@gestionempresarial.com con la siguiente información:
- Nombre de la aplicación
- Propósito de la integración
- Dominio desde el que se realizarán las solicitudes
- Contacto técnico

### ¿Qué debo hacer si el token expira?

Utilice el endpoint `/api/Auth/refresh-token` con el token de actualización para obtener un nuevo token JWT.

### ¿Cómo puedo reportar un problema con la API?

Envíe un correo electrónico a api-support@gestionempresarial.com con la siguiente información:
- Descripción detallada del problema
- Endpoint afectado
- Código de respuesta recibido
- Cuerpo de la solicitud (sin información sensible)
- Fecha y hora del problema
