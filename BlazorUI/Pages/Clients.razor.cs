using BlazorUI.Interfaces;
using Microsoft.AspNetCore.Components;
using BlazorUI.Models;
using BlazorUI.Validators;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System.IdentityModel.Tokens.Jwt;


namespace BlazorUI.Pages
{
    public partial class Clients : ComponentBase
    {
        [Inject] private IAppointmentService IAppointmentService { get; set; }
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; }

        private List<Appointment>? appointments;

        private bool isCreateOpen = false;
        private CreateAppointment createAppointment = new();
        private EditContext editContext;// To Manage the form state and validation
        private ValidationMessageStore messageStore;
        private CreateAppointmentValidator createAppointmentValidator = new();

        //Toast, message Error, Info, Success
        private (bool Visible, string Message, string Type) toast;

        //To bind the datetime-local input, we need a string property that we can convert to DateTime when saving
        private string dateTimeString;

        private async Task Refresh()
        {
            var result = await IAppointmentService.GetAllAppointmentAsync();
            if (result.Success)
            {
                appointments = result.Data;
                StateHasChanged();
            }
        }
        protected override void OnInitialized()
        {

            createAppointment = new CreateAppointment();
            editContext = new EditContext(createAppointment);
            messageStore = new ValidationMessageStore(editContext);

            editContext.OnFieldChanged += (sender, e) =>
            {
                messageStore.Clear(e.FieldIdentifier);
                editContext.NotifyValidationStateChanged();
            };
        }
        protected override async Task OnInitializedAsync()
        {
            var userId = (await AuthStateProvider.GetAuthenticationStateAsync()).User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var result = await IAppointmentService.GetAppointmentByUserAsync(userId);
            if (result.Success)
            {
                appointments = result.Data;
            }
        }
        private DateTime DateTimeValue
        {
            get
            {
                if (string.IsNullOrWhiteSpace(dateTimeString))
                    return DateTime.MinValue;

                if (DateTime.TryParse(dateTimeString,
                        out var dt))
                {
                    return dt;
                }

                return DateTime.MinValue;
            }
            set
            {
                dateTimeString = value.ToString("yyyy-MM-ddTHH:mm");
            }
        }
        async Task ShowToast(string message, string type = "info")
        {
            toast = (true, message, type);
            StateHasChanged(); //refresh the UI to show the toast
            await Task.Delay(2500);
            toast = (false, "", "");
            StateHasChanged();

        }
        void Create()
        {
            createAppointment = new CreateAppointment();
            isCreateOpen = true;
        }
        private async Task SaveCreate()
        {
            createAppointment.DateTime = DateTimeValue;

            //Validate the form using FluentValidation
            var resultValidator = createAppointmentValidator.Validate(createAppointment);
            if (!resultValidator.IsValid)
            {
                ShowValidationErrors(resultValidator);
                return;
            }

            //Get the current user id from the JWT token
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var currentUserId = authState.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            createAppointment.UserId = currentUserId ?? string.Empty;


            var result = await IAppointmentService.CreateAppointmentAsync(createAppointment);
            if (result.Success)
            {
                appointments?.Add(result.Data);//the new appointment is added to the list to update the UI without refreshing
                ShowToast("Cita creada exitosamente", "success");
                isCreateOpen = false;
                createAppointment = new CreateAppointment();
                editContext = new EditContext(createAppointment);
                await Refresh();
            }
            else
            {
                ShowToast("Error al crear la cita: ", "error");

            }

        }
        void Cancel()
        {
            isCreateOpen = false;
            createAppointment = new CreateAppointment();
            editContext = new EditContext(createAppointment);

        }
        //Show the validation errors in the form
        private void ShowValidationErrors(FluentValidation.Results.ValidationResult resultValidator)
        {
            messageStore.Clear();
            foreach (var error in resultValidator.Errors)
            {
                var fieldIdentifier = new FieldIdentifier(createAppointment, error.PropertyName);
                messageStore.Add(fieldIdentifier, error.ErrorMessage);
            }
            editContext.NotifyValidationStateChanged();
        }
    }
}