using System;
using GestionEmpresarial.Domain.Common;

namespace GestionEmpresarial.Domain.Entities
{
    public class RoleModule : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public Guid ModuleId { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }

        // Relaciones
        public virtual Role Role { get; set; }
        public virtual Module Module { get; set; }
    }
}
