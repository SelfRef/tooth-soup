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

namespace ToothSoupAPI.Controllers
{
	[ApiController]
	[Authorize]
	[Route("api/[controller]")]
	public class DentistController : ControllerBase
	{
		private readonly Database _db;

		public DentistController(Database database)
		{
			_db = database;
		}

		[HttpGet("Patients")]
		public async Task<ActionResult<IEnumerable<PatientResult>>> GetPatients()
		{
			var id = GetDentistId();
			if (id == null) return Unauthorized();

			return await _db.Patients.Where(p => p.DentistId.ToString() == id).Select(p => new PatientResult {
				Id = p.Id,
				FirstName = p.FirstName,
				LastName = p.LastName,
				Email = p.User.Email,
			}).ToListAsync();
		}

		[HttpGet("Patient/{id}")]
		public async Task<ActionResult<Patient>> GetPatient(int patientId)
		{
			var id = GetDentistId();
			if (id == null) return Unauthorized();

			var patient = await _db.Patients.Where(p => p.DentistId.ToString() == id).Include(p => p.User).FirstOrDefaultAsync(p => p.Id == patientId);
			if (patient == null) return NotFound();
			return patient;
		}

		[HttpGet("Appointments")]
		public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
		{
			var id = GetDentistId();
			if (id == null) return Unauthorized();

			var appointments = await _db.Appointments
				.Where(a => a.DentistId.ToString() == id)
				.ToListAsync();
			return appointments;
		}

		[HttpGet("Appointment/{id}")]
		public async Task<ActionResult<Appointment>> GetAppointment(int appointmentId)
		{
			var id = GetDentistId();
			if (id == null) return Unauthorized();

			var appointment = await _db.Appointments
				.Where(a => a.DentistId.ToString() == id && a.Id == appointmentId)
				.FirstOrDefaultAsync();
			if (appointment == null) return NotFound();
			return appointment;
		}

		[HttpPost("Appointment")]
		public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment)
		{
			var id = GetDentistId();
			if (id == null) return Unauthorized();

			appointment.DentistId = int.Parse(id);
			await _db.Appointments.AddAsync(appointment);
			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(PostAppointment), new { appointment.Id }, appointment);
		}

		[HttpPut("Appointment")]
		public async Task<ActionResult<Appointment>> PutAppointment(Appointment newAppointment)
		{
			var id = GetDentistId();
			if (id == null) return Unauthorized();

			var appointment = await _db.Appointments
				.Where(a => a.DentistId.ToString() == id && a.Id == newAppointment.Id)
				.FirstOrDefaultAsync();
			if (appointment == null) return NotFound();
			appointment.DateTime = newAppointment.DateTime;
			appointment.Duration = newAppointment.Duration;
			appointment.DentistId = appointment.DentistId;
			appointment.PatientId = appointment.PatientId;
			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(PutAppointment), new { appointment.Id }, appointment);
		}

		[HttpDelete("Appointment/{id}")]
		public async Task<ActionResult> DeleteAppointment(int appointmentId)
		{
			var id = GetDentistId();
			if (id == null) return Unauthorized();

			var appointment = await _db.Appointments
				.Where(a => a.DentistId.ToString() == id && a.Id == appointmentId)
				.FirstOrDefaultAsync();
			if (appointment == null) return NotFound();
			_db.Appointments.Remove(appointment);
			return Ok();
		}

		private string GetDentistId()
		{
			var isDentist = HttpContext.User.HasClaim(c => c.Type == "Role" && c.Value == nameof(UserRole.Dentist));
			if (!isDentist) return null;
			return HttpContext.User.Claims.SingleOrDefault(c => c.Type == "Id")?.Value;
		}
	}
}
