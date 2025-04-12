using System;
using GestionEmpresarial.Domain.Common;

namespace GestionEmpresarial.Domain.Entities
{
    public class UserRole : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public bool IsDeleted { get; set; }

        // Navegación
        public virtual User User { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
    }
}
