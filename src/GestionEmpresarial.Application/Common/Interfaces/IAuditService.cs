using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GestionEmpresarial.Domain.Entities;

namespace GestionEmpresarial.Application.Common.Interfaces
{
    public interface IAuditService
    {
        Task<List<AuditLog>> GetAuditLogsAsync(string? userId = null, string? tableName = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 10);
        Task<AuditLog> GetAuditLogByIdAsync(Guid id);
        Task<int> GetAuditLogsCountAsync(string? userId = null, string? tableName = null, DateTime? fromDate = null, DateTime? toDate = null);
        Task LogActionAsync(string userId, string userName, string action, string tableName, string primaryKey, string? oldValues, string? newValues, string? affectedColumns, string? ipAddress = null, string? userAgent = null);
    }
}
