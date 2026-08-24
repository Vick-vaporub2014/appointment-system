using Application.DTOs;
using Application.InterfacesRepo;
using Domain.Enitities;
using Domain.Enums;

using Infrastructure.DbContext;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


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

        public async Task<User?> GetByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;
            var roles = await _userManager.GetRolesAsync(user);

            return new User
            {
                Id = user.Id,
                Name = user.FullName ?? user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? Roles.Patient
            };

        }
        public async Task<User?> GetMyUserAsync(string userId)
        {

            var userWhitAppointemnts = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (userWhitAppointemnts == null) return null;

            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);

            return new User
            {
                Id = userWhitAppointemnts.Id,
                Name = userWhitAppointemnts.FullName ?? userWhitAppointemnts.UserName ?? string.Empty,
                Email = userWhitAppointemnts.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? Roles.Patient,
                Appointments = await _context.Appointments.Where(a => a.UserId == userId).ToListAsync()
            };

        }
        public async Task<List<User>> GetAllAsync()
        {
            // Using LINQ to join Users, UserRoles, and Roles tables to get the user details along with their roles
            var users = await (from u in _context.Users
                               join ur in _context.UserRoles on u.Id equals ur.UserId into userRoles
                               from ur in userRoles.DefaultIfEmpty()
                               join r in _context.Roles on ur.RoleId equals r.Id into roles
                               from r in roles.DefaultIfEmpty()
                               select new User
                               {
                                   Id = u.Id,
                                   Name = u.FullName ?? u.UserName ?? string.Empty,
                                   Email = u.Email ?? string.Empty,
                                   Role = r != null ? r.Name : Roles.Patient
                               }).AsNoTracking().ToListAsync();

            return users;



        }
        public async Task<bool> AssignRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Contains(role))
            {
                return true;
            }
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }
            await _userManager.AddToRoleAsync(user, role);
            return true;
        }
        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id);
            if (appUser == null) return false;
            return await _userManager.CheckPasswordAsync(appUser, password);

        }
        public async Task<bool> CreateUserAsync(User user, string password)
        {
            var appUser = new ApplicationUser
            {
                UserName = user.Email,
                Email = user.Email,
                FullName = user.Name
            };
            var result = await _userManager.CreateAsync(appUser, password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Failed to create user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            else if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(appUser, user.Role);
                user.Id = appUser.Id; // Set the Id of the user to the generated Id from Identity
            }
            return result.Succeeded;
        }
        public async Task<User> FindByEmailAsync(string email)
        {
            var appUser = await _userManager.FindByEmailAsync(email);
            if (appUser == null) return null;
            var roles = await _userManager.GetRolesAsync(appUser);
            return new User
            {
                Id = appUser.Id,
                Name = appUser.FullName ?? appUser.UserName ?? string.Empty,
                Email = appUser.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? Roles.Patient
            };
        }
    }
}
