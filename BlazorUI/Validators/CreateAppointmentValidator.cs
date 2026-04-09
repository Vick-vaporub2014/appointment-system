using FluentValidation;
using BlazorUI.Models;
namespace BlazorUI.Validators
{
    public class CreateAppointmentValidator : AbstractValidator<CreateAppointment>
    {
        //This validator is used to validate the CreateAppointment model. It checks that the DateTime is in the future, that the Notes are not empty and do not exceed 200 characters, and that the UserId is not empty
        public CreateAppointmentValidator()
        {

            RuleFor(x => x.DateTime)
            .NotEmpty().WithMessage("La fecha y hora son obligatorias.");

            RuleFor(x => x.DateTime)
            .Must(BeInFuture).WithMessage("La cita debe ser en el futuro.")
                .Unless(x => x.DateTime == DateTime.MinValue);

            RuleFor(x => x.DateTime)
            .Must(HaveWholeHour).WithMessage("La cita debe comenzar en una hora exacta (ej. 13:00, 14:00).")
            .Unless(x => x.DateTime == DateTime.MinValue);


            RuleFor(x => x.Notes)
                .MaximumLength(200).WithMessage("Las notas no deben de superar los 200 caracteres")
                .Must(NotWhiteSpace).WithMessage("No puede esta vacio las notas");

        }
        private bool NotWhiteSpace(string notes) => !string.IsNullOrWhiteSpace(notes);
        private bool BeInFuture(DateTime dateTime) => dateTime > DateTime.UtcNow;
        private bool HaveWholeHour(DateTime dateTime) => dateTime.Minute == 0 && dateTime.Second == 0;

    }
}
