using System;
using System.Collections.Generic;

namespace GestionEmpresarial.Application.Common.Models
{
    public class AuthenticationResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public bool IsLdapUser { get; set; }
    }
}
