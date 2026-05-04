using BlazorUI.Interfaces;
using Microsoft.AspNetCore.Components;
using BlazorUI.Models;
using BlazorUI.Validators;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System.IdentityModel.Tokens.Jwt;
using BlazorUI.Services;


namespace BlazorUI.Pages
{
    public partial class UserPage : ComponentBase
    {
        [Inject] private IUserServices UserServices { get; set; }
        [Inject] private CustomAuthStateProvider AuthStateProvider { get; set; }

        private List<User>? Users;
        private string selectedRoles = "AllRoles";
        private string searchUser = "";
        private List<User> usersFiltered = new List<User>();


        private bool isDoctor;
        private bool isAdmin;
        private bool isPatient;


        //When the user changes the status filter, we update the selectedStatus property and apply the filter to update
        private string SelectedRoles
        {
            get => selectedRoles;
            set
            {
                selectedRoles = value;
                ApplyFilter();
            }
        }
        //filter the appointments based on the selected status, search user and search notes
        private void ApplyFilter()
        {
            if (Users == null) return;
            usersFiltered = Users.Where(u =>
                (selectedRoles.Equals("AllRoles", StringComparison.OrdinalIgnoreCase) ||
                 u.Role.Equals(selectedRoles, StringComparison.OrdinalIgnoreCase)) &&

                (string.IsNullOrWhiteSpace(searchUser) ||
                 u.Name.IndexOf(searchUser, StringComparison.OrdinalIgnoreCase) >= 0)
            ).ToList();

        }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            isDoctor = user.IsInRole("Doctor");
            isAdmin = user.IsInRole("Admin");
            isPatient = user.IsInRole("Patient");

            var response = await UserServices.GetAllUsersAsync();
            if (response.Success)
            {
                Users = response.Data;
                ApplyFilter();
            }
            else
            {
                // Handle error (e.g., show a message to the user)
                Console.WriteLine($"Error fetching users: {response.Message}");
            }
        }
        //When the user changes the search input, we update the searchUser property and apply the filter to update the displayed
        private void OnSearchUserChanged(ChangeEventArgs e)
        {
            searchUser = e.Value?.ToString() ?? string.Empty;
            ApplyFilter();
        }
    }
}
