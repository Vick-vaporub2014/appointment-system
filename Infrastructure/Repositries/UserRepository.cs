using Application.DTOs;
using Application.InterfacesRepo;
using Domain.Enums;
using Domain.Identity;
using Infrastructure.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private readonly ApplicationDbContext _context;
        public UserRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<UserDTO> GetByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;
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
            if (user == null) return null;
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
            // Using LINQ to join Users, UserRoles, and Roles tables to get the user details along with their roles
            var users = await (from u in _context.Users
                               join ur in _context.UserRoles on u.Id equals ur.UserId into userRoles
                               from ur in userRoles.DefaultIfEmpty()
                               join r in _context.Roles on ur.RoleId equals r.Id into roles
                               from r in roles.DefaultIfEmpty()
                               select new UserDTO
                               {
                                   UserId = u.Id,
                                   Name = u.UserName,
                                   Email = u.Email,
                                   Role = r != null ? r.Name : Roles.Patient
                               }).ToListAsync();

            return users;



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
