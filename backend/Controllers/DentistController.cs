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
			return await _db.Patients.Select(p => new PatientResult {
				Id = p.Id,
				FirstName = p.FirstName,
				LastName = p.LastName,
				Email = p.User.Email,
			}).ToListAsync();
		}

		[HttpGet("Patient/{id}")]
		public async Task<ActionResult<Patient>> GetPatient(int patientId)
		{
			var patient = await _db.Patients.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == patientId);
			if (patient == null) return NotFound();
			return patient;
		}

		[HttpGet("Appointments")]
		public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
		{
			var appointments = await _db.Appointments
				//.Where(a => a.DoctorId == ) // TODO: Add filtering
				.ToListAsync();
			return appointments;
		}

		[HttpGet("Appointment/{id}")]
		public async Task<ActionResult<Appointment>> GetAppointment(int appointmentId)
		{
			var appointment = await _db.Appointments
				//.Where(a => a.DoctorId == ) // TODO: Add filtering
				.FindAsync(appointmentId);
			if (appointment == null) return NotFound();
			return appointment;
		}

		[HttpPost("Appointment")]
		public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment)
		{
			await _db.Appointments.AddAsync(appointment);
			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(PostAppointment), new { appointment.Id }, appointment);
		}

		[HttpPut("Appointment")]
		public async Task<ActionResult<Appointment>> PutAppointment(Appointment newAppointment)
		{
			var appointment = await _db.Appointments.FindAsync(newAppointment.Id);
			if (appointment == null) return NotFound();
			appointment.DateTime = newAppointment.DateTime;
			appointment.Duration = newAppointment.Duration;
			appointment.DoctorId = appointment.DoctorId;
			appointment.PatientId = appointment.PatientId;
			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(PutAppointment), new { appointment.Id }, appointment);
		}

		[HttpDelete("Appointment/{id}")]
		public async Task<ActionResult> DeleteAppointment(int appointmentId)
		{
			var appointment = await _db.Appointments
				//.Where(a => a.DoctorId == ) // TODO: Add filtering
				.FindAsync(appointmentId);
			if (appointment == null) return NotFound();
			_db.Appointments.Remove(appointment);
			return Ok();
		}
	}
}
