using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using General.Purpose.Communicator.Models;
using General.Purpose.Communicator.Interfaces;

namespace General.Purpose.Communicator.Controllers
{
    public class AccountController : Controller, IAccountController
    {
        public string Password { private get; set; }
        private readonly IPasswordManager _passwordManager;
        public AccountController(/*ApplicationContext context*/ IPasswordManager passwordManager)
        {
            //_context = context;
            _passwordManager = passwordManager;
        }
        [HttpPost("/token")]
        public async Task<IActionResult> Token(string username, string password)
        {
            var identity = await GetIdentity(username, password);
            if (identity == null)
            {
                return BadRequest("Invalid username or password.");
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };
            return Json(response);
        }

        private async Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            var person = new User();

            if ((username == "Vovan" || username == "KernelEvil") && password == _passwordManager.Password)
            {
                person.Email = username;

                person.Password = _passwordManager.Password;
                person.Role.Name = "admin";
                person.Role.Id = 1;
            }
            else
            {
                person = null;
            }
            

            //User person = await _context.Users.Include(r => r.Role).FirstOrDefaultAsync(x => x.Email == username && x.Password == password);
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role.Name)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            // если пользователь не найден
            return null;
        }
    }
}