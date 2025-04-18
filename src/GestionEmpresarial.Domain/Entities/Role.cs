using System;
using System.Collections.Generic;
using GestionEmpresarial.Domain.Common;

namespace GestionEmpresarial.Domain.Entities
{
    public class Role : AuditableEntity
    {
        public Role()
        {
            UserRoles = new List<UserRole>();
            RolePermissions = new List<RolePermission>();
            RoleModules = new List<RoleModule>();
            RoleRoutes = new List<RoleRoute>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }

        // Relaciones
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
        public virtual ICollection<RoleModule> RoleModules { get; set; }
        public virtual ICollection<RoleRoute> RoleRoutes { get; set; }
    }
}
