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
using System.Security.Claims;

namespace ToothSoupAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = UserRole.DENTIST)]
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
			if (!id.HasValue) return Unauthorized();

			return await _db.Patients
				.Where(p => p.DentistId == id)
				.Include(p => p.User)
				.Select(p => new PatientResult {
					Id = p.Id,
					Pesel = p.Pesel,
					FirstName = p.User.FirstName,
					LastName = p.User.LastName,
					Email = p.User.Email,
					BirthDate = p.BirthDate,
					DentistId = p.DentistId,
				}).ToListAsync();
		}

		[HttpGet("Patient/{id}")]
		public async Task<ActionResult<PatientResult>> GetPatient(int id)
		{
			var dentistId = GetDentistId();
			if (!dentistId.HasValue) return Unauthorized();

			var patient = await _db.Patients
				.Where(p => p.Id == id)
				.Include(p => p.User)
				.Select(p => new PatientResult
				{
					Id = p.Id,
					Pesel = p.Pesel,
					FirstName = p.User.FirstName,
					LastName = p.User.LastName,
					Email = p.User.Email,
					BirthDate = p.BirthDate,
					DentistId = p.DentistId,
				}).FirstOrDefaultAsync();

			if (patient == null) return NotFound();
			if (patient.DentistId != dentistId) return Unauthorized("Patient is not assigned to you");

			return patient;
		}

		[HttpPost("Patient")]
		public async Task<ActionResult<PatientResult>> CreatePatient(PatientRequest patient)
		{
			var id = GetDentistId();
			if (!id.HasValue) return Unauthorized();

			var newUser = new User {
				Email = patient.Email,
				Password = patient.Password,
				FirstName = patient.FirstName,
				LastName = patient.LastName,
				Role = UserRole.PATIENT,
			};
			
			var newPatient = new Patient {
				Pesel = patient.Pesel,
				BirthDate = patient.BirthDate,
				User = newUser,
				DentistId = id.Value,
			};

			await _db.Patients.AddAsync(newPatient);
			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(GetPatient), new { newPatient.Id }, newPatient);
		}

		[HttpDelete("Patient/{id}")]
		public async Task<ActionResult<int>> DeletePatient(int id) {
			var dentistId = GetDentistId();
			if (!dentistId.HasValue) return Unauthorized();

			var patient = await _db.Patients.FindAsync(id);
			if (patient == null) return NotFound();

			_db.Patients.Remove(patient);
			await _db.SaveChangesAsync();
			return id;
		}

		[HttpGet("Appointments")]
		public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
		{
			var id = GetDentistId();
			if (!id.HasValue) return Unauthorized();

			var appointments = await _db.Appointments
				.Where(a => a.DentistId == id)
				.ToListAsync();
			return appointments;
		}

		[HttpGet("Appointments/Patient/{patientId}")]
		public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointmentsForPatient(int patientId)
		{
			var id = GetDentistId();
			if (!id.HasValue) return Unauthorized();

			var appointments = await _db.Appointments
				.Where(a => a.DentistId == id && a.PatientId == patientId)
				.ToListAsync();
			return appointments;
		}

		[HttpGet("Appointment/{appointmentId}")]
		public async Task<ActionResult<Appointment>> GetAppointment(int appointmentId)
		{
			var id = GetDentistId();
			if (!id.HasValue) return Unauthorized();

			var appointment = await _db.Appointments
				.Where(a => a.DentistId == id && a.Id == appointmentId)
				.FirstOrDefaultAsync();
			if (appointment == null) return NotFound();
			return appointment;
		}

		[HttpPost("Appointment")]
		public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment)
		{
			var id = GetDentistId();
			if (!id.HasValue) return Unauthorized();

			appointment.DentistId = id.Value;
			await _db.Appointments.AddAsync(appointment);
			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(GetAppointment), new { appointment.Id }, appointment);
		}

		[HttpPut("Appointment")]
		public async Task<ActionResult<Appointment>> PutAppointment(Appointment newAppointment)
		{
			var id = GetDentistId();
			if (!id.HasValue) return Unauthorized();

			var appointment = await _db.Appointments
				.Where(a => a.DentistId == id && a.Id == newAppointment.Id)
				.FirstOrDefaultAsync();
			if (appointment == null) return NotFound();
			appointment.DateTime = newAppointment.DateTime;
			appointment.Duration = newAppointment.Duration;
			appointment.DentistId = appointment.DentistId;
			appointment.PatientId = appointment.PatientId;
			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(GetAppointment), new { appointment.Id }, appointment);
		}

		[HttpDelete("Appointment/{appointmentId}")]
		public async Task<ActionResult> DeleteAppointment(int appointmentId)
		{
			var id = GetDentistId();
			if (!id.HasValue) return Unauthorized();

			var appointment = await _db.Appointments
				.Where(a => a.DentistId == id && a.Id == appointmentId)
				.FirstOrDefaultAsync();
			if (appointment == null) return NotFound();
			_db.Appointments.Remove(appointment);
			return Ok();
		}

		private int? GetDentistId()
		{
			var isDentist = HttpContext.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == UserRole.DENTIST);
			if (!isDentist) return null;
			return int.Parse(HttpContext.User.Identity.Name);
		}
	}
}
