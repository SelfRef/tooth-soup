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
using Namotion.Reflection;

namespace ToothSoupAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = UserRole.PATIENT)]
	public class PatientController : ControllerBase
	{
		private readonly Database _db;

		public PatientController(Database database)
		{
			_db = database;
		}

		[HttpGet("Me")]
		public async Task<ActionResult<PatientResult>> GetMyInfo()
		{
			var id = GetPatientId();
			if (!id.HasValue) return Unauthorized();

			var patient = await _db.Patients
				.Where(p => p.UserId == id)
				.Select(p => new PatientResult {
					Pesel = p.Pesel,
					FirstName = p.User.FirstName,
					LastName = p.User.LastName,
					Email = p.User.Email,
					BirthDate = p.BirthDate,
					DentistName = GetUserName(p.Dentist.User),
					DentistId = p.DentistId
				})
				.SingleOrDefaultAsync();
			if (patient == null) return NotFound();

			return patient;
		}

		[HttpPut("Me")]
		public async Task<ActionResult<User>> Put(UserUpdate newUser)
		{
			var id = GetPatientId();
			if (!id.HasValue) return Unauthorized();
			
			var user = await _db.Users.FindAsync(id);
			if (user == null) return NotFound("User");
			if (!newUser.Email.IsNullOrEmpty()) user.Email = newUser.Email;
			if (!newUser.Password.IsNullOrEmpty()) user.Password = newUser.Password;

			if (newUser.DentistId.HasValue) {
				var patient = await _db.Patients.FirstOrDefaultAsync(p => p.UserId == id);
				var dentist = await _db.Dentists.FindAsync(newUser.DentistId.Value);
				if (patient == null) return NotFound("Patient");
				if (dentist == null) return NotFound("Dentist");
				if (patient.DentistId != dentist.Id) {
					if (dentist.PreventUserLinking) return Forbid("PreventUserLinking");
					patient.DentistId = dentist.Id;
				}
			}

			await _db.SaveChangesAsync();
			return CreatedAtAction(nameof(GetMyInfo), GetMyInfo().Result.Value);
		}

		[HttpGet("Dentists")]
		public async Task<ActionResult<IEnumerable<DentistResult>>> GetDentists()
		{
			var id = GetPatientId();
			if (!id.HasValue) return Unauthorized();

			var dentists = await _db.Dentists
				.Where(d => !d.PreventUserLinking || d.Patients.Any(p => p.UserId == id))
				.Select(d => new DentistResult {
					Id = d.Id,
					FirstName = d.User.FirstName,
					LastName = d.User.LastName
				})
				.ToListAsync();

			return dentists;
		}

		// [HttpPost]
		// public async Task<ActionResult<Appointment>> RegisterAppointment(Appointment appointment)
		// {
		// 	return CreatedAtAction(nameof(GetAppointment), new { appointment.Id }, appointment);
		// }

		private int? GetPatientId()
		{
			var isPatient = HttpContext.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == UserRole.PATIENT);
			if (!isPatient) return null;
			return int.Parse(HttpContext.User.Identity.Name);
		}

		private static string GetUserName(User user) {
			if (user != null) return $"{user.FirstName} {user.LastName}";
			return null;
		}
	}
}
