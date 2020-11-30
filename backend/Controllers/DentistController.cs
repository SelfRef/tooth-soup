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
using IdentityServer4.Extensions;

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

			bool unlinked = Request.Query.ContainsKey("unlinked");
			string filter = Request.Query["filter"].FirstOrDefault()?.ToLower();

			return await _db.Patients
				.Where(p => p.DentistId == (unlinked ? null : id))
				.Include(p => p.User)
				.Where(p => $"{p.Pesel} {p.User.FirstName} {p.User.LastName} {p.User.Email}".ToLower().Contains(filter ?? string.Empty))
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
			if (patient.DentistId != dentistId) return Unauthorized("Patient is not linked to you");

			return patient;
		}

		[HttpGet("Patient/{id}/Link")]
		public async Task<ActionResult<PatientResult>> LinkPatient(int id)
		{
			var dentistId = GetDentistId();
			if (!dentistId.HasValue) return Unauthorized();

			var patient = await _db.Patients
				.Where(p => p.Id == id)
				.Include(p => p.User)
				.FirstOrDefaultAsync();

			if (patient == null) return NotFound();
			if (patient.DentistId != null) return Unauthorized("Patient is already linked");

			patient.DentistId = dentistId;
			await _db.SaveChangesAsync();

			return new PatientResult
			{
				Id = patient.Id,
				Pesel = patient.Pesel,
				FirstName = patient.User.FirstName,
				LastName = patient.User.LastName,
				Email = patient.User.Email,
				BirthDate = patient.BirthDate,
				DentistId = patient.DentistId,
			};
		}

		[HttpGet("Patient/{id}/Unlink")]
		public async Task<ActionResult<PatientResult>> UninkPatient(int id)
		{
			var dentistId = GetDentistId();
			if (!dentistId.HasValue) return Unauthorized();

			var patient = await _db.Patients
				.Where(p => p.Id == id)
				.Include(p => p.User)
				.FirstOrDefaultAsync();

			if (patient == null) return NotFound();
			if (patient.DentistId != dentistId) return Unauthorized("Patient is not linked to you");

			patient.DentistId = null;
			await _db.SaveChangesAsync();

			return new PatientResult
			{
				Id = patient.Id,
				Pesel = patient.Pesel,
				FirstName = patient.User.FirstName,
				LastName = patient.User.LastName,
				Email = patient.User.Email,
				BirthDate = patient.BirthDate,
				DentistId = patient.DentistId,
			};
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

		[HttpPut("Patient")]
		public async Task<ActionResult<PatientResult>> UpdatePatient(PatientRequest patient)
		{
			var dentistId = GetDentistId();
			if (!dentistId.HasValue) return Unauthorized();

			var newPatient = await _db.Patients
				.Where(p => p.Id == patient.Id)
				.Include(p => p.User)
				.FirstOrDefaultAsync();

			if (newPatient == null) return NotFound();
			if (newPatient.DentistId != dentistId) return Unauthorized("Patient is not linked to you");

			if (!patient.Email.IsNullOrEmpty()) newPatient.User.Email = patient.Email;
			if (!patient.Password.IsNullOrEmpty()) newPatient.User.Password = patient.Password;
			if (!patient.FirstName.IsNullOrEmpty()) newPatient.User.FirstName = patient.FirstName;
			if (!patient.LastName.IsNullOrEmpty()) newPatient.User.LastName = patient.LastName;
			if (!patient.Pesel.IsNullOrEmpty()) newPatient.Pesel = patient.Pesel;
			if (patient.BirthDate != null) newPatient.BirthDate = patient.BirthDate;

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
			var dentistId = GetDentistId();
			if (!dentistId.HasValue) return Unauthorized();

			var appointments = await _db.Appointments
				.Where(a => a.DentistId == dentistId)
				.ToListAsync();
			return appointments;
		}

		[HttpGet("Appointments/Patient/{id}")]
		public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointmentsForPatient(int id)
		{
			var dentistId = GetDentistId();
			if (!dentistId.HasValue) return Unauthorized();

			var appointments = await _db.Appointments
				.Where(a => a.DentistId == dentistId && a.PatientId == id)
				.ToListAsync();
			return appointments;
		}

		[HttpGet("Appointment/{id}")]
		public async Task<ActionResult<Appointment>> GetAppointment(int id)
		{
			var dentistId = GetDentistId();
			if (!dentistId.HasValue) return Unauthorized();

			var appointment = await _db.Appointments
				.Where(a => a.DentistId == dentistId && a.Id == id)
				.FirstOrDefaultAsync();
			if (appointment == null) return NotFound();
			return appointment;
		}

		[HttpPost("Appointment")]
		public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment)
		{
			var dentistId = GetDentistId();
			if (!dentistId.HasValue) return Unauthorized();

			appointment.DentistId = dentistId.Value;
			await _db.Appointments.AddAsync(appointment);
			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(GetAppointment), new { appointment.Id }, appointment);
		}

		[HttpPut("Appointment")]
		public async Task<ActionResult<Appointment>> PutAppointment(AppointmentRequest newAppointment)
		{
			var dentistId = GetDentistId();
			if (!dentistId.HasValue) return Unauthorized();

			var appointment = await _db.Appointments
				.Where(a => a.DentistId == dentistId && a.Id == newAppointment.Id)
				.FirstOrDefaultAsync();
			if (appointment == null) return NotFound();
			if (newAppointment.DateTime.HasValue) appointment.DateTime = newAppointment.DateTime.Value;
			if (newAppointment.Duration.HasValue) appointment.Duration = newAppointment.Duration.Value;
			if (newAppointment.Canceled.HasValue) appointment.Canceled = newAppointment.Canceled.Value;
			
			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(GetAppointment), new { appointment.Id }, appointment);
		}

		[HttpDelete("Appointment/{id}")]
		public async Task<ActionResult> DeleteAppointment(int id)
		{
			var dentistId = GetDentistId();
			if (!dentistId.HasValue) return Unauthorized();

			var appointment = await _db.Appointments
				.Where(a => a.DentistId == dentistId && a.Id == id)
				.FirstOrDefaultAsync();
			if (appointment == null) return NotFound();

			_db.Appointments.Remove(appointment);
			await _db.SaveChangesAsync();
			
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
