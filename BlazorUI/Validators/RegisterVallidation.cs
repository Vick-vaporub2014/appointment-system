using BlazorUI.Models;
using FluentValidation;
using static BlazorUI.Models.Auth;

namespace BlazorUI.Validators
{
    public class RegisterVallidation : AbstractValidator<RegisterDTO>
    {
        public RegisterVallidation()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre de usuario no puede exceder los 100 caracteres.")
                .MinimumLength(2).WithMessage("El nombre de usuario debe tener al menos 2 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                .EmailAddress().WithMessage("El correo electrónico no es válido.")
                .Must(email => IsValidateEmail(email))
                    .WithMessage("El correo electrónico contiene caracteres inválidos.(/ , ; ...)");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(12).WithMessage("La contraseña debe tener al menos 12 caracteres.")
                .Matches("[A-Z]").WithMessage("Debe contener al menos una mayúscula")
                .Matches("[0-9]").WithMessage("Debe contener al menos un número")
                .Matches("[^a-zA-Z0-9]").WithMessage("Debe contener al menos un carácter especial")
                .Must(password => !HasToManyRepeatedCharacters(password))
                    .WithMessage("La contraseña no puede contener más de tres caracteres repetidos.");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("El nombre completo es obligatorio.")
                .MaximumLength(200).WithMessage("El nombre completo no puede exceder los 200 caracteres.")
                .MinimumLength(2).WithMessage("El nombre completo debe tener al menos 2 caracteres.");

        }
        private bool HasToManyRepeatedCharacters(string password)
        {
            //Very simple check for more than 3 repeated characters in a row
            for (int i = 0; i < password.Length - 3; i++)
            {
                if (password[i] == password[i + 1] &&
                    password[i] == password[i + 2] &&
                    password[i] == password[i + 3])
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            var invalidChars = new[] { "\"", " ", "'", ",", ";", ":", "<", ">", "(", ")" };

            return !invalidChars.Any(c => email.Contains(c));
        }
    }
}
