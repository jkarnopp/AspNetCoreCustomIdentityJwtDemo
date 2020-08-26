using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreCustomIdentityJwtDemo.services;
using AspNetCoreCustomIdentyJwtDemo.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCoreCustomIdentityJwtDemo.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountService accountService, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _accountService = accountService;
            _logger = logger;
        }

        
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!await _accountService.IsValidUserCredentials(request))
            {
                return Unauthorized();
            }

            return Ok(await _accountService.Login(request));
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Registration registration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _accountService.Register(registration);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                var request = new LoginRequest(registration.UserName, registration.Password);
                return Ok(await _accountService.Login(request));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest();
        }

        [HttpGet("user")]
        [Authorize(AuthenticationSchemes = JwtTokenConfig.AuthSchemes)]
        public ActionResult GetCurrentUser()
        {
            return Ok(new LoginResult
            {
                UserName = User.Identity.Name,
                Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray(),
                OriginalUserName = User.FindFirst("OriginalUserName")?.Value
            });
        }

        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = JwtTokenConfig.AuthSchemes)]
        public ActionResult Logout()
        {
            var userName = User.Identity.Name;
            _accountService.Logout(userName);
            return Ok();
        }

        [HttpPost("refresh-token")]
        [Authorize(AuthenticationSchemes = JwtTokenConfig.AuthSchemes)]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var userName = User.Identity.Name;
                _logger.LogInformation($"User [{userName}] is trying to refresh JWT token.");

                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                {
                    return Unauthorized();
                }

                var accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");

                
                var jwtResult = _accountService.Refresh(request.RefreshToken, accessToken, DateTime.Now);
                _logger.LogInformation($"User [{userName}] has refreshed JWT token.");
                return Ok(new LoginResult
                {
                    UserName = userName,
                    Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray(),
                    AccessToken = jwtResult.AccessToken,
                    RefreshToken = jwtResult.RefreshToken.TokenString
                });
            }
            catch (SecurityTokenException e)
            {
                return Unauthorized(e.Message); // return 401 so that the client side can redirect the user to login page
            }
        }
    }
}