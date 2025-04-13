# Documentación de Usuarios Internos

## Descripción General

El sistema proporciona funcionalidades especiales para usuarios internos (con dominio @mintrabajo.gov.co), permitiendo un proceso de inicio de sesión y registro simplificado, así como verificación contra el directorio activo para garantizar que solo se puedan crear cuentas para usuarios reales de la organización.

## Características Principales

### 1. Inicio de Sesión Simplificado

Los usuarios internos pueden iniciar sesión utilizando solo su nombre de usuario, sin necesidad de incluir el dominio @mintrabajo.gov.co. Por ejemplo:

- Un usuario con correo `jperezc@mintrabajo.gov.co` puede iniciar sesión simplemente ingresando `jperezc` como nombre de usuario.
- También puede iniciar sesión con el correo completo `jperezc@mintrabajo.gov.co` si lo prefiere.

### 2. Registro Simplificado

Al crear usuarios internos, el sistema permite varias formas de especificar el correo electrónico:

- **Solo con el nombre de usuario**: Se puede proporcionar solo el nombre de usuario (ej. `jperezc`) y el sistema automáticamente agregará el dominio @mintrabajo.gov.co.
- **Con el correo completo**: También se puede proporcionar el correo completo (ej. `jperezc@mintrabajo.gov.co`).
- **Campo de correo vacío**: Si se deja el campo de correo vacío pero se proporciona un nombre de usuario, el sistema utilizará el nombre de usuario para generar el correo electrónico.

### 3. Verificación en Directorio Activo

Antes de crear un usuario interno, el sistema verifica que el usuario exista en el directorio activo:

- Si el usuario no existe en el directorio activo, se rechaza la creación y se muestra un mensaje de error.
- Solo se pueden crear cuentas para usuarios que realmente existen en el directorio activo de la organización.
- Esta verificación garantiza la consistencia entre el directorio activo y el sistema de gestión empresarial.

## Flujo de Autenticación

1. El usuario intenta iniciar sesión con su nombre de usuario (con o sin dominio @mintrabajo.gov.co).
2. El sistema detecta si es un usuario interno basado en el nombre de usuario o correo electrónico.
3. Si es un usuario interno, el sistema busca al usuario por el correo electrónico completo.
4. Si se encuentra el usuario, se procede con la autenticación normal (LDAP o local).

## Flujo de Registro

1. Se intenta crear un usuario con nombre de usuario y correo electrónico (que puede estar incompleto o vacío).
2. El sistema detecta si es un usuario interno basado en el correo electrónico o nombre de usuario.
3. Si es un usuario interno, se completa automáticamente el correo electrónico con el dominio @mintrabajo.gov.co.
4. El sistema verifica si el usuario existe en el directorio activo.
5. Si el usuario existe en el directorio activo, se procede con la creación normal.
6. Si el usuario no existe en el directorio activo, se rechaza la creación y se muestra un mensaje de error.

## Configuración

La funcionalidad de usuarios internos utiliza la misma configuración LDAP que se especifica en `appsettings.json`:

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

## Consideraciones Técnicas

- Los usuarios internos se marcan con la propiedad `IsInternalUser = true` en la base de datos.
- La verificación en el directorio activo se realiza mediante el método `UserExistsAsync` del servicio LDAP.
- El dominio @mintrabajo.gov.co está codificado en el sistema para identificar a los usuarios internos.
- La funcionalidad de usuarios internos está integrada con el sistema de activación de cuentas mediante tokens y códigos.
