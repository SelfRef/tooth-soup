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
	public class PatientsController : ControllerBase
	{
		private readonly Database _db;

		public PatientsController(Database database)
		{
			_db = database;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Patient>>> GetList()
		{
			return await _db.Patients.ToArrayAsync();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<User>> Get(int id)
		{
			var user = await _db.Users.FindAsync(id);
			user.Password = null;
			return user;
		}

		[HttpPost]
		public async Task<ActionResult<User>> Post(User user)
		{
			await _db.Users.AddAsync(user);
			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(Post), new { user.Id }, user);
		}

		[HttpPut]
		public async Task<ActionResult<User>> Put(User newUser)
		{
			var user = await _db.Users.FindAsync(newUser.Id);
			if (user == null) return NotFound();
			user.Email = newUser.Email;
			user.Password = newUser.Password;
			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(Put), user);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			var user = await _db.Users.FindAsync(id);
			if (user == null) return NotFound();
			_db.Users.Remove(user);
			await _db.SaveChangesAsync();
			return Ok();
		}
	}
}
