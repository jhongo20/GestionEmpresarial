using System.Threading.Tasks;

namespace GestionEmpresarial.Application.Common.Interfaces
{
    public interface ILdapService
    {
        /// <summary>
        /// Autentica a un usuario contra el directorio LDAP
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <param name="password">Contraseña</param>
        /// <returns>True si la autenticación es exitosa, False en caso contrario</returns>
        Task<bool> AuthenticateAsync(string username, string password);

        /// <summary>
        /// Obtiene el correo electrónico de un usuario del directorio LDAP
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <returns>Correo electrónico del usuario</returns>
        Task<string> GetUserEmailAsync(string username);

        /// <summary>
        /// Obtiene el nombre completo de un usuario del directorio LDAP
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <returns>Nombre completo del usuario</returns>
        Task<string> GetUserDisplayNameAsync(string username);

        /// <summary>
        /// Verifica si el servicio LDAP está habilitado
        /// </summary>
        /// <returns>True si el servicio LDAP está habilitado, False en caso contrario</returns>
        bool IsEnabled();

        /// <summary>
        /// Obtiene el nombre del rol por defecto para usuarios LDAP
        /// </summary>
        /// <returns>Nombre del rol por defecto</returns>
        string GetDefaultRoleName();
    }
}
