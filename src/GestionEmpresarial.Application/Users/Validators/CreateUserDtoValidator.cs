using FluentValidation;
using GestionEmpresarial.Application.Users.Dtos;
using System.Text.RegularExpressions;

namespace GestionEmpresarial.Application.Users.Validators
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
                .Length(3, 50).WithMessage("El nombre de usuario debe tener entre 3 y 50 caracteres.")
                .Matches(@"^[a-zA-Z0-9._-]+$").WithMessage("El nombre de usuario solo puede contener letras, números, puntos, guiones y guiones bajos.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
                .Must(password => Regex.IsMatch(password, @"[A-Z]")).WithMessage("La contraseña debe contener al menos una letra mayúscula.")
                .Must(password => Regex.IsMatch(password, @"[a-z]")).WithMessage("La contraseña debe contener al menos una letra minúscula.")
                .Must(password => Regex.IsMatch(password, @"[0-9]")).WithMessage("La contraseña debe contener al menos un número.")
                .Must(password => Regex.IsMatch(password, @"[^a-zA-Z0-9]")).WithMessage("La contraseña debe contener al menos un carácter especial.");

            RuleFor(x => x.FirstName)
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.LastName)
                .MaximumLength(100).WithMessage("El apellido no puede exceder los 100 caracteres.");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("El número de teléfono no puede exceder los 20 caracteres.");
        }
    }
}
