using GestionEmpresarial.Application.Common.Interfaces;
using GestionEmpresarial.Application.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

        public LdapService(IOptions<LdapSettings> ldapSettings, ILogger<LdapService> logger)
        {
            _ldapSettings = ldapSettings.Value;
            _logger = logger;
        }

        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            if (!_ldapSettings.Enabled)
            {
                _logger.LogWarning("LDAP authentication is disabled");
                return false;
            }

            try
            {
                // Crear la conexión LDAP
                var ldapIdentifier = new LdapDirectoryIdentifier(_ldapSettings.Server, _ldapSettings.Port);
                
                // Primero autenticamos con la cuenta de servicio para buscar al usuario
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

                if (searchResponse.Entries.Count == 0)
                {
                    _logger.LogWarning("User {Username} not found in LDAP", username);
                    return false;
                }

                // Obtener el DN del usuario
                var userDn = searchResponse.Entries[0].DistinguishedName;

                // Intentar autenticar con las credenciales del usuario
                try
                {
                    using var userConnection = new LdapConnection(ldapIdentifier)
                    {
                        AuthType = AuthType.Basic,
                        Credential = new NetworkCredential(userDn, password)
                    };

                    if (_ldapSettings.UseSSL)
                    {
                        userConnection.SessionOptions.SecureSocketLayer = true;
                        userConnection.SessionOptions.VerifyServerCertificate = (conn, cert) => true;
                    }

                    await Task.Run(() => userConnection.Bind());
                    _logger.LogInformation("User {Username} authenticated successfully via LDAP", username);
                    return true;
                }
                catch (LdapException ex)
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
                    return entry.Attributes[_ldapSettings.EmailAttribute][0] as string ?? string.Empty;
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
                    return entry.Attributes[_ldapSettings.DisplayNameAttribute][0] as string ?? string.Empty;
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

                bool userExists = searchResponse.Entries.Count > 0;
                
                if (userExists)
                {
                    _logger.LogInformation("User {Username} found in LDAP directory", username);
                }
                else
                {
                    _logger.LogWarning("User {Username} not found in LDAP directory", username);
                }
                
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
