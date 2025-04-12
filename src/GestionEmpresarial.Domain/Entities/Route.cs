using System;
using System.Collections.Generic;
using GestionEmpresarial.Domain.Common;

namespace GestionEmpresarial.Domain.Entities
{
    public class Route : AuditableEntity
    {
        public Route()
        {
            RoleRoutes = new List<RoleRoute>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        
        // Relación con el módulo
        public Guid ModuleId { get; set; }
        public virtual Module Module { get; set; }
        
        // Relaciones
        public virtual ICollection<RoleRoute> RoleRoutes { get; set; }
    }
}
