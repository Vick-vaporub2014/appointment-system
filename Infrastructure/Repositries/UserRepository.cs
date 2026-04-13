using Application.DTOs;
using Application.InterfacesRepo;
using Domain.Enums;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositries
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserDTO> GetByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null) return null;
            var roles = await _userManager.GetRolesAsync(user);

            return new UserDTO
            {
                UserId = user.Id,
                Name = user.UserName,
                Email = user.Email,
                Role = roles.FirstOrDefault() ?? Roles.Patient
            };

        }
        public async Task<UserDTO> GetMyUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null) return null;
            var roles = await _userManager.GetRolesAsync(user);

            return new UserDTO
            {
                UserId = user.Id,
                Name = user.UserName,
                Email = user.Email,
                Role = roles.FirstOrDefault() ?? Roles.Patient
            };
        }
        public async Task<List<UserDTO>> GetAllAsync()
        {
            var users = _userManager.Users.ToList();
            var userDTOs = new List<UserDTO>();
            foreach (var user in users) {
                var roles = await _userManager.GetRolesAsync(user);
                userDTOs.Add( new UserDTO
                {
                    UserId = user.Id,
                    Name = user.UserName,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? Roles.Patient
                });
            }
            return userDTOs;
        }
        public async Task AssignRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
