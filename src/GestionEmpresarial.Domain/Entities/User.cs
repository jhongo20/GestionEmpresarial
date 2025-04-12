using System;
using System.Collections.Generic;
using GestionEmpresarial.Domain.Common;
using GestionEmpresarial.Domain.Enums;

namespace GestionEmpresarial.Domain.Entities
{
    public class User : AuditableEntity
    {
        public User()
        {
            UserRoles = new List<UserRole>();
            RefreshTokens = new List<RefreshToken>();
            UserSessions = new List<UserSession>();
            ActivationTokens = new List<ActivationToken>();
        }

        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public UserStatus Status { get; set; } = UserStatus.Active;
        public UserType UserType { get; set; } = UserType.Internal;
        public string? SecurityStamp { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public string? LdapDomain { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmailConfirmed { get; set; } = false;
        public DateTime? EmailConfirmedAt { get; set; }
        public bool IsLdapUser { get; set; }

        // Relaciones
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual ICollection<UserSession> UserSessions { get; set; }
        public virtual ICollection<ActivationToken> ActivationTokens { get; set; }
    }
}
