using GrooveBoxApi.Data;
using GrooveBoxApi.DataAccess;
using GrooveBoxApi.DataBaseModel;
using GrooveBoxApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GrooveBoxApi.Controllers;
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUserData _userData;
    private readonly ILogger<UserController> _logger;

    public UserController(ApplicationDbContext context,
                          UserManager<IdentityUser> userManager,
                          RoleManager<IdentityRole> roleManager,
                          IUserData userData,
                          ILogger<UserController> logger)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _userData = userData;
        _logger = logger;
    }

    [HttpGet]
    [Route("GetMyId")]
    public ApplicationUserModel GetByObjectId()
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        UserModel userModel = _userData.GetUserById(userId);

        ApplicationUserModel user = new()
        {
            Id = userModel.Id,
            ObjectIdentifier = userModel.ObjectIdentifier,
            DisplayName = userModel.DisplayName,
        };

        var userRoles = from ur in _context.UserRoles
                        join r in _context.Roles on ur.RoleId equals r.Id
                        select new { ur.UserId, ur.RoleId, r.Name };

        user.Roles = userRoles.Where(x => x.UserId == user.Id).ToDictionary(key => key.RoleId, val => val.Name);
        
        return user;
    }

    [HttpPost]
    [Route("UpdateUser")]
    [Authorize(Roles = "Admin")]
    public async Task UpdateUser(UserModel user)
    {
        try
        {
            _userData.UpdateUser(user);
            var u = _context.Users.Where(x => x.Id == user.Id).First();

            u.Id = user.Id;
            u.ObjectIdentifier = user.ObjectIdentifier;
            u.Email = user.EmailAddress;
            u.EmailConfirmed = true;
            u.UserName = user.DisplayName;

            await _userManager.UpdateAsync(u);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    public record UserRegistrationModel(
        string ObjectId,
        string FirstName,
        string LastName,
        string DisplayName,
        string EmailAddress,
        string Password
    );

    [AllowAnonymous]
    [Route("Register")]
    [HttpPost]
    public async Task<IActionResult> Register(UserRegistrationModel user)
    {
        if (ModelState.IsValid)
        {
            var existingUser = await _userManager.FindByEmailAsync(user.EmailAddress);
            if (existingUser is null)
            {
                ApplicationUser newUser = new()
                {
                    ObjectIdentifier = user.ObjectId,
                    Email = user.EmailAddress,
                    EmailConfirmed = true,
                    UserName = user.DisplayName,
                };

                IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);

                if (result.Succeeded)
                {
                    existingUser = await _userManager.FindByEmailAsync(user.EmailAddress);

                    if (existingUser is null)
                    {
                        return BadRequest();
                    }

                    UserModel u = new()
                    {
                        Id = existingUser.Id,
                        ObjectIdentifier = user.ObjectId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        DisplayName = user.DisplayName,
                        EmailAddress = user.EmailAddress,
                    };

                    _userData.InsertUser(u);
                    return Ok();
                }
            }
        }
        return BadRequest();
    }

    [HttpPost]
    [Authorize]
    public async Task CreateRole(string roleName)
    {
        IdentityRole identityRole = new()
        {
            Name = roleName,
        };

        IdentityResult result = await _roleManager.CreateAsync(identityRole);

        if (result.Succeeded)
        {
            _logger.LogInformation("The role {RoleName} has been created.", identityRole.Name);
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [Route("Admin/GetAllRoles")]
    public Dictionary<string, string> GetAllRoles()
    {
        var roles = _context.Roles.ToDictionary(x => x.Id, x => x.Name);

        return roles;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("Admin/AddUserToRole")]
    public async Task AddUserToRole(UserRolePairModel pairing)
    {
        string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await _userManager.FindByIdAsync(pairing.UserId);

        _logger.LogInformation("Admin {Admin} added user {User} to role {Role}",
            loggedInUserId, pairing.UserId, pairing.RoleName);

        await _userManager.AddToRoleAsync(user, pairing.RoleName);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("Admin/RemoveUserFromRole")]
    public async Task RemoveUserFromRole(UserRolePairModel pairing)
    {
        string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await _userManager.FindByIdAsync(pairing.UserId);

        _logger.LogInformation("Admin {Admin} removed user {User} from role {Role}",
            loggedInUserId, pairing.UserId, pairing.RoleName);

        await _userManager.RemoveFromRoleAsync(user, pairing.RoleName);
    }
}
