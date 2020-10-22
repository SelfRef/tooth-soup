
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ToothSoupAPI.Models;

namespace ToothSoupAPI.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class LoginController : ControllerBase
	{
		private readonly IConfiguration _config;

		public LoginController(IConfiguration config)
		{
			_config = config;
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

			var token = new JwtSecurityToken(_config["Jwt:Issuer"],
				_config["Jwt:Issuer"],
				null,
				expires: DateTime.Now.AddMinutes(120),
				signingCredentials: credentials);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		private User AuthenticateUser(User userData)
		{
			User user = null;

			//Validate the User Credentials    
			//Demo Purpose, I have Passed HardCoded User Information    
			if (userData.Email == "janusz@ex.pl")
			{
				user = new User { Email = "janusz@ex.pl" };
			}
			return user;
		}
	}
}