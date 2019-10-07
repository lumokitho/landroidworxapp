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
    [IgnoreAntiforgeryToken]
    public class LoginActionModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILsClientWeb _lsClientWeb;

        public LoginActionModel(ILsClientWeb lsClientWeb, IConfiguration configuration)
        {
            _lsClientWeb = lsClientWeb;
            _configuration = configuration;
        }

        public string ReturnUrl { get; set; }
        public class LoginModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public bool RememberMe { get; set; }
            public string ReturnUrl { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string username, string password, bool rememberMe, string returnUrl)
        {
            returnUrl = returnUrl ?? Url.Content("~/"); 

            var response = _lsClientWeb.Login(new LsClientWeb_LoginRequest()
            {
                ClientSecret = _configuration.GetValue<string>("ClientSecret"),
                GrantType = "password",
                Scope = "*",
                Username = username,
                Password = password
            });

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Administrator"),
                new Claim("BearerToken", response.BearerToken),
                new Claim("BrokerUrl", response.BrokerUrl),
            };
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
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

        public async Task<IActionResult>
            OnPostAsync([FromBody] LoginModel model)
            {
            model.ReturnUrl = model.ReturnUrl ?? Url.Content("~/");
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
                Username = model.Username,
                Password = model.Password
            });

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Username),
                new Claim(ClaimTypes.Role, "Administrator"),
                new Claim("BearerToken", response.BearerToken),
                new Claim("BrokerUrl", response.BearerToken),
            };
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
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

            return LocalRedirect(model.ReturnUrl);
        }
    }
}

