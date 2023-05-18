using GrooveBoxApi.Data;
using GrooveBoxApi.DataAccess;
using GrooveBoxApi.Models;
using GrooveBoxApiLibrary.Models;
using GrooveBoxApiLibrary.MongoDataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
    private readonly IEmailSender _emailSender;

    public UserController(ApplicationDbContext context,
                          UserManager<IdentityUser> userManager,
                          IUserData userData,
                          ILogger<UserController> logger,
                          IEmailSender emailSender)
    {
        _context = context;
        _userManager = userManager;
        _userData = userData;
        _logger = logger;
        _emailSender = emailSender;
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
                    EmailConfirmed = false,
                    UserName = user.DisplayName,
                };

                IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);

                if (result.Succeeded)
                {
                    string token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

                    string callbackUrl = Url.Action("ConfirmEmail", "User", 
                        new { userId = newUser.Id, token }, Request.Scheme);

                    await _emailSender.SendEmailAsync(user.EmailAddress, "Confirm your account",
                        $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.");

                    var mongoUser = new UserModel()
                    {
                        ObjectIdentifier = newUser.Id,
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
    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
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
    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.EmailAddress);
            if (user is null || await _userManager.IsEmailConfirmedAsync(user) is false)
            {
                return Ok();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { email = model.EmailAddress, token }, Request.Scheme);

            await _emailSender.SendEmailAsync(model.EmailAddress, "Reset Password",
                $"Please reset your password by <a href='{callbackUrl}'>clicking here</a>.");

            return Ok();
        }

        return BadRequest(ModelState);
    }

    [AllowAnonymous]
    [HttpPost("ResetEmail")]
    public async Task<IActionResult> ResetEmail(ResetEmailModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user is null)
            {
                return Ok(); 
            }

            string token = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);

            string callbackUrl = Url.Action("ConfirmEmailChange", "User",
                new { userId = user.Id, email = model.NewEmail, token }, Request.Scheme);

            string confirmationEmailContent = @$"Please confirm your email change by clicking the following link: 
                <a href='{callbackUrl}'>Confirm Email Change</a>.";

            await _emailSender.SendEmailAsync(user.Email, "Confirm Email Change", confirmationEmailContent);

            return Ok();
        }

        return BadRequest(ModelState);
    }

    [AllowAnonymous]
    [HttpGet("ConfirmEmailChange")]
    public async Task<IActionResult> ConfirmEmailChange(string userId, string email, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var mongoUser = await _userData.GetUserFromAuthenticationAsync(userId);

        if (user is null)
        {
            return NotFound();
        }

        var result = await _userManager.ChangeEmailAsync(user, email, token);

        if (result.Succeeded)
        {
            mongoUser.EmailAddress = email;
            await _userData.UpdateUserAsync(mongoUser);
            return Ok();
        }

        return BadRequest();
    }
}
