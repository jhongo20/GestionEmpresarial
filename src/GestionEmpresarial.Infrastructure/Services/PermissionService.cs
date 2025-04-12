using GestionEmpresarial.Application.Common.Interfaces;
using GestionEmpresarial.Application.Common.Models;
using GestionEmpresarial.Application.Permissions.Dtos;
using GestionEmpresarial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace GestionEmpresarial.Infrastructure.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;

        public PermissionService(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IDateTime dateTime)
        {
            _context = context;
            _currentUserService = currentUserService;
            _dateTime = dateTime;
        }

        public async Task<Result<List<PermissionDto>>> GetAllPermissionsAsync()
        {
            var permissions = await _context.Permissions
                .Where(p => !p.IsDeleted)
                .Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                    CreatedBy = p.CreatedBy,
                    UpdatedAt = p.UpdatedAt,
                    UpdatedBy = p.UpdatedBy
                })
                .ToListAsync();

            return Result<List<PermissionDto>>.Success(permissions);
        }

        public async Task<Result<PermissionDto>> GetPermissionByIdAsync(Guid id)
        {
            var permission = await _context.Permissions
                .Where(p => p.Id == id && !p.IsDeleted)
                .Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                    CreatedBy = p.CreatedBy,
                    UpdatedAt = p.UpdatedAt,
                    UpdatedBy = p.UpdatedBy
                })
                .FirstOrDefaultAsync();

            if (permission == null)
            {
                return Result<PermissionDto>.Failure("El permiso especificado no existe.");
            }

            return Result<PermissionDto>.Success(permission);
        }

        public async Task<Result<PermissionDto>> CreatePermissionAsync(CreatePermissionDto createPermissionDto)
        {
            // Verificar si ya existe un permiso con el mismo nombre
            var existingPermission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name.ToLower() == createPermissionDto.Name.ToLower() && !p.IsDeleted);

            if (existingPermission != null)
            {
                return Result<PermissionDto>.Failure($"Ya existe un permiso con el nombre '{createPermissionDto.Name}'.");
            }

            var permission = new Permission
            {
                Id = Guid.NewGuid(),
                Name = createPermissionDto.Name,
                Description = createPermissionDto.Description,
                IsDeleted = false,
                CreatedBy = _currentUserService.UserId ?? "System",
                CreatedAt = _dateTime.Now,
                UpdatedBy = _currentUserService.UserId ?? "System",
                UpdatedAt = _dateTime.Now
            };

            await _context.Permissions.AddAsync(permission);
            await _context.SaveChangesAsync(default);

            var permissionDto = new PermissionDto
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description,
                CreatedAt = permission.CreatedAt,
                CreatedBy = permission.CreatedBy,
                UpdatedAt = permission.UpdatedAt,
                UpdatedBy = permission.UpdatedBy
            };

            return Result<PermissionDto>.Success(permissionDto);
        }

        public async Task<Result<PermissionDto>> UpdatePermissionAsync(UpdatePermissionDto updatePermissionDto)
        {
            var permission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Id == updatePermissionDto.Id && !p.IsDeleted);

            if (permission == null)
            {
                return Result<PermissionDto>.Failure("El permiso especificado no existe.");
            }

            // Verificar si ya existe otro permiso con el mismo nombre
            var existingPermission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name.ToLower() == updatePermissionDto.Name.ToLower() 
                                      && p.Id != updatePermissionDto.Id 
                                      && !p.IsDeleted);

            if (existingPermission != null)
            {
                return Result<PermissionDto>.Failure($"Ya existe otro permiso con el nombre '{updatePermissionDto.Name}'.");
            }

            permission.Name = updatePermissionDto.Name;
            permission.Description = updatePermissionDto.Description;
            permission.UpdatedBy = _currentUserService.UserId ?? "System";
            permission.UpdatedAt = _dateTime.Now;

            _context.Permissions.Update(permission);
            await _context.SaveChangesAsync(default);

            var permissionDto = new PermissionDto
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description,
                CreatedAt = permission.CreatedAt,
                CreatedBy = permission.CreatedBy,
                UpdatedAt = permission.UpdatedAt,
                UpdatedBy = permission.UpdatedBy
            };

            return Result<PermissionDto>.Success(permissionDto);
        }

        public async Task<Result> DeletePermissionAsync(Guid id)
        {
            var permission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (permission == null)
            {
                return Result.Failure("El permiso especificado no existe.");
            }

            // Verificar si hay roles que usan este permiso
            var hasRoles = await _context.RolePermissions
                .AnyAsync(rp => rp.PermissionId == id && !rp.IsDeleted);

            if (hasRoles)
            {
                return Result.Failure("No se puede eliminar el permiso porque está asignado a uno o más roles.");
            }

            // Eliminar lógicamente el permiso
            permission.IsDeleted = true;
            permission.UpdatedBy = _currentUserService.UserId ?? "System";
            permission.UpdatedAt = _dateTime.Now;

            _context.Permissions.Update(permission);
            await _context.SaveChangesAsync(default);

            return Result.Success();
        }

        public async Task<Result<PaginatedList<PermissionDto>>> GetPermissionsPagedAsync(PaginationParams paginationParams)
        {
            var query = _context.Permissions
                .Where(p => !p.IsDeleted)
                .AsQueryable();

            // Aplicar filtrado y ordenación
            if (!string.IsNullOrEmpty(paginationParams.SortBy))
            {
                var sortDirection = paginationParams.SortDirection?.ToLower() == "desc" ? "descending" : "ascending";
                var sortProperty = paginationParams.SortBy;
                
                // Asegurarse de que la propiedad existe
                if (typeof(Permission).GetProperty(sortProperty) != null)
                {
                    query = query.OrderBy($"{sortProperty} {sortDirection}");
                }
                else
                {
                    query = query.OrderBy(p => p.Id);
                }
            }
            else
            {
                query = query.OrderBy(p => p.Id);
            }

            var paginatedPermissions = await PaginatedList<PermissionDto>.CreateAsync(
                query.Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                    CreatedBy = p.CreatedBy,
                    UpdatedAt = p.UpdatedAt,
                    UpdatedBy = p.UpdatedBy
                }),
                paginationParams.PageNumber,
                paginationParams.PageSize);

            return Result<PaginatedList<PermissionDto>>.Success(paginatedPermissions);
        }
    }
}
