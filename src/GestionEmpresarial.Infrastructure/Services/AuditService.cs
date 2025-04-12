using GestionEmpresarial.Application.Common.Interfaces;
using GestionEmpresarial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionEmpresarial.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly IApplicationDbContext _context;
        private readonly IDateTime _dateTime;

        public AuditService(
            IApplicationDbContext context,
            IDateTime dateTime)
        {
            _context = context;
            _dateTime = dateTime;
        }

        public async Task<List<AuditLog>> GetAuditLogsAsync(string? userId = null, string? tableName = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 10)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(a => a.UserId == userId);

            if (!string.IsNullOrEmpty(tableName))
                query = query.Where(a => a.TableName == tableName);

            if (fromDate.HasValue)
                query = query.Where(a => a.DateTime >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a => a.DateTime <= toDate.Value);

            return await query
                .OrderByDescending(a => a.DateTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<AuditLog> GetAuditLogByIdAsync(Guid id)
        {
            return await _context.AuditLogs.FindAsync(id);
        }

        public async Task<int> GetAuditLogsCountAsync(string? userId = null, string? tableName = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(a => a.UserId == userId);

            if (!string.IsNullOrEmpty(tableName))
                query = query.Where(a => a.TableName == tableName);

            if (fromDate.HasValue)
                query = query.Where(a => a.DateTime >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a => a.DateTime <= toDate.Value);

            return await query.CountAsync();
        }

        public async Task LogActionAsync(string userId, string userName, string action, string tableName, string primaryKey, string? oldValues, string? newValues, string? affectedColumns, string? ipAddress = null, string? userAgent = null)
        {
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                UserName = userName,
                Action = action,
                TableName = tableName,
                DateTime = _dateTime.Now,
                PrimaryKey = primaryKey,
                OldValues = oldValues,
                NewValues = newValues,
                AffectedColumns = affectedColumns,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Type = action switch
                {
                    "Create" => "Create",
                    "Update" => "Update",
                    "Delete" => "Delete",
                    "Login" => "Security",
                    "Logout" => "Security",
                    "PasswordChange" => "Security",
                    _ => "Other"
                }
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }
    }
}
