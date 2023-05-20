using GrooveBoxApi.Data;
using GrooveBoxApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GrooveBoxApi.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class RoleController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<RoleController> _logger;

    public RoleController(ApplicationDbContext context,
                          UserManager<IdentityUser> userManager,
                          RoleManager<IdentityRole> roleManager,
                          ILogger<RoleController> logger)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
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
