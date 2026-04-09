
using BlazorUI.Interfaces;
using Microsoft.AspNetCore.Components;
using BlazorUI.Models;
using BlazorUI.Validators;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System.IdentityModel.Tokens.Jwt;

namespace BlazorUI.Pages
{
    public partial class AppointmentDoc: ComponentBase
    {
        [Inject] private IAppointmentService IAppointmentService { get; set; }
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        private List<Appointment>? appointments;

        private bool isCreateOpen = false;
        private CreateAppointment createAppointment = new();
        private EditContext editContext;// To Manage the form state and validation
        private ValidationMessageStore messageStore;
        private CreateAppointmentValidator createAppointmentValidator = new();

        private bool isDeleteConfirmOpen = false;
        private int appointmentIdToDelete;

        private string selectedStatus = "todos";
        private string searchUser = "";
        private string searchNotes = "";
        private List<Appointment> filteredAppointments = new();

        //Toast, message Error, Info, Success
        private (bool Visible, string Message, string Type) toast;

        //To bind the datetime-local input, we need a string property that we can convert to DateTime when saving
        private string dateTimeString;

        //Refresh
        private async Task Refresh()
        {
            var result = await IAppointmentService.GetAllAppointmentAsync();
            if (result.Success)
            {
                appointments = result.Data;
                ApplyFilter();
                StateHasChanged();
            }
        }

        //When the user changes the status filter, we update the selectedStatus property and apply the filter to update
        private string SelectedStatus
        {
            get => selectedStatus;
            set
            {
                selectedStatus = value;
                ApplyFilter();
            }
        }
        //filter the appointments based on the selected status, search user and search notes
        private void ApplyFilter()
        {
            if (appointments == null) return;
            filteredAppointments = appointments.Where(a =>
            (selectedStatus == "todos" || a.Status.Equals(selectedStatus, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(searchUser) || a.User.Name.Contains(searchUser, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(searchNotes) || a.Notes?.Contains(searchNotes, StringComparison.OrdinalIgnoreCase) == true)
            )
                .ToList();

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
            var result = await IAppointmentService.GetAllAppointmentAsync();
            if (result.Success)
            {
                appointments = result.Data;
                ApplyFilter();
            }
            else
            {
                // Manejo de error
                Console.WriteLine(result.Message);
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

        private async Task UpdateStatus(int id, string? status)
        {

            if (string.IsNullOrEmpty(status)) return;
            var appointment = appointments?.FirstOrDefault(a => a.AppointmentId == id);

            var dto = new UpdateAppointment { AppointmentId = id, Status = status, Notes = appointment?.Notes };

            var result = await IAppointmentService.UpdateAppointmentStatusAsync(dto);

            if (result.Success && result.Data != null)
            {

                appointment.Status = result.Data.Status;
                appointment.Notes = result.Data.Notes;
                appointment.DateTime = result.Data.DateTime;
                appointment.User = result.Data.User;


                await Refresh();
                ShowToast("Cita editada exitosamente", "success");

            }
            else
            {
                ShowToast("Error al editar la cita", "error");
                Console.WriteLine(result.Message);
            }


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
