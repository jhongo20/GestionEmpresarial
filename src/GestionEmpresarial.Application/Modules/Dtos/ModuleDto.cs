using System;
using System.Collections.Generic;
using GestionEmpresarial.Application.Routes.Dtos;

namespace GestionEmpresarial.Application.Modules.Dtos
{
    public class ModuleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public List<RouteDto> Routes { get; set; } = new List<RouteDto>();
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
