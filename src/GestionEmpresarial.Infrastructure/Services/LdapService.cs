using GestionEmpresarial.Application.Common.Interfaces;
using GestionEmpresarial.Application.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Threading.Tasks;

namespace GestionEmpresarial.Infrastructure.Services
{
    public class LdapService : ILdapService
    {
        private readonly LdapSettings _ldapSettings;
        private readonly ILogger<LdapService> _logger;
        private readonly IMemoryCache _cache;
        
        // Claves de caché para diferentes operaciones LDAP
        private const string CACHE_KEY_USER_EXISTS = "LDAP_USER_EXISTS_{0}";
        private const string CACHE_KEY_USER_EMAIL = "LDAP_USER_EMAIL_{0}";
        private const string CACHE_KEY_USER_DISPLAY_NAME = "LDAP_USER_DISPLAY_NAME_{0}";
        
        // Tiempos de expiración de caché para diferentes tipos de datos
        private static readonly TimeSpan USER_EXISTS_CACHE_DURATION = TimeSpan.FromMinutes(30);
        private static readonly TimeSpan USER_ATTRIBUTES_CACHE_DURATION = TimeSpan.FromHours(2);

        public LdapService(
            IOptions<LdapSettings> ldapSettings,
            ILogger<LdapService> logger,
            IMemoryCache memoryCache)
        {
            _ldapSettings = ldapSettings.Value;
            _logger = logger;
            _cache = memoryCache;
        }

        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            if (!_ldapSettings.Enabled)
            {
                return false;
            }

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            try
            {
                // Para la autenticación no usamos caché por seguridad
                // Cada intento de autenticación debe verificarse contra el directorio LDAP

                var ldapIdentifier = new LdapDirectoryIdentifier(_ldapSettings.Server, _ldapSettings.Port);
                using var connection = new LdapConnection(ldapIdentifier)
                {
                    AuthType = AuthType.Basic
                };

                if (_ldapSettings.UseSSL)
                {
                    connection.SessionOptions.SecureSocketLayer = true;
                    connection.SessionOptions.VerifyServerCertificate = (conn, cert) => true;
                }

                // Primero, buscar al usuario para obtener su DN completo
                using var serviceConnection = new LdapConnection(ldapIdentifier)
                {
                    AuthType = AuthType.Basic,
                    Credential = new NetworkCredential(_ldapSettings.BindDN, _ldapSettings.BindPassword)
                };

                if (_ldapSettings.UseSSL)
                {
                    serviceConnection.SessionOptions.SecureSocketLayer = true;
                    serviceConnection.SessionOptions.VerifyServerCertificate = (conn, cert) => true;
                }

                await Task.Run(() => serviceConnection.Bind());

                var searchRequest = new SearchRequest(
                    _ldapSettings.SearchBase,
                    string.Format(_ldapSettings.SearchFilter, username),
                    SearchScope.Subtree,
                    null
                );

                var searchResponse = (SearchResponse)await Task.Run(() => serviceConnection.SendRequest(searchRequest));

                if (searchResponse.Entries.Count == 0)
                {
                    _logger.LogWarning("User {Username} not found in LDAP directory", username);
                    return false;
                }

                var userDn = searchResponse.Entries[0].DistinguishedName;

                // Ahora intentar autenticar con las credenciales del usuario
                try
                {
                    connection.Credential = new NetworkCredential(userDn, password);
                    await Task.Run(() => connection.Bind());
                    
                    _logger.LogInformation("User {Username} authenticated successfully via LDAP", username);
                    
                    // Actualizar la caché de existencia de usuario después de una autenticación exitosa
                    var cacheKey = string.Format(CACHE_KEY_USER_EXISTS, username);
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(USER_EXISTS_CACHE_DURATION);
                    _cache.Set(cacheKey, true, cacheEntryOptions);
                    
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to authenticate user {Username} via LDAP", username);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LDAP authentication error for user {Username}", username);
                return false;
            }
        }

