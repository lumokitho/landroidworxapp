using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LandroidWorxApp.BusinessLogic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace LandroidWorxApp.Pages
{
    public class LoginLogicModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILsClientWeb _lsClientWeb;

        public LoginLogicModel(ILsClientWeb lsClientWeb, IConfiguration configuration)
        {
            _lsClientWeb = lsClientWeb;
            _configuration = configuration;
        }

        public string ReturnUrl { get; set; }
        public async Task<IActionResult>
            OnGetAsync(string paramUsername, string paramPassword, bool paramRememberMe)
        {
            string returnUrl = Url.Content("~/");
            try
            {
                // Clear the existing external cookie
                await HttpContext
                    .SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
            }
            catch { }


            var response = _lsClientWeb.Login(new LsClientWeb_LoginRequest()
            {
                ClientSecret = _configuration.GetValue<string>("ClientSecret"),
                GrantType = "password",
                Scope = "*",
                Username = paramUsername,
                Password = paramPassword
            });

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, paramUsername),
                new Claim(ClaimTypes.Role, "Administrator"),
                new Claim("BearerToken", response.BearerToken),
                new Claim("BrokerUrl", response.BearerToken),
            };
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = paramRememberMe,
                RedirectUri = this.Request.Host.Value
            };
            try
            {
                await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }

            return LocalRedirect(returnUrl);
        }
    }
}

