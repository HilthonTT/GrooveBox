using GrooveBoxApi.Data;
using GrooveBoxApi.DataAccess;
using GrooveBoxApi.Models;
using GrooveBoxLibrary.DataAccess;
using GrooveBoxLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GrooveBoxApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ISQLUserData _SQLuserData;
    private readonly IUserData _userData;
    private readonly ILogger<UserController> _logger;

    public UserController(ApplicationDbContext context,
                          UserManager<IdentityUser> userManager,
                          RoleManager<IdentityRole> roleManager,
                          ISQLUserData SQLuserData,
                          IUserData userData,
                          ILogger<UserController> logger)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _SQLuserData = SQLuserData;
        _userData = userData;
        _logger = logger;
    }

    [HttpGet]
    [Route("GetMyId")]
    public async Task<ApplicationUserModel> GetMyId()
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        SQLUserModel userModel = _SQLuserData.GetUserById(userId);
        UserModel mongoUser = await _userData.GetUserFromAuthenticationAsync(userModel.ObjectIdentifier);

        ApplicationUserModel user = new()
        {
            Id = mongoUser.Id,
            ObjectIdentifier = mongoUser.ObjectIdentifier,
            FileName = mongoUser.FileName,
            FirstName = mongoUser.FirstName,
            LastName = mongoUser.LastName,
            DisplayName = mongoUser.DisplayName,
            EmailAddress = mongoUser.EmailAddress,
            AuthoredFiles = mongoUser.AuthoredFiles,
            VotedOnFiles = mongoUser.VotedOnFiles,
            SubscribedAuthors = mongoUser.SubscribedAuthors,
            UserSubscriptions = mongoUser.UserSubscriptions,
        };

        var userRoles = from ur in _context.UserRoles
                        join r in _context.Roles on ur.RoleId equals r.Id
                        select new { ur.UserId, ur.RoleId, r.Name };

        user.Roles = userRoles.Where(x => x.UserId == user.Id).ToDictionary(key => key.RoleId, val => val.Name);
        
        return user;
    }

    public record GetId(
        string Id
    );

    [HttpPost("GetByObjectId/{objectId}")]
    public async Task<ApplicationUserModel> GetById(string objectId)
    {
        try
        {
            SQLUserModel SQLUser = _SQLuserData.GetUserByObjectId(objectId);
            UserModel mongoUser = await _userData.GetUserFromAuthenticationAsync(objectId);

            ApplicationUserModel user = new()
            {
                Id = mongoUser.Id,
                ObjectIdentifier = mongoUser.ObjectIdentifier,
                FileName = mongoUser.FileName,
                FirstName = mongoUser.FirstName,
                LastName = mongoUser.LastName,
                DisplayName = mongoUser.DisplayName,
                EmailAddress = mongoUser.EmailAddress,
                AuthoredFiles = mongoUser.AuthoredFiles,
                VotedOnFiles = mongoUser.VotedOnFiles,
                SubscribedAuthors = mongoUser.SubscribedAuthors,
                UserSubscriptions = mongoUser.UserSubscriptions,
            };

            var userRoles = from ur in _context.UserRoles
                            join r in _context.Roles on ur.RoleId equals r.Id
                            select new { ur.UserId, ur.RoleId, r.Name };

            user.Roles = userRoles.Where(x => x.UserId == SQLUser.Id).ToDictionary(key => key.RoleId, val => val.Name);

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return null;
        }
    }

    [HttpPost("UpdateUserSubscription/{authorId}/{userId}")]
    public async Task UpdateUserSubscription(string authorId, string userId)
    {
        await _userData.UpdateSubscriptionAsync(authorId, userId);
    }

    [HttpPost]
    [Route("UpdateUser")]
    public async Task UpdateUser(UserModel user)
    {
        try
        {
            await _userData.UpdateUserAsync(user);
            var sqlUser = _SQLuserData.GetUserByObjectId(user.ObjectIdentifier);
            sqlUser.FirstName = user.FirstName;
            sqlUser.LastName = user.LastName;
            sqlUser.DisplayName = user.DisplayName;
            sqlUser.EmailAddress = user.EmailAddress;

            _SQLuserData.UpdateUser(sqlUser);
            var u = _context.Users.Where(x => x.Id == sqlUser.Id).First();

            u.Id = user.Id;
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
        string ObjectIdentifier,
        string FirstName,
        string LastName,
        string DisplayName,
        string EmailAddress,
        string Password,
        string fileName
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
                IdentityUser newUser = new()
                {
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

                    SQLUserModel u = new()
                    {
                        Id = existingUser.Id,
                        ObjectIdentifier = user.ObjectIdentifier,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        DisplayName = user.DisplayName,
                        EmailAddress = user.EmailAddress,
                    };

                    _SQLuserData.InsertUser(u);

                    var mongoUser = new GrooveBoxLibrary.Models.UserModel()
                    {
                        ObjectIdentifier = user.ObjectIdentifier,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        DisplayName = user.DisplayName,
                        EmailAddress = user.EmailAddress,
                        FileName = user.fileName,
                    };

                    await _userData.CreateUserAsync(mongoUser);

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
