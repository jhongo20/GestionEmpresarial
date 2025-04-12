using GestionEmpresarial.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionEmpresarial.Application.Common.Interfaces
{
    public interface IEmailTemplateService
    {
        Task<List<EmailTemplate>> GetAllTemplatesAsync();
        Task<EmailTemplate> GetTemplateByIdAsync(Guid id);
        Task<EmailTemplate> GetTemplateByTypeAsync(string type);
        Task<EmailTemplate> GetDefaultTemplateByTypeAsync(string type);
        Task<EmailTemplate> CreateTemplateAsync(EmailTemplate template);
        Task<EmailTemplate> UpdateTemplateAsync(EmailTemplate template);
        Task<bool> DeleteTemplateAsync(Guid id);
        Task<bool> SetDefaultTemplateAsync(Guid id);
        Task<string> ProcessTemplateAsync(string templateContent, Dictionary<string, string> variables);
    }
}
