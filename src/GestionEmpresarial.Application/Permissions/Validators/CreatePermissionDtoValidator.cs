using FluentValidation;
using GestionEmpresarial.Application.Permissions.Dtos;

namespace GestionEmpresarial.Application.Permissions.Validators
{
    public class CreatePermissionDtoValidator : AbstractValidator<CreatePermissionDto>
    {
        public CreatePermissionDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del permiso es obligatorio.")
                .Length(3, 50).WithMessage("El nombre del permiso debe tener entre 3 y 50 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción del permiso es obligatoria.")
                .MaximumLength(200).WithMessage("La descripción del permiso no puede exceder los 200 caracteres.");
        }
    }
}
