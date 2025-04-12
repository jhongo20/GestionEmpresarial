using FluentValidation;
using GestionEmpresarial.Application.Users.Dtos;
using System.Text.RegularExpressions;

namespace GestionEmpresarial.Application.Users.Validators
{
    public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordDtoValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("El ID del usuario es obligatorio.");

            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("La contraseña actual es obligatoria.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("La nueva contraseña es obligatoria.")
                .MinimumLength(8).WithMessage("La nueva contraseña debe tener al menos 8 caracteres.")
                .Must(password => Regex.IsMatch(password, @"[A-Z]")).WithMessage("La nueva contraseña debe contener al menos una letra mayúscula.")
                .Must(password => Regex.IsMatch(password, @"[a-z]")).WithMessage("La nueva contraseña debe contener al menos una letra minúscula.")
                .Must(password => Regex.IsMatch(password, @"[0-9]")).WithMessage("La nueva contraseña debe contener al menos un número.")
                .Must(password => Regex.IsMatch(password, @"[^a-zA-Z0-9]")).WithMessage("La nueva contraseña debe contener al menos un carácter especial.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("La confirmación de la contraseña es obligatoria.")
                .Equal(x => x.NewPassword).WithMessage("La confirmación de la contraseña no coincide con la nueva contraseña.");
        }
    }
}
