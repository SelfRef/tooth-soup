
using System.Security.Claims;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ToothSoupAPI.Models;
using System.Linq;

namespace ToothSoupAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class LoginController : ControllerBase
	{
		private readonly IConfiguration _config;
		private readonly Database _db;

		public LoginController(IConfiguration config, Database db)
		{
			_config = config;
			_db = db;
		}
		[AllowAnonymous]
		[HttpPost]
		public IActionResult Login([FromBody] User login)
		{
			IActionResult response = Unauthorized();
			var user = AuthenticateUser(login);

			if (user != null)
			{
				var tokenString = GenerateJSONWebToken(user);
				response = Ok(new { token = tokenString });
			}

			return response;
		}

		private string GenerateJSONWebToken(User userInfo)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var claims = new[] {
				new Claim("Role", userInfo.Role.ToString()),
				new Claim("Id", userInfo.Id.ToString())
			};

			var token = new JwtSecurityToken(_config["Jwt:Issuer"],
				_config["Jwt:Issuer"],
				claims,
				expires: DateTime.Now.AddMinutes(120),
				signingCredentials: credentials);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		private User AuthenticateUser(User userData)
		{
			User user = _db.Users.Where(u => u.Email == userData.Email && u.Password == userData.Password).FirstOrDefault();

			return user;
		}
	}
}