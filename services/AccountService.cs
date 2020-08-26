using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreCustomIdentyJwtDemo.Models;
using AspNetCoreCustomIdentyJwtDemo.services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AspNetCoreCustomIdentityJwtDemo.services
{
    public interface IAccountService
    {
        Task<bool> IsValidUserCredentials(LoginRequest loginRequest);

        Task<LoginResult> Login(LoginRequest loginRequest);

        Task<IdentityResult> Register(Registration registration);

        void Logout(string? userName);

        JwtAuthResult Refresh(string requestRefreshToken, string accessToken, in DateTime now);
    }

    public class AccountService : IAccountService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtAuthService _jwtAuthService;
        private readonly ILogger<AccountService> _logger;

        public AccountService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IJwtAuthService jwtAuthService, ILogger<AccountService> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtAuthService = jwtAuthService;
            _logger = logger;
        }

        public async Task<bool> IsValidUserCredentials(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.UserName);

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);

            return signInResult.Succeeded;
        }

        public async Task<LoginResult> Login(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.UserName);
            
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginRequest.UserName)
            };

            if (roles != null)
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

            
            var jwtResult = _jwtAuthService.GenerateTokens(loginRequest.UserName, claims.ToArray(), DateTime.Now);
            _logger.LogInformation($"User [{loginRequest.UserName}] logged in the system.");
            return new LoginResult
            {
                UserName = loginRequest.UserName,
                Roles = roles?.ToArray(),
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.TokenString
            };
        }

        public async Task<IdentityResult> Register(Registration registration)
        {
            var user = new ApplicationUser
            {
                UserName = registration.UserName,
                Email = registration.Email,
                FirstName = registration.FirstName,
                LastName = registration.LastName,
                PhoneNumber = registration.PhoneNumber,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, registration.Password);

            return result;
        }

        public void Logout(string? userName)
        {
            _jwtAuthService.RemoveRefreshTokenByUserName(userName);
            _logger.LogInformation($"User [{userName}] logged out the system.");
        }

        public JwtAuthResult Refresh(string requestRefreshToken, string accessToken, in DateTime now)
        {
            return _jwtAuthService.Refresh(requestRefreshToken, accessToken, DateTime.Now);
        }
    }
}