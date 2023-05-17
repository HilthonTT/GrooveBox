using GrooveBoxApi.Data;
using GrooveBoxApi.DataAccess;
using GrooveBoxApiLibrary.Models;
using GrooveBoxApiLibrary.MongoDataAccess;
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
    private readonly IUserData _userData;
    private readonly ILogger<UserController> _logger;

    public UserController(ApplicationDbContext context,
                          UserManager<IdentityUser> userManager,
                          IUserData userData,
                          ILogger<UserController> logger)
    {
        _context = context;
        _userManager = userManager;
        _userData = userData;
        _logger = logger;
    }

    [HttpGet]
    [Route("GetMyId")]
    public async Task<ApplicationUserModel> GetMyId()
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var mongoUser = await _userData.GetUserFromAuthenticationAsync(userId);

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

    [HttpPost("GetByObjectId/{objectId}")]
    public async Task<ApplicationUserModel> GetByObjectId(string objectId)
    {
        try
        {
            var user = await _userData.GetUserFromAuthenticationAsync(objectId);

            ApplicationUserModel u = new()
            {
                Id = user.Id,
                ObjectIdentifier = user.ObjectIdentifier,
                FileName = user.FileName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DisplayName = user.DisplayName,
                EmailAddress = user.EmailAddress,
                AuthoredFiles = user.AuthoredFiles,
                VotedOnFiles = user.VotedOnFiles,
                SubscribedAuthors = user.SubscribedAuthors,
                UserSubscriptions = user.UserSubscriptions,
            };

            var userRoles = from ur in _context.UserRoles
                            join r in _context.Roles on ur.RoleId equals r.Id
                            select new { ur.UserId, ur.RoleId, r.Name };

            user.Roles = userRoles.Where(x => x.UserId == user.ObjectIdentifier)
                .ToDictionary(key => key.RoleId, val => val.Name);

            return u;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error: {response}", ex.Message);
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
            var u = _context.Users.Where(x => x.Id == user.ObjectIdentifier).First();

            u.Id = user.Id;
            u.Email = user.EmailAddress;
            u.EmailConfirmed = true;
            u.UserName = user.DisplayName;

            await _userManager.UpdateAsync(u);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error: {response}", ex.Message);
        }
    }
    
    public record UserRegistrationModel(
        string FirstName,
        string LastName,
        string DisplayName,
        string EmailAddress,
        string Password,
        string FileName
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

                    //string token = await _userManager.GenerateEmailConfirmationTokenAsync(existingUser);
                    //string confirmationLink = Url.Action(
                    //    "ConfirmEmail",
                    //    "Account",
                    //    new { userId = existingUser.Id, token },
                    //    Request.Scheme);

                    //CreatedAtAction("RegisterConfirmation", new { message = "User registered successfully." });

                    var mongoUser = new UserModel()
                    {
                        ObjectIdentifier = existingUser.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        DisplayName = user.DisplayName,
                        EmailAddress = user.EmailAddress,
                        FileName = user.FileName ?? "",
                    };

                    await _userData.CreateUserAsync(mongoUser);

                    return Ok();
                }
            }
        }
        return BadRequest();
    }

    [AllowAnonymous]
    [HttpGet("confirm-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (userId is null || token is null)
        {
            return BadRequest("User ID and token are required.");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return BadRequest("User not found.");
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            return Ok("Email confirmed successfully.");
        }
        else
        {
            return BadRequest("Email confirmation failed.");
        }
    }

    [AllowAnonymous]
    [HttpGet("email-confirmed")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult EmailConfirmed()
    {
        return Ok("Email confirmed successfully.");
    }
}
