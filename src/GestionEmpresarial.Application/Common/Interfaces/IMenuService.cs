using GestionEmpresarial.Application.Common.Models;
using GestionEmpresarial.Application.Menus.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionEmpresarial.Application.Common.Interfaces
{
    public interface IMenuService
    {
        Task<Result<List<MenuDto>>> GetMenuByUserIdAsync(Guid userId);
        Task<Result<List<MenuDto>>> GetMenuByRoleIdAsync(Guid roleId);
    }
}
