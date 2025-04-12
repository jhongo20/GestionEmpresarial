using GestionEmpresarial.Application.Common.Models;
using GestionEmpresarial.Application.Users.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionEmpresarial.Application.Common.Interfaces
{
    public interface IUserService
    {
        Task<Result<List<UserDto>>> GetAllUsersAsync();
        Task<Result<PaginatedList<UserDto>>> GetUsersPagedAsync(PaginationParams paginationParams);
        Task<Result<UserDto>> GetUserByIdAsync(Guid id);
        Task<Result<UserDto>> CreateUserAsync(CreateUserDto createUserDto);
        Task<Result<UserDto>> CreateLdapUserAsync(CreateLdapUserDto createLdapUserDto);
        Task<Result<UserDto>> UpdateUserAsync(UpdateUserDto updateUserDto);
        Task<Result<bool>> DeleteUserAsync(Guid id);
        Task<Result<bool>> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
        Task<Result<List<UserDto>>> GetUsersByRoleAsync(Guid roleId);
        Task<Result<PaginatedList<UserDto>>> GetUsersByRolePagedAsync(Guid roleId, PaginationParams paginationParams);
        Task<Result<string>> GenerateActivationTokenAsync(Guid userId);
        Task<Result<bool>> ActivateAccountAsync(ActivateAccountDto activateAccountDto);
        Task<Result<bool>> ResendActivationEmailAsync(string email);
        Task<Result<bool>> ActivateAccountWithTokenAsync(string token);
        Task<Result<bool>> ActivateAccountWithCodeAsync(string email, string code);
    }
}
