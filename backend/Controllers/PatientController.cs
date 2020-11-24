using System.Reflection.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ToothSoupAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using IdentityServer4.Extensions;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace ToothSoupAPI.Controllers
{
	[ApiController]
	[Authorize(Roles = UserRole.PATIENT)]
	[Route("api/[controller]")]
	public class PatientController : ControllerBase
	{
		private readonly Database _db;
		private readonly UserManager<User> _userManager;

		public PatientController(Database database, UserManager<User> userManager)
		{
			_db = database;
			_userManager = userManager;
		}

		[HttpGet]
		public async Task<ActionResult<User>> Get()
		{
			var user = await _db.Users.FindAsync(User.Identity.Name);
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

		[HttpPost]
		public async Task<ActionResult<Appointment>> RegisterAppointment(Appointment appointment)
		{
			return CreatedAtAction(nameof(RegisterAppointment), appointment);
		}
	}
}
