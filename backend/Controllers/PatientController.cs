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
	public class PatientController : ControllerBase
	{
		private readonly Database _db;

		public PatientController(Database database)
		{
			_db = database;
		}

		[HttpGet]
		public async Task<ActionResult<User>> Get()
		{
			var user = await _db.Users.FindAsync(); // TODO: Get user id
			user.Password = null;
			return user;
		}

		[HttpPut]
		public async Task<ActionResult<User>> Put(User newUser)
		{
			var user = await _db.Users.FindAsync(); // TODO: Get user id
			if (user == null) return NotFound();
			user.Email = newUser.Email;
			user.Password = newUser.Password;
			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(Put), user);
		}

		// [HttpDelete]
		// public async Task<ActionResult> Delete()
		// {
		// 	var user = await _db.Users.FindAsync(id); // TODO: Get user id
		// 	if (user == null) return NotFound();
		// 	_db.Users.Remove(user);
		// 	await _db.SaveChangesAsync();
		// 	return Ok();
		// }
	}
}
