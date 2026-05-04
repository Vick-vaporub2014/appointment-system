using BlazorUI.Interfaces;
using BlazorUI.Models;
using BlazorUI.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Pages
{
    public partial class AssignRolePage : ComponentBase
    {
        [Inject] private NavigationManager Navigation { get; set; }
        [Inject] private IUserServices UserService { get; set; }

        [Parameter] public string UserId { get; set; }

        private User user;
        private string selectedRole;
        private bool showWarning;
        private string message;


        protected override async Task OnInitializedAsync()
        {
            var result = await UserService.GetUserByIdAsync(UserId);
            if (result.Success)
            {
                user = result.Data;
                selectedRole = user.Role;
            }
        }

        private void UpdateRole()
        {
            showWarning = true;
        }

        private async Task ConfirmUpdate()
        {
            var result = await UserService.AssignRoleAsync(UserId, selectedRole);
            if (result.Success)
            {
                message = "Rol actualizado correctamente.";
                showWarning = false;
            }
            else
            {
                message = $"Error: {result.Message}";
            }
        }
    }
}