        public async Task<string> GetUserEmailAsync(string username)
        {
            if (!_ldapSettings.Enabled)
            {
                return string.Empty;
            }

            // Verificar si el correo electrónico del usuario ya está en caché
            var cacheKey = string.Format(CACHE_KEY_USER_EMAIL, username);
            if (_cache.TryGetValue(cacheKey, out string userEmail))
            {
                return userEmail;
            }

            try
            {
                var ldapIdentifier = new LdapDirectoryIdentifier(_ldapSettings.Server, _ldapSettings.Port);
                using var connection = new LdapConnection(ldapIdentifier)
                {
                    AuthType = AuthType.Basic,
                    Credential = new NetworkCredential(_ldapSettings.BindDN, _ldapSettings.BindPassword)
                };

                if (_ldapSettings.UseSSL)
                {
                    connection.SessionOptions.SecureSocketLayer = true;
                    connection.SessionOptions.VerifyServerCertificate = (conn, cert) => true;
                }

                await Task.Run(() => connection.Bind());

                var searchRequest = new SearchRequest(
                    _ldapSettings.SearchBase,
                    string.Format(_ldapSettings.SearchFilter, username),
                    SearchScope.Subtree,
                    new string[] { _ldapSettings.EmailAttribute }
                );

                var searchResponse = (SearchResponse)await Task.Run(() => connection.SendRequest(searchRequest));

                if (searchResponse.Entries.Count == 0)
                {
                    return string.Empty;
                }

                var entry = searchResponse.Entries[0];
                if (entry.Attributes.Contains(_ldapSettings.EmailAttribute))
                {
                    userEmail = entry.Attributes[_ldapSettings.EmailAttribute][0] as string ?? string.Empty;

                    // Almacenar el correo electrónico del usuario en caché
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(USER_ATTRIBUTES_CACHE_DURATION);
                    _cache.Set(cacheKey, userEmail, cacheEntryOptions);

                    return userEmail;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving email for user {Username} from LDAP", username);
                return string.Empty;
            }
        }

        public async Task<string> GetUserDisplayNameAsync(string username)
        {
            if (!_ldapSettings.Enabled)
            {
                return string.Empty;
            }

            // Verificar si el nombre de visualización del usuario ya está en caché
            var cacheKey = string.Format(CACHE_KEY_USER_DISPLAY_NAME, username);
            if (_cache.TryGetValue(cacheKey, out string userDisplayName))
            {
                return userDisplayName;
            }

            try
            {
                var ldapIdentifier = new LdapDirectoryIdentifier(_ldapSettings.Server, _ldapSettings.Port);
                using var connection = new LdapConnection(ldapIdentifier)
                {
                    AuthType = AuthType.Basic,
                    Credential = new NetworkCredential(_ldapSettings.BindDN, _ldapSettings.BindPassword)
                };

                if (_ldapSettings.UseSSL)
                {
                    connection.SessionOptions.SecureSocketLayer = true;
                    connection.SessionOptions.VerifyServerCertificate = (conn, cert) => true;
                }

                await Task.Run(() => connection.Bind());

                var searchRequest = new SearchRequest(
                    _ldapSettings.SearchBase,
                    string.Format(_ldapSettings.SearchFilter, username),
                    SearchScope.Subtree,
                    new string[] { _ldapSettings.DisplayNameAttribute }
                );

                var searchResponse = (SearchResponse)await Task.Run(() => connection.SendRequest(searchRequest));

                if (searchResponse.Entries.Count == 0)
                {
                    return string.Empty;
                }

                var entry = searchResponse.Entries[0];
                if (entry.Attributes.Contains(_ldapSettings.DisplayNameAttribute))
                {
                    userDisplayName = entry.Attributes[_ldapSettings.DisplayNameAttribute][0] as string ?? string.Empty;

                    // Almacenar el nombre de visualización del usuario en caché
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(USER_ATTRIBUTES_CACHE_DURATION);
                    _cache.Set(cacheKey, userDisplayName, cacheEntryOptions);

                    return userDisplayName;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving display name for user {Username} from LDAP", username);
                return string.Empty;
            }
        }

        public bool IsEnabled()
        {
            return _ldapSettings.Enabled;
        }

        public string GetDefaultRoleName()
        {
            return _ldapSettings.DefaultRole;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            if (!_ldapSettings.Enabled)
            {
                _logger.LogWarning("LDAP is disabled, cannot check if user exists");
                return false;
            }

            // Verificar si la existencia del usuario ya está en caché
            var cacheKey = string.Format(CACHE_KEY_USER_EXISTS, username);
            if (_cache.TryGetValue(cacheKey, out bool userExists))
            {
                return userExists;
            }

            try
            {
                // Crear la conexión LDAP
                var ldapIdentifier = new LdapDirectoryIdentifier(_ldapSettings.Server, _ldapSettings.Port);
                
                // Autenticamos con la cuenta de servicio para buscar al usuario
                using var serviceConnection = new LdapConnection(ldapIdentifier)
                {
                    AuthType = AuthType.Basic,
                    Credential = new NetworkCredential(_ldapSettings.BindDN, _ldapSettings.BindPassword)
                };

                if (_ldapSettings.UseSSL)
                {
                    serviceConnection.SessionOptions.SecureSocketLayer = true;
                    serviceConnection.SessionOptions.VerifyServerCertificate = (conn, cert) => true; // En producción, esto debería validar correctamente
                }

                // Conectar con la cuenta de servicio
                await Task.Run(() => serviceConnection.Bind());

                // Buscar al usuario
                var searchRequest = new SearchRequest(
                    _ldapSettings.SearchBase,
                    string.Format(_ldapSettings.SearchFilter, username),
                    SearchScope.Subtree,
                    null
                );

                var searchResponse = (SearchResponse)await Task.Run(() => serviceConnection.SendRequest(searchRequest));

                userExists = searchResponse.Entries.Count > 0;
                
                if (userExists)
                {
                    _logger.LogInformation("User {Username} found in LDAP directory", username);
                }
                else
                {
                    _logger.LogWarning("User {Username} not found in LDAP directory", username);
                }

                // Almacenar la existencia del usuario en caché
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(USER_EXISTS_CACHE_DURATION);
                _cache.Set(cacheKey, userExists, cacheEntryOptions);

                return userExists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user {Username} exists in LDAP", username);
                return false;
            }
        }
    }
}
