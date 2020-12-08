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
			var userId = GetPatientId();
			if (!userId.HasValue) return Unauthorized();

			var patient = await _db.Patients
				.Where(p => p.UserId == userId)
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
		public async Task<ActionResult<PatientResult>> Put(UserUpdate newUser)
		{
			var userId = GetPatientId();
			if (!userId.HasValue) return Unauthorized();
			
			var user = await _db.Users.FindAsync(userId);
			if (user == null) return NotFound("User");
			if (!newUser.Email.IsNullOrEmpty()) user.Email = newUser.Email;
			if (!newUser.Password.IsNullOrEmpty()) user.Password = newUser.Password;

			if (newUser.DentistId.HasValue) {
				var patient = await _db.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
				var dentist = await _db.Dentists.FindAsync(newUser.DentistId.Value);
				if (patient == null) return NotFound("Patient");
				if (dentist == null) return NotFound("Dentist");
				if (patient.DentistId != dentist.Id) {
					if (!dentist.CanLink) return Forbid("CanLink");
					patient.DentistId = dentist.Id;
				}
			}

			await _db.SaveChangesAsync();
			return CreatedAtAction(nameof(GetMyInfo), GetMyInfo().Result.Value);
		}

		[HttpGet("Dentists")]
		public async Task<ActionResult<IEnumerable<DentistResult>>> GetDentists()
		{
			var userId = GetPatientId();
			if (!userId.HasValue) return Unauthorized();

			var patient = await _db.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
			if (patient == null) return NotFound("Patient");

			var dentists = await _db.Dentists
				.Select(d => new DentistResult {
					Id = d.Id,
					FirstName = d.User.FirstName,
					LastName = d.User.LastName,
					CanLink = d.CanLink,
					CanCreateAppointment = d.CanCreateAppointment || d.Patients.Any(p => p.Id == patient.Id)
				})
				.ToListAsync();

			return dentists;
		}

		[HttpGet("Services")]
		public async Task<ActionResult<IEnumerable<ServiceResponse>>> GetServices()
		{
			var userId = GetPatientId();
			if (!userId.HasValue) return Unauthorized();

			var services = await _db.Services
				.Where(s => !s.Deleted)
				.Select(s => new ServiceResponse {
					Id = s.Id,
					Name = s.Name,
					Price = s.Price
				})
				.ToListAsync();
			return services;
		}

		[HttpGet("Appointments")]
		public async Task<ActionResult<IEnumerable<AppointmentPatientResponse>>> GetAppointments()
		{
			var userId = GetPatientId();
			if (!userId.HasValue) return Unauthorized();

			var patient = await _db.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
			if (patient == null) return NotFound("Patient");

			var appointments = await _db.Appointments
				.Where(a => a.PatientId == patient.Id)
				.Select(a => new AppointmentPatientResponse {
					Id = a.Id,
					StartDate = a.StartDate,
					EndDate = a.EndDate,
					Duration = a.Duration,
					Canceled = a.Canceled,
					DentistId = a.DentistId,
					DentistName = GetUserName(a.Dentist.User),
					ServiceId = a.ServiceId,
					ServiceName = a.Service.Name
				})
				.ToListAsync();

			return appointments;
		}

		[HttpGet("Appointments/{id}")]
		public async Task<ActionResult<AppointmentPatientResponse>> GetAppointment(int id)
		{
			var userId = GetPatientId();
			if (!userId.HasValue) return Unauthorized();

			var patient = await _db.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
			if (patient == null) return NotFound("Patient");

			var appointment = await _db.Appointments
				.FindAsync(id);
			
			if (appointment == null || appointment.PatientId != patient.Id) return NotFound();

			return new AppointmentPatientResponse {
				Id = appointment.Id,
				StartDate = appointment.StartDate,
				EndDate = appointment.EndDate,
				Duration = appointment.Duration,
				Canceled = appointment.Canceled,
				DentistId = appointment.DentistId,
				DentistName = GetUserName(appointment.Dentist.User),
				ServiceId = appointment.ServiceId,
				ServiceName = appointment.Service.Name
			};
		}

		[HttpGet("Appointments/Dentists/{dentistId}/{date}")]
		public async Task<ActionResult<IEnumerable<AppointmentPatientResponse>>> GetDentistAppointments(int dentistId, DateTime date)
		{
			var userId = GetPatientId();
			if (!userId.HasValue) return Unauthorized();

			var patient = await _db.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
			if (patient == null) return NotFound("Patient");

			var appointments = await _db.Appointments
				.Where(a => a.DentistId == dentistId && a.StartDate.Date == date.Date && !a.Canceled)
				.Select(a => new AppointmentPatientResponse {
					Id = a.Id,
					StartDate = a.StartDate,
					EndDate = a.EndDate,
					Duration = a.Duration,
					Canceled = a.Canceled,
					DentistId = a.DentistId,
					DentistName = GetUserName(a.Dentist.User),
					ServiceId = a.PatientId == patient.Id ? a.ServiceId : 0,
					ServiceName = a.PatientId == patient.Id ? a.Service.Name : null,
				})
				.ToListAsync();

			return appointments;
		}

		[HttpPost("Appointments")]
		public async Task<ActionResult<AppointmentPatientResponse>> PostAppointment(Appointment appointment)
		{
			var userId = GetPatientId();
			if (!userId.HasValue) return Unauthorized();

			var patient = await _db.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
			if (patient == null) return NotFound("Patient");

			var dentist = await _db.Dentists.FindAsync(appointment.DentistId);
			if (dentist == null) return NotFound("Dentist");
			if (!dentist.CanCreateAppointment && !dentist.Patients.Any(p => p.Id == patient.Id)) return Forbid("CanCreateAppointment");

			appointment.PatientId = patient.Id;
			await _db.Appointments.AddAsync(appointment);
			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(GetAppointment), new { appointment.Id }, null);
		}

		[HttpPut("Appointments")]
		public async Task<ActionResult<AppointmentPatientResponse>> PutAppointment(AppointmentRequest newAppointment)
		{
			var userId = GetPatientId();
			if (!userId.HasValue) return Unauthorized();

			var patient = await _db.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
			if (patient == null) return NotFound("Patient");

			var appointment = await _db.Appointments
				.Where(a => a.PatientId == patient.Id && a.Id == newAppointment.Id)
				.FirstOrDefaultAsync();
			if (appointment == null) return NotFound();
			if (newAppointment.StartDate.HasValue) appointment.StartDate = newAppointment.StartDate.Value;
			if (newAppointment.EndDate.HasValue) appointment.EndDate = newAppointment.EndDate.Value;
			if (newAppointment.Canceled.HasValue) appointment.Canceled = newAppointment.Canceled.Value;
			
			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(GetAppointment), new { appointment.Id }, null);
		}

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
