using GestionEmpresarial.Domain.Common;
using System;

namespace GestionEmpresarial.Domain.Entities
{
    public class EmailTemplate : AuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
        public string PlainTextBody { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsDefault { get; set; }
        
        // Variables que se pueden usar en la plantilla (formato JSON)
        public string AvailableVariables { get; set; }
    }
}
