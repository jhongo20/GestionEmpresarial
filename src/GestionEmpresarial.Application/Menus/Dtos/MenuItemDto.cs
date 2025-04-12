using System;

namespace GestionEmpresarial.Application.Menus.Dtos
{
    public class MenuItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Path { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}
