using System;
using System.Collections.Generic;
using GestionEmpresarial.Domain.Common;

namespace GestionEmpresarial.Domain.Entities
{
    public class Permission : AuditableEntity
    {
        public Permission()
        {
            RolePermissions = new List<RolePermission>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }

        // Relaciones
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}
