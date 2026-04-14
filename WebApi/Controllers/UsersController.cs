using Application.DTOs;
using Application.InterfacesServices;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] //the route is set to "api/users" based on the controller name
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService) {
            _userService = userService;
        }
        [Authorize(Roles = Roles.Admin + "," + Roles.Doctor )]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var result = await _userService.GetUserByIdAsync(userId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
            
        }
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyUser()
        {
            var userId = User.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var result = await _userService.GetMyUserAsync(userId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [Authorize(Roles = Roles.Admin +"," + Roles.Doctor)]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var result = await _userService.GetAllUsersAsync();
                return Ok(result);
            } 
            catch(InvalidOperationException ex)
            {
                return NotFound( ex.Message );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }
        [Authorize(Roles = Roles.Admin)]
         [HttpPost("assign-role")]
         public async Task<IActionResult> AssignRole([FromBody] AssignRoleDTO dto)
         {
            try
            {
                await _userService.AssignRoleAsync(dto.UserId, dto.Role);
                return Ok(new { Success = true, Message = "Role assigned successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

    }
}
