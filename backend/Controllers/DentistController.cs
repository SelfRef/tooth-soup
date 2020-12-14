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
using Namotion.Reflection;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;
using System.Xml;

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

		[HttpGet("Me")]
		public async Task<ActionResult<DentistResult>> GetMyInfo()
		{
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			var dentist = await _db.Dentists
				.Where(p => p.UserId == userId)
				.Select(p => new DentistResult {
					FirstName = p.User.FirstName,
					LastName = p.User.LastName,
					Email = p.User.Email,
					CanLink = p.CanLink,
					CanCreateAppointment = p.CanCreateAppointment
				})
				.SingleOrDefaultAsync();
			if (dentist == null) return NotFound();

			return dentist;
		}

		[HttpPut("Me")]
		public async Task<ActionResult<DentistResult>> Put(DentistUpdate newUser)
		{
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();
			
			var user = await _db.Users.FindAsync(userId);
			if (user == null) return NotFound("User");
			if (!newUser.Email.IsNullOrEmpty()) user.Email = newUser.Email;
			if (!newUser.Password.IsNullOrEmpty()) user.Password = newUser.Password;

			var dentist = await _db.Dentists.FirstOrDefaultAsync(d => d.UserId == userId);
			if (dentist == null) return NotFound("Dentist");
			if (newUser.CanLink.HasValue) dentist.CanLink = newUser.CanLink.Value;
			if (newUser.CanCreateAppointment.HasValue) dentist.CanCreateAppointment = newUser.CanCreateAppointment.Value;

			await _db.SaveChangesAsync();
			return CreatedAtAction(nameof(GetMyInfo), GetMyInfo().Result.Value);
		}

		[HttpGet("Patients")]
		public async Task<ActionResult<IEnumerable<PatientResult>>> GetPatients()
		{
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			bool unlinked = Request.Query.ContainsKey("unlinked");
			string filter = Request.Query["filter"].FirstOrDefault()?.ToLower();

			var dentist = await _db.Dentists.FirstOrDefaultAsync(d => d.UserId == userId);
			if (dentist == null) return NotFound("Dentist");
			int? dentistId = dentist.Id;
			if (unlinked) dentistId = null;

			return await _db.Patients
				.Where(p => p.DentistId == dentistId)
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

		[HttpGet("Patients/{id}")]
		public async Task<ActionResult<PatientResult>> GetPatient(int id)
		{
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			var dentist = await _db.Dentists.FirstOrDefaultAsync(d => d.UserId == userId);
			if (dentist == null) return NotFound("Dentist");

			var patient = await _db.Patients
				.Where(p => p.Id == id)
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
			if (patient.DentistId != dentist.Id) return Unauthorized("Patient is not linked to you");

			return patient;
		}

		[HttpPut("Patients/{id}/Link")]
		public async Task<ActionResult<PatientResult>> LinkPatient(int id)
		{
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			var patient = await _db.Patients
				.Where(p => p.Id == id)
				.Include(p => p.User)
				.FirstOrDefaultAsync();

			if (patient == null) return NotFound();
			if (patient.DentistId != null) return BadRequest("Patient is already linked");

			var dentist = await _db.Dentists.FirstOrDefaultAsync(d => d.UserId == userId);
			if (dentist == null) return NotFound("Dentist");

			patient.DentistId = dentist.Id;
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

		[HttpPut("Patients/{id}/Unlink")]
		public async Task<ActionResult<PatientResult>> UninkPatient(int id)
		{
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			var patient = await _db.Patients
				.Where(p => p.Id == id)
				.Include(p => p.User)
				.FirstOrDefaultAsync();

			var dentist = await _db.Dentists.FirstOrDefaultAsync(d => d.UserId == userId);
			if (dentist == null) return NotFound("Dentist");

			if (patient == null) return NotFound();
			if (patient.DentistId != dentist.Id) return Unauthorized("Patient is not linked to you");

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

		[HttpPost("Patients")]
		public async Task<ActionResult<PatientResult>> CreatePatient(PatientRequest patient)
		{
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			var newUser = new User {
				Email = patient.Email,
				Password = patient.Password,
				FirstName = patient.FirstName,
				LastName = patient.LastName,
				Role = UserRole.PATIENT,
			};

			var dentist = await _db.Dentists.FirstOrDefaultAsync(d => d.UserId == userId);
			if (dentist == null) return NotFound("Dentist");
			
			var newPatient = new Patient {
				Pesel = patient.Pesel,
				BirthDate = patient.BirthDate,
				User = newUser,
				DentistId = dentist.Id,
			};

			await _db.Patients.AddAsync(newPatient);
			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(GetPatient), new { newPatient.Id }, null);
		}

		[HttpPut("Patients")]
		public async Task<ActionResult<PatientResult>> UpdatePatient(PatientRequest patient)
		{
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			var newPatient = await _db.Patients
				.Where(p => p.Id == patient.Id)
				.FirstOrDefaultAsync();

			var dentist = await _db.Dentists.FirstOrDefaultAsync(d => d.UserId == userId);
			if (dentist == null) return NotFound("Dentist");

			if (newPatient == null) return NotFound();
			if (newPatient.DentistId != dentist.Id) return Unauthorized("Patient is not linked to you");

			if (!patient.Email.IsNullOrEmpty()) newPatient.User.Email = patient.Email;
			if (!patient.Password.IsNullOrEmpty()) newPatient.User.Password = patient.Password;
			if (!patient.FirstName.IsNullOrEmpty()) newPatient.User.FirstName = patient.FirstName;
			if (!patient.LastName.IsNullOrEmpty()) newPatient.User.LastName = patient.LastName;
			if (!patient.Pesel.IsNullOrEmpty()) newPatient.Pesel = patient.Pesel;
			if (patient.BirthDate != null) newPatient.BirthDate = patient.BirthDate;

			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(GetPatient), new { newPatient.Id }, null);
		}

		[HttpGet("Appointments")]
		public async Task<ActionResult<IEnumerable<AppointmentResponse>>> GetAppointments()
		{
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			var dentist = await _db.Dentists.FirstOrDefaultAsync(d => d.UserId == userId);
			if (dentist == null) return NotFound("Dentist");

			var appointments = await _db.Appointments
				.Where(a => a.DentistId == dentist.Id)
				.Select(a => new AppointmentResponse {
					Id = a.Id,
					StartDate = a.StartDate,
					EndDate = a.EndDate,
					Duration = a.Duration,
					Canceled = a.Canceled,
					PatientId = a.PatientId,
					PatientName = $"{a.Patient.User.FirstName} {a.Patient.User.LastName}",
					ServiceId = a.ServiceId,
					ServiceName = a.Service.Name,
					ServicePrice = a.Service.Price,
				})
				.ToListAsync();
			return appointments;
		}

		[HttpGet("Appointments/Xml")]
		public async Task<ActionResult<string>> GetAppointmentsXml()
		{
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			var dentist = await _db.Dentists.FirstOrDefaultAsync(d => d.UserId == userId);
			if (dentist == null) return NotFound("Dentist");

			var appointments = await _db.Appointments
				.Where(a => a.DentistId == dentist.Id)
				.Select(a => new AppointmentResponse {
					Id = a.Id,
					StartDate = a.StartDate,
					EndDate = a.EndDate,
					Duration = a.Duration,
					Canceled = a.Canceled,
					PatientId = a.PatientId,
					PatientName = $"{a.Patient.User.FirstName} {a.Patient.User.LastName}",
					ServiceId = a.ServiceId,
					ServiceName = a.Service.Name,
					ServicePrice = a.Service.Price,
				})
				.ToListAsync();

			var appointmentsRoot = new DentistAppointmentList {
				Appointments = appointments
			};

			var serializer = new XmlSerializer(typeof(DentistAppointmentList));
			var ms = new MemoryStream();
			serializer.Serialize(ms, appointmentsRoot);
			ms.Position = 0;
			var sr = new StreamReader(ms);
			return sr.ReadToEnd();
		}

		[HttpGet("Appointments/Patients/{id}")]
		public async Task<ActionResult<IEnumerable<AppointmentResponse>>> GetAppointmentsForPatient(int id)
		{
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			var dentist = await _db.Dentists.FirstOrDefaultAsync(d => d.UserId == userId);
			if (dentist == null) return NotFound("Dentist");

			var appointments = await _db.Appointments
				.Where(a => a.DentistId == dentist.Id && a.PatientId == id)
				.Select(a => new AppointmentResponse
				{
					Id = a.Id,
					StartDate = a.StartDate,
					EndDate = a.EndDate,
					Duration = a.Duration,
					Canceled = a.Canceled,
					PatientId = a.PatientId,
					PatientName = $"{a.Patient.User.FirstName} {a.Patient.User.LastName}",
					ServiceId = a.ServiceId,
					ServiceName = a.Service.Name,
					ServicePrice = a.Service.Price,
				})
				.ToListAsync();
			return appointments;
		}

		[HttpGet("Appointments/Patients/{id}/Xml")]
		public async Task<ActionResult<string>> GetAppointmentsForPatientXml(int id)
		{
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			var dentist = await _db.Dentists.FirstOrDefaultAsync(d => d.UserId == userId);
			if (dentist == null) return NotFound("Dentist");

			var appointments = await _db.Appointments
				.Where(a => a.DentistId == dentist.Id && a.PatientId == id)
				.Select(a => new AppointmentResponse
				{
					Id = a.Id,
					StartDate = a.StartDate,
					EndDate = a.EndDate,
					Duration = a.Duration,
					Canceled = a.Canceled,
					PatientId = a.PatientId,
					PatientName = $"{a.Patient.User.FirstName} {a.Patient.User.LastName}",
					ServiceId = a.ServiceId,
					ServiceName = a.Service.Name,
					ServicePrice = a.Service.Price,
				})
				.ToListAsync();

			var appointmentsRoot = new DentistAppointmentList {
				Appointments = appointments
			};
			
			var serializer = new XmlSerializer(typeof(DentistAppointmentList));
			var ms = new MemoryStream();
			serializer.Serialize(ms, appointmentsRoot);
			ms.Position = 0;
			var sr = new StreamReader(ms);
			return sr.ReadToEnd();
		}

		[HttpGet("Appointments/{id}")]
		public async Task<ActionResult<AppointmentResponse>> GetAppointment(int id)
		{
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			var dentist = await _db.Dentists.FirstOrDefaultAsync(d => d.UserId == userId);
			if (dentist == null) return NotFound("Dentist");

			var appointment = await _db.Appointments
				.Where(a => a.DentistId == dentist.Id && a.Id == id)
				.Select(a => new AppointmentResponse
				{
					Id = a.Id,
					StartDate = a.StartDate,
					EndDate = a.EndDate,
					Duration = a.Duration,
					Canceled = a.Canceled,
					PatientId = a.PatientId,
					PatientName = $"{a.Patient.User.FirstName} {a.Patient.User.LastName}",
					ServiceId = a.ServiceId,
					ServiceName = a.Service.Name,
					ServicePrice = a.Service.Price,
				})
				.FirstOrDefaultAsync();
			if (appointment == null) return NotFound();
			return appointment;
		}

		[HttpPost("Appointments")]
		public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment)
		{
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			var dentist = await _db.Dentists.FirstOrDefaultAsync(d => d.UserId == userId);
			if (dentist == null) return NotFound("Dentist");

			appointment.DentistId = dentist.Id;
			await _db.Appointments.AddAsync(appointment);
			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(GetAppointment), new { appointment.Id }, null);
		}

		[HttpPut("Appointments")]
		public async Task<ActionResult<Appointment>> PutAppointment(AppointmentRequest newAppointment)
		{
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			var dentist = await _db.Dentists.FirstOrDefaultAsync(d => d.UserId == userId);
			if (dentist == null) return NotFound("Dentist");

			var appointment = await _db.Appointments
				.Where(a => a.DentistId == dentist.Id && a.Id == newAppointment.Id)
				.FirstOrDefaultAsync();
			if (appointment == null) return NotFound();
			if (newAppointment.StartDate.HasValue) appointment.StartDate = newAppointment.StartDate.Value;
			if (newAppointment.EndDate.HasValue) appointment.EndDate = newAppointment.EndDate.Value;
			if (newAppointment.Canceled.HasValue) appointment.Canceled = newAppointment.Canceled.Value;
			
			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(GetAppointment), new { appointment.Id }, null);
		}

		[HttpDelete("Appointments/{id}")]
		public async Task<ActionResult> DeleteAppointment(int id)
		{
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			var dentist = await _db.Dentists.FirstOrDefaultAsync(d => d.UserId == userId);
			if (dentist == null) return NotFound("Dentist");

			var appointment = await _db.Appointments
				.Where(a => a.DentistId == dentist.Id && a.Id == id)
				.FirstOrDefaultAsync();
			if (appointment == null) return NotFound();

			_db.Appointments.Remove(appointment);
			await _db.SaveChangesAsync();
			
			return Ok();
		}

		[HttpGet("Services")]
		public async Task<ActionResult<IEnumerable<ServiceResponse>>> GetServices()
		{
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			var dentist = await _db.Dentists.Include(d => d.Services).FirstOrDefaultAsync(d => d.UserId == userId);
			if (dentist == null) return NotFound("Dentist");

			var linkedOnly = Request.Query.ContainsKey("Linked");

			var services = await _db.Services
				.Where(s => !s.Deleted)
				.Select(s => new ServiceResponse {
					Id = s.Id,
					Name = s.Name,
					Price = s.Price,
					AppointmentsCount = _db.Appointments.Where(a => a.ServiceId == s.Id).Count(),
					Linked = s.Dentists.Contains(dentist),
					DentistsCount = s.Dentists.Count,
					CanEdit = !s.Dentists.Any(d => d.UserId != userId)
				})
				.Where(s => !linkedOnly || s.Linked)
				.ToListAsync();

			return services;
		}

		[HttpGet("Services/{id}")]
		public async Task<ActionResult<ServiceResponse>> GetService(int id)
		{
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			var dentist = await _db.Dentists.FirstOrDefaultAsync(d => d.UserId == userId);
			if (dentist == null) return NotFound("Dentist");

			var service = await _db.Services
				.Where(s => !s.Deleted)
				.Select(s => new ServiceResponse
				{
					Id = s.Id,
					Name = s.Name,
					Price = s.Price,
					AppointmentsCount = _db.Appointments.Where(a => a.ServiceId == s.Id).Count(),
					Linked = s.Dentists.Contains(dentist),
					DentistsCount = s.Dentists.Count,
					CanEdit = !s.Dentists.Any(d => d.UserId != userId)
				})
				.FirstOrDefaultAsync(s => s.Id == id);

			if (service == null) return NotFound();
			return service;
		}

		[HttpPost("Services")]
		public async Task<ActionResult<Service>> CreateService(Service service)
		{
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			await _db.Services.AddAsync(service);
			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(GetService), new { service.Id }, null);
		}

		[HttpPut("Services")]
		public async Task<ActionResult<Service>> UpdateService(Service newService)
		{
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			var dentist = await _db.Dentists.Include(d => d.Services).FirstOrDefaultAsync(d => d.UserId == userId);
			if (dentist == null) return NotFound("Dentist");

			var service = await _db.Services
				.FirstOrDefaultAsync(s => s.Id == newService.Id);
			if (service == null) return NotFound("Service");

			if (service.Dentists.Any(d => d.UserId != userId)) return Forbid("Cannot edit service with linked other dentists");

			service.Name = newService.Name;
			service.Price = newService.Price;
			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(GetService), new { service.Id }, null);
		}

		[HttpDelete("Services/{id}")]
		public async Task<ActionResult> DeleteService(int id)
		{
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			var dentist = await _db.Dentists.Include(d => d.Services).FirstOrDefaultAsync(d => d.UserId == userId);
			if (dentist == null) return NotFound("Dentist");

			var service = await _db.Services
				.Where(s => !s.Deleted)
				.FirstOrDefaultAsync(s => s.Id == id);
			if (service == null) return NotFound("Service");

			if (service.Dentists.Any(d => d.UserId != userId)) return Forbid("Cannot remove service with linked other dentists");

			service.Deleted = true;
			if (dentist.Services.Contains(service)) dentist.Services.Remove(service);
			await _db.SaveChangesAsync();

			return Ok();
		}

		[HttpPut("Services/{id}/{link}")]
		public async Task<ActionResult<ServiceResponse>> GetService(int id, string link)
		{
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			var service = await _db.Services.FindAsync(id);
			if (service == null) return NotFound("Service");

			var dentist = await _db.Dentists.Include(d => d.Services).FirstOrDefaultAsync(d => d.UserId == userId);
			if (dentist == null) return NotFound("Dentist");

			switch (link)
			{
					case "Link":
						if (!dentist.Services.Contains(service)) dentist.Services.Add(service);
						else return BadRequest("Service is already linked");
						break;
					case "Unlink":
						if (dentist.Services.Contains(service)) dentist.Services.Remove(service);
						else return BadRequest("Service is not linked");
						break;
					default:
						return BadRequest("Unknown operation, possible are: Link, Unlink");
			}

			await _db.SaveChangesAsync();
			return Ok(id);
		}

		private int? GetUserId()
		{
			var isDentist = HttpContext.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == UserRole.DENTIST);
			if (!isDentist) return null;
			return int.Parse(HttpContext.User.Identity.Name);
		}
	}
}
