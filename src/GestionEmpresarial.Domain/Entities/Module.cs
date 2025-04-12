using System;
using System.Collections.Generic;
using GestionEmpresarial.Domain.Common;

namespace GestionEmpresarial.Domain.Entities
{
    public class Module : AuditableEntity
    {
        public Module()
        {
            Routes = new List<Route>();
            RoleModules = new List<RoleModule>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }

        // Relaciones
        public virtual ICollection<Route> Routes { get; set; }
        public virtual ICollection<RoleModule> RoleModules { get; set; }
    }
}
