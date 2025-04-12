using GestionEmpresarial.Application.Common.Interfaces;
using System;

namespace GestionEmpresarial.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.UtcNow;
    }
}
