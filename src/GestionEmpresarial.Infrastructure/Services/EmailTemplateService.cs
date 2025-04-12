using GestionEmpresarial.Application.Common.Interfaces;
using GestionEmpresarial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GestionEmpresarial.Infrastructure.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<EmailTemplateService> _logger;

        public EmailTemplateService(
            IApplicationDbContext context,
            ILogger<EmailTemplateService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<EmailTemplate>> GetAllTemplatesAsync()
        {
            return await _context.EmailTemplates
                .Where(t => !t.IsDeleted)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<EmailTemplate> GetTemplateByIdAsync(Guid id)
        {
            return await _context.EmailTemplates
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        }

        public async Task<EmailTemplate> GetTemplateByTypeAsync(string type)
        {
            return await _context.EmailTemplates
                .Where(t => t.Type == type && t.IsActive && !t.IsDeleted)
                .OrderByDescending(t => t.IsDefault)
                .FirstOrDefaultAsync();
        }

        public async Task<EmailTemplate> GetDefaultTemplateByTypeAsync(string type)
        {
            return await _context.EmailTemplates
                .FirstOrDefaultAsync(t => t.Type == type && t.IsDefault && t.IsActive && !t.IsDeleted);
        }

        public async Task<EmailTemplate> CreateTemplateAsync(EmailTemplate template)
        {
            template.Id = Guid.NewGuid();
            template.IsActive = true;
            template.IsDeleted = false;

            // Si se marca como predeterminada, desmarcar las demás del mismo tipo
            if (template.IsDefault)
            {
                await UnsetDefaultForTypeAsync(template.Type);
            }

            _context.EmailTemplates.Add(template);
            await _context.SaveChangesAsync();

            return template;
        }

        public async Task<EmailTemplate> UpdateTemplateAsync(EmailTemplate template)
        {
            var existingTemplate = await _context.EmailTemplates
                .FirstOrDefaultAsync(t => t.Id == template.Id);

            if (existingTemplate == null)
            {
                return null;
            }

            // Si se marca como predeterminada y no lo era antes, desmarcar las demás del mismo tipo
            if (template.IsDefault && !existingTemplate.IsDefault)
            {
                await UnsetDefaultForTypeAsync(template.Type);
            }

            existingTemplate.Name = template.Name;
            existingTemplate.Subject = template.Subject;
            existingTemplate.HtmlBody = template.HtmlBody;
            existingTemplate.PlainTextBody = template.PlainTextBody;
            existingTemplate.Description = template.Description;
            existingTemplate.Type = template.Type;
            existingTemplate.IsActive = template.IsActive;
            existingTemplate.IsDefault = template.IsDefault;
            existingTemplate.AvailableVariables = template.AvailableVariables;

            await _context.SaveChangesAsync();

            return existingTemplate;
        }

        public async Task<bool> DeleteTemplateAsync(Guid id)
        {
            var template = await _context.EmailTemplates
                .FirstOrDefaultAsync(t => t.Id == id);

            if (template == null)
            {
                return false;
            }

            template.IsDeleted = true;

            // Si era la plantilla predeterminada, establecer otra como predeterminada
            if (template.IsDefault)
            {
                var newDefault = await _context.EmailTemplates
                    .FirstOrDefaultAsync(t => t.Type == template.Type && t.Id != id && t.IsActive && !t.IsDeleted);

                if (newDefault != null)
                {
                    newDefault.IsDefault = true;
                }
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SetDefaultTemplateAsync(Guid id)
        {
            var template = await _context.EmailTemplates
                .FirstOrDefaultAsync(t => t.Id == id);

            if (template == null || !template.IsActive || template.IsDeleted)
            {
                return false;
            }

            // Desmarcar las demás plantillas del mismo tipo
            await UnsetDefaultForTypeAsync(template.Type);

            // Marcar esta plantilla como predeterminada
            template.IsDefault = true;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<string> ProcessTemplateAsync(string templateContent, Dictionary<string, string> variables)
        {
            if (string.IsNullOrEmpty(templateContent))
            {
                return string.Empty;
            }

            string processedContent = templateContent;

            // Reemplazar variables en el formato {{variable}}
            foreach (var variable in variables)
            {
                processedContent = Regex.Replace(
                    processedContent,
                    $@"{{\s*{variable.Key}\s*}}",
                    variable.Value ?? string.Empty,
                    RegexOptions.IgnoreCase);
            }

            return await Task.FromResult(processedContent);
        }

        private async Task UnsetDefaultForTypeAsync(string type)
        {
            var defaultTemplates = await _context.EmailTemplates
                .Where(t => t.Type == type && t.IsDefault)
                .ToListAsync();

            foreach (var template in defaultTemplates)
            {
                template.IsDefault = false;
            }

            await _context.SaveChangesAsync();
        }
    }
}
