using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;
using TcpExample.Domain.Common.UserService;

namespace TcpExample.UI.Controllers
{
    public class AccountController : Controller
    {
        IUserService userService;
        public AccountController(IUserService userService)
        {
          this.userService = userService;
        }

        [AllowAnonymous]
        public IActionResult Login(string error=null)
        {
            if (error != null)
                ViewBag.Error = error;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> Login(string UserName, string Password)
        {
            try
            {
                var result = userService.GetUseerByUserNamePassword(UserName, Password);
                if (result.Success)
                {

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, result.Data.FirstName + " "+ result.Data.LastName),
                        new Claim("UserId", result.Data.Id.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(90),
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);
                    return RedirectToAction("Index", "Home");
                }
                else {
                    return RedirectToAction("Login", "account", new { error = result.Message });
                }


            }
            catch (Exception e)
            {
                return RedirectToAction("Login", "account",new { error=e.Message});
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}