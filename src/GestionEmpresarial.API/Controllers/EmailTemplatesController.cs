using GestionEmpresarial.Application.Common.Interfaces;
using GestionEmpresarial.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionEmpresarial.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class EmailTemplatesController : ControllerBase
    {
        private readonly IEmailTemplateService _emailTemplateService;

        public EmailTemplatesController(IEmailTemplateService emailTemplateService)
        {
            _emailTemplateService = emailTemplateService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTemplates()
        {
            var templates = await _emailTemplateService.GetAllTemplatesAsync();
            return Ok(templates);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTemplateById(Guid id)
        {
            var template = await _emailTemplateService.GetTemplateByIdAsync(id);
            if (template == null)
                return NotFound();

            return Ok(template);
        }

        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetTemplateByType(string type)
        {
            var template = await _emailTemplateService.GetTemplateByTypeAsync(type);
            if (template == null)
                return NotFound();

            return Ok(template);
        }

        [HttpGet("default/type/{type}")]
        public async Task<IActionResult> GetDefaultTemplateByType(string type)
        {
            var template = await _emailTemplateService.GetDefaultTemplateByTypeAsync(type);
            if (template == null)
                return NotFound();

            return Ok(template);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTemplate([FromBody] EmailTemplate template)
        {
            if (template == null)
                return BadRequest();

            var createdTemplate = await _emailTemplateService.CreateTemplateAsync(template);
            return CreatedAtAction(nameof(GetTemplateById), new { id = createdTemplate.Id }, createdTemplate);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTemplate(Guid id, [FromBody] EmailTemplate template)
        {
            if (template == null || id != template.Id)
                return BadRequest();

            var updatedTemplate = await _emailTemplateService.UpdateTemplateAsync(template);
            if (updatedTemplate == null)
                return NotFound();

            return Ok(updatedTemplate);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplate(Guid id)
        {
            var result = await _emailTemplateService.DeleteTemplateAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPut("{id}/set-default")]
        public async Task<IActionResult> SetDefaultTemplate(Guid id)
        {
            var result = await _emailTemplateService.SetDefaultTemplateAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("types")]
        public IActionResult GetTemplateTypes()
        {
            var types = new List<string>
            {
                "AccountActivation",
                "RegistrationConfirmation",
                "PasswordReset",
                "AccountUpdated",
                "WelcomeEmail",
                "NewsletterSubscription",
                "InvoiceNotification",
                "PaymentConfirmation",
                "OrderConfirmation",
                "ShippingNotification",
                "AppointmentReminder",
                "AccountDeactivation",
                "SecurityAlert"
            };

            return Ok(types);
        }

        [HttpPost("test")]
        public async Task<IActionResult> TestTemplate([FromBody] TestTemplateRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.TemplateContent))
                return BadRequest("El contenido de la plantilla es requerido");

            var processedContent = await _emailTemplateService.ProcessTemplateAsync(
                request.TemplateContent,
                request.Variables ?? new Dictionary<string, string>());

            return Ok(new { ProcessedContent = processedContent });
        }
    }

    public class TestTemplateRequest
    {
        public string TemplateContent { get; set; }
        public Dictionary<string, string> Variables { get; set; }
    }
}
