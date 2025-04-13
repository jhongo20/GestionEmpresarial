# Sistema de Caché para Consultas LDAP

## Descripción General

Se ha implementado un sistema de caché en memoria para optimizar las consultas LDAP frecuentes, reduciendo la carga en el servidor de directorio activo y mejorando el rendimiento general del sistema. Esta implementación sigue las recomendaciones de mejora propuestas para el sistema.

## Características Implementadas

1. **Caché en Memoria**: Utiliza el sistema de caché en memoria de .NET Core (`IMemoryCache`) para almacenar temporalmente los resultados de consultas LDAP frecuentes.

2. **Tiempos de Expiración Configurables**: Se han definido diferentes tiempos de expiración según el tipo de datos:
   - Verificación de existencia de usuario: 30 minutos
   - Atributos de usuario (correo, nombre): 2 horas

3. **Actualización Automática**: La caché se actualiza automáticamente después de operaciones exitosas, como autenticaciones.

4. **Configuración Optimizada**: Se ha configurado el sistema de caché con límites de tamaño y frecuencia de escaneo para mantener un rendimiento óptimo.

## Componentes Modificados

### 1. Servicio LDAP (`LdapService.cs`)

Se ha modificado el servicio LDAP para implementar la caché en los siguientes métodos:

- `UserExistsAsync`: Verifica si un usuario existe en el directorio LDAP
- `GetUserEmailAsync`: Obtiene el correo electrónico de un usuario
- `GetUserDisplayNameAsync`: Obtiene el nombre de visualización de un usuario
- `AuthenticateAsync`: Actualiza la caché después de una autenticación exitosa

### 2. Configuración de Dependencias (`DependencyInjection.cs`)

Se ha agregado la configuración del servicio de caché en memoria:

```csharp
// Configuración del servicio de caché en memoria
services.AddMemoryCache(options =>
{
    // Configurar opciones de caché
    options.SizeLimit = 1024; // Tamaño máximo en MB
    options.ExpirationScanFrequency = TimeSpan.FromMinutes(5); // Frecuencia de escaneo
});
```

## Funcionamiento

### Verificación de Existencia de Usuario

```csharp
public async Task<bool> UserExistsAsync(string username)
{
    // Verificar si la existencia del usuario ya está en caché
    var cacheKey = string.Format(CACHE_KEY_USER_EXISTS, username);
    if (_cache.TryGetValue(cacheKey, out bool userExists))
    {
        return userExists;
    }

    // Si no está en caché, consultar al directorio LDAP
    // ...

    // Almacenar el resultado en caché
    var cacheEntryOptions = new MemoryCacheEntryOptions()
        .SetSlidingExpiration(USER_EXISTS_CACHE_DURATION);
    _cache.Set(cacheKey, userExists, cacheEntryOptions);

    return userExists;
}
```

### Obtención de Atributos de Usuario

```csharp
public async Task<string> GetUserEmailAsync(string username)
{
    // Verificar si el correo electrónico ya está en caché
    var cacheKey = string.Format(CACHE_KEY_USER_EMAIL, username);
    if (_cache.TryGetValue(cacheKey, out string userEmail))
    {
        return userEmail;
    }

    // Si no está en caché, consultar al directorio LDAP
    // ...

    // Almacenar el resultado en caché
    var cacheEntryOptions = new MemoryCacheEntryOptions()
        .SetSlidingExpiration(USER_ATTRIBUTES_CACHE_DURATION);
    _cache.Set(cacheKey, userEmail, cacheEntryOptions);

    return userEmail;
}
```

## Beneficios

1. **Mejora del Rendimiento**: Reduce significativamente el tiempo de respuesta para consultas repetidas al directorio LDAP.

2. **Reducción de Carga**: Disminuye la carga en el servidor de directorio activo al evitar consultas redundantes.

3. **Mayor Disponibilidad**: El sistema puede seguir funcionando temporalmente incluso si el servidor LDAP está temporalmente inaccesible, utilizando los datos en caché.

4. **Escalabilidad Mejorada**: Permite manejar un mayor número de usuarios concurrentes sin sobrecargar el servidor LDAP.

## Consideraciones de Seguridad

1. **Autenticación**: Por razones de seguridad, la autenticación siempre se realiza directamente contra el servidor LDAP, sin utilizar caché.

2. **Tiempo de Vida Limitado**: Los datos en caché tienen un tiempo de vida limitado para garantizar que los cambios en el directorio activo se reflejen en el sistema dentro de un período razonable.

3. **Invalidación de Caché**: Se recomienda implementar en el futuro un mecanismo para invalidar manualmente la caché en caso de cambios importantes en el directorio activo.

## Configuración Recomendada

La configuración actual está optimizada para un entorno de producción típico, pero puede ajustarse según las necesidades específicas:

- **Tamaño de Caché**: Ajustar `SizeLimit` según la memoria disponible y el número de usuarios.
- **Tiempos de Expiración**: Modificar `USER_EXISTS_CACHE_DURATION` y `USER_ATTRIBUTES_CACHE_DURATION` según la frecuencia de cambios en el directorio activo.
- **Frecuencia de Escaneo**: Ajustar `ExpirationScanFrequency` según la carga del sistema.

## Próximos Pasos

1. **Monitoreo de Rendimiento**: Implementar métricas para monitorear el rendimiento de la caché (tasa de aciertos, uso de memoria).

2. **Caché Distribuida**: Considerar la migración a una solución de caché distribuida como Redis para entornos con múltiples instancias de la aplicación.

3. **Invalidación Selectiva**: Implementar un mecanismo para invalidar selectivamente entradas de caché basado en eventos externos (como cambios en el directorio activo).

4. **Compresión de Datos**: Evaluar la implementación de compresión para reducir el uso de memoria en la caché.
