using GestionEmpresarial.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GestionEmpresarial.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class AuditController : ControllerBase
    {
        private readonly IAuditService _auditService;

        public AuditController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuditLogs(
            [FromQuery] string? userId = null,
            [FromQuery] string? tableName = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var logs = await _auditService.GetAuditLogsAsync(userId, tableName, fromDate, toDate, page, pageSize);
            var count = await _auditService.GetAuditLogsCountAsync(userId, tableName, fromDate, toDate);

            return Ok(new
            {
                Data = logs,
                TotalCount = count,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize)
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuditLogById(Guid id)
        {
            var log = await _auditService.GetAuditLogByIdAsync(id);
            if (log == null)
                return NotFound();

            return Ok(log);
        }

        [HttpGet("tables")]
        public IActionResult GetAuditTables()
        {
            // Lista de tablas principales del sistema para filtrar auditorías
            var tables = new[]
            {
                "Users",
                "Roles",
                "Permissions",
                "Modules",
                "Routes",
                "RoleModules",
                "RoleRoutes",
                "auth"
            };

            return Ok(tables);
        }

        [HttpGet("actions")]
        public IActionResult GetAuditActions()
        {
            // Lista de acciones para filtrar auditorías
            var actions = new[]
            {
                "Create",
                "Update",
                "Delete",
                "Login",
                "Logout",
                "PasswordChange"
            };

            return Ok(actions);
        }
    }
}
