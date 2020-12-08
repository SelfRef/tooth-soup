﻿using System.Reflection.Metadata;
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
	[Authorize(Roles = UserRole.ADMIN)]
	public class AdminController : ControllerBase
	{
		private readonly Database _db;

		public AdminController(Database database)
		{
			_db = database;
		}

		[HttpGet("Users")]
		public async Task<ActionResult<IEnumerable<UserContract>>> GetUsers() {
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			var users = await _db.Users.Select(u => new UserContract {
				Id = u.Id,
				Email = u.Email,
				FirstName = u.FirstName,
				LastName = u.LastName,
				Role = u.Role,
			}).ToListAsync();

			foreach (var user in users)
			{
				switch (user.Role)
				{
					case UserRole.PATIENT:
						user.Patient = await _db.Patients
							.Where(p => p.UserId == user.Id)
							.Include(p => p.Dentist).ThenInclude(d => d.User)
							.Select(p => new PatientResult {
								Id = p.Id,
								Pesel = p.Pesel,
								FirstName = user.FirstName,
								LastName = user.LastName,
								Email = user.Email,
								BirthDate = p.BirthDate,
								DentistId = p.DentistId,
								DentistName = GetDentistName(p.Dentist)
							})
							.FirstOrDefaultAsync();
						break;
					case UserRole.DENTIST:
						user.Dentist = await _db.Dentists
							.Where(d => d.UserId == user.Id)
							.Select(d => new DentistResult {
								Id = d.Id,
								FirstName = user.FirstName,
								LastName = user.LastName,
								Email = user.Email,
								CanLink = d.CanLink,
								CanCreateAppointment = d.CanCreateAppointment
							})
							.FirstOrDefaultAsync();
						break;
				}
			}

			return users;
		}

		[HttpGet("User/{id}")]
		public async Task<ActionResult<UserContract>> GetUser(int id) {
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			var user = await _db.Users.FindAsync(id);
			if (user == null) return NotFound("User");
			
			var userResult = new UserContract {
				Id = user.Id,
				Email = user.Email,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Role = user.Role,
			};

			switch (user.Role)
			{
			case UserRole.PATIENT:
				userResult.Patient = await _db.Patients
					.Where(p => p.UserId == user.Id)
					.Include(p => p.Dentist).ThenInclude(d => d.User)
					.Select(p => new PatientResult {
						Id = p.Id,
						Pesel = p.Pesel,
						FirstName = user.FirstName,
						LastName = user.LastName,
						Email = user.Email,
						BirthDate = p.BirthDate,
						DentistId = p.DentistId,
						DentistName = GetDentistName(p.Dentist)
					})
					.FirstOrDefaultAsync();
				break;
			case UserRole.DENTIST:
				userResult.Dentist = await _db.Dentists
					.Where(d => d.UserId == user.Id)
					.Select(d => new DentistResult {
						Id = d.Id,
						FirstName = user.FirstName,
						LastName = user.LastName,
						Email = user.Email,
						CanLink = d.CanLink,
						CanCreateAppointment = d.CanCreateAppointment
					})
					.FirstOrDefaultAsync();
				break;
			}

			return userResult;
		}

		[HttpPost("User")]
		public async Task<ActionResult<UserContract>> CreateUser(UserContract newUser) {
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			var emailUsed = _db.Users.Where(u => u.Email == newUser.Email).Any();
			if (emailUsed) return BadRequest("Email already in use");
			
			var user = new User {
				Email = newUser.Email,
				FirstName = newUser.FirstName,
				LastName = newUser.LastName,
				Role = newUser.Role,
			};
			await _db.Users.AddAsync(user);

			switch (user.Role)
			{
				case UserRole.PATIENT:
					await _db.Patients.AddAsync(new Patient {
						Pesel = newUser.Patient.Pesel,
						BirthDate = newUser.Patient.BirthDate,
						UserId = user.Id,
						DentistId = newUser.Patient.DentistId
					});
					break;
				case UserRole.DENTIST:
					await _db.Dentists.AddAsync(new Dentist {
						UserId = user.Id,
						CanLink = newUser.Dentist.CanLink,
						CanCreateAppointment = newUser.Dentist.CanCreateAppointment
					});
					break;
			}

			await _db.SaveChangesAsync();
			return CreatedAtAction(nameof(GetUser), new { user.Id }, GetUser(user.Id).Result.Value);
		}

		[HttpPut("User")]
		public async Task<ActionResult<UserContract>> UpdateUser(UserContract newUser) {
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			var user = await _db.Users.FindAsync(newUser.Id);
			if (user == null) return NotFound("User");

			var emailUsed = _db.Users.Where(u => u.Id != user.Id && u.Email == newUser.Email).Any();
			if (emailUsed) return BadRequest("Email already in use");

			if (user.Role != newUser.Role) return BadRequest("You cannot change user role");
			
			user.Email = newUser.Email;
			user.FirstName = newUser.FirstName;
			user.LastName = newUser.LastName;
			if (user.Password != null) user.Password = newUser.Password;

			switch (user.Role)
			{
				case UserRole.PATIENT:
					var patient = await _db.Patients.Where(p => p.UserId == user.Id).FirstOrDefaultAsync();
					patient.Pesel = newUser.Patient.Pesel;
					patient.BirthDate = newUser.Patient.BirthDate;
					patient.DentistId = newUser.Patient.DentistId;
					break;
				case UserRole.DENTIST:
					var dentist = await _db.Dentists.Where(p => p.UserId == user.Id).FirstOrDefaultAsync();
					dentist.CanLink = newUser.Dentist.CanLink;
					dentist.CanCreateAppointment = newUser.Dentist.CanCreateAppointment;
					break;
			}

			await _db.SaveChangesAsync();
			return CreatedAtAction(nameof(GetUser), new { user.Id }, GetUser(user.Id).Result.Value);
		}

		[HttpDelete("User/{id}")]
		public async Task<ActionResult> UpdateUser(int id) {
			var userId = GetUserId();
			if (!userId.HasValue) return Unauthorized();

			var user = await _db.Users.FindAsync(id);
			if (user == null) return NotFound("User");
			
			_db.Users.Remove(user);

			switch (user.Role)
			{
				case UserRole.PATIENT:
					var patient = await _db.Patients.Where(p => p.UserId == id).FirstOrDefaultAsync();
					if (patient != null) _db.Patients.Remove(patient);
					break;
				case UserRole.DENTIST:
					var dentist = await _db.Dentists.Where(p => p.UserId == id).FirstOrDefaultAsync();
					if (dentist != null) _db.Dentists.Remove(dentist);
					break;
			}

			await _db.SaveChangesAsync();
			return Ok(id);
		}

		private int? GetUserId()
		{
			var isAdmin = HttpContext.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == UserRole.ADMIN);
			if (!isAdmin) return null;
			return int.Parse(HttpContext.User.Identity.Name);
		}

		private static string GetDentistName(Dentist dentist) {
			if (dentist == null || dentist.User == null) return null;
			return $"{dentist.User.FirstName} {dentist.User.LastName}";
		}
	}
}
