using System.Reflection.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ToothSoupAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ToothSoupAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AdminController : ControllerBase
	{
		private readonly Database _db;

		public AdminController(Database database)
		{
			_db = database;
		}

		[HttpGet("Users")]
		public async Task<ActionResult<IEnumerable<User>>> GetUserList()
		{
			var users = await _db.Users.ToListAsync();
			users.ForEach(u => u.Password = null);
			return users;
		}

		[HttpGet("User/{id}")]
		public async Task<ActionResult<User>> GetUser(int id)
		{
			var user = await _db.Users.FindAsync(id);
			user.Password = null;
			return user;
		}

		[HttpPost("User")]
		public async Task<ActionResult<User>> PostUser(User user)
		{
			await _db.Users.AddAsync(user);
			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(PostUser), new { user.Id }, user);
		}

		[HttpPut("User")]
		public async Task<ActionResult<User>> PutUser(User newUser)
		{
			var user = await _db.Users.FindAsync(newUser.Id);
			if (user == null) return NotFound();
			user.Email = newUser.Email;
			user.Password = newUser.Password;
			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(PutUser), user);
		}

		[HttpDelete("User/{id}")]
		public async Task<ActionResult> DeleteUser(int id)
		{
			var user = await _db.Users.FindAsync(id);
			if (user == null) return NotFound();
			_db.Users.Remove(user);
			await _db.SaveChangesAsync();
			return Ok();
		}
	}
}
