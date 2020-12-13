using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToothSoupAPI.Models;

namespace ToothSoupAPI.Seed
{
	public static class SeedData
	{
		public static void Seed(ModelBuilder modelBuilder)
		{
			var today = DateTime.Now.Date;

			modelBuilder.Entity<User>().HasData(
				new User {
					Id = 1,
					Email = "dentist1@qwe.pl",
					Password = "qwe",
					FirstName = "Jan",
					LastName = "Kowalski",
					Role = UserRole.DENTIST,
				},
				new User {
					Id = 2,
					Email = "dentist2@qwe.pl",
					Password = "qwe",
					FirstName = "Marcin",
					LastName = "Nowak",
					Role = UserRole.DENTIST,
				},
				new User {
					Id = 3,
					Email = "patient1@qwe.pl",
					Password = "qwe",
					FirstName = "Janusz",
					LastName = "Nosacz",
					Role = UserRole.PATIENT,
				},
				new User {
					Id = 4,
					Email = "patient2@qwe.pl",
					Password = "qwe",
					FirstName = "Zbigniew",
					LastName = "Stonoga",
					Role = UserRole.PATIENT,
				},
				new User {
					Id = 5,
					Email = "patient3@qwe.pl",
					Password = "qwe",
					FirstName = "Karol",
					LastName = "Wojtyła",
					Role = UserRole.PATIENT,
				},
				new User {
					Id = 6,
					Email = "admin1@qwe.pl",
					Password = "qwe",
					FirstName = "Jacek",
					LastName = "Sasin",
					Role = UserRole.ADMIN,
				}
			);

			modelBuilder.Entity<Dentist>().HasData(
				new Dentist {
					Id = 1,
					UserId = 1,
					CanLink = true,
					CanCreateAppointment = false
				},
				new Dentist
				{
					Id = 2,
					UserId = 2,
					CanLink = true,
					CanCreateAppointment = true
				}
			);

			modelBuilder.Entity<Patient>().HasData(
				new Patient {
					Id = 1,
					Pesel = "01234567891",
					BirthDate = today.AddYears(-40),
					UserId = 3,
					DentistId = 1
				},
				new Patient {
					Id = 2,
					Pesel = "98765432109",
					BirthDate = today.AddYears(-30),
					UserId = 4,
					DentistId = 1
				},
				new Patient {
					Id = 3,
					Pesel = "23456789012",
					BirthDate = today.AddYears(-60),
					UserId = 5,
					DentistId = null
				}
			);

			
			modelBuilder.Entity<Appointment>().HasData(
				new Appointment {
					Id = 1,
					StartDate = today.AddDays(-1).AddHours(10),
					EndDate = today.AddDays(-1).AddHours(11),
					Canceled = false,
					DentistId = 1,
					PatientId = 1,
					ServiceId = 1,
				},
				new Appointment {
					Id = 2,
					StartDate = today.AddHours(9),
					EndDate = today.AddHours(10.5),
					Canceled = true,
					DentistId = 1,
					PatientId = 1,
					ServiceId = 2,
				},
				new Appointment {
					Id = 3,
					StartDate = today.AddHours(11),
					EndDate = today.AddHours(12),
					Canceled = false,
					DentistId = 1,
					PatientId = 1,
					ServiceId = 3,
				},
				new Appointment {
					Id = 4,
					StartDate = today.AddHours(14),
					EndDate = today.AddHours(16.5),
					Canceled = false,
					DentistId = 1,
					PatientId = 1,
					ServiceId = 4,
				},
				new Appointment {
					Id = 5,
					StartDate = today.AddDays(1).AddHours(7),
					EndDate = today.AddDays(1).AddHours(8.75),
					Canceled = false,
					DentistId = 1,
					PatientId = 1,
					ServiceId = 3,
				},
				new Appointment {
					Id = 6,
					StartDate = today.AddDays(2).AddHours(10),
					EndDate = today.AddDays(2).AddHours(11.25),
					Canceled = false,
					DentistId = 1,
					PatientId = 1,
					ServiceId = 4,
				},
				new Appointment {
					Id = 7,
					StartDate = today.AddHours(12),
					EndDate = today.AddHours(13.5),
					Canceled = false,
					DentistId = 1,
					PatientId = 2,
					ServiceId = 2,
				},
				new Appointment {
					Id = 8,
					StartDate = today.AddHours(17),
					EndDate = today.AddHours(17.25),
					Canceled = false,
					DentistId = 1,
					PatientId = 2,
					ServiceId = 1,
				},
				new Appointment {
					Id = 9,
					StartDate = today.AddDays(1).AddHours(15),
					EndDate = today.AddDays(1).AddHours(17),
					Canceled = false,
					DentistId = 1,
					PatientId = 2,
					ServiceId = 3,
				},
				new Appointment {
					Id = 10,
					StartDate = today.AddDays(1).AddHours(10),
					EndDate = today.AddDays(1).AddHours(12),
					Canceled = true,
					DentistId = 1,
					PatientId = 3,
					ServiceId = 1,
				},
				new Appointment {
					Id = 11,
					StartDate = today.AddDays(1).AddHours(13),
					EndDate = today.AddDays(1).AddHours(14.75),
					Canceled = false,
					DentistId = 1,
					PatientId = 2,
					ServiceId = 4,
				},
				new Appointment {
					Id = 12,
					StartDate = today.AddDays(2).AddHours(16),
					EndDate = today.AddDays(2).AddHours(18.5),
					Canceled = false,
					DentistId = 1,
					PatientId = 3,
					ServiceId = 3,
				},
				new Appointment {
					Id = 13,
					StartDate = today.AddDays(3).AddHours(6),
					EndDate = today.AddDays(3).AddHours(7.75),
					Canceled = false,
					DentistId = 1,
					PatientId = 1,
					ServiceId = 1,
				},
				new Appointment {
					Id = 14,
					StartDate = today.AddDays(3).AddHours(13),
					EndDate = today.AddDays(3).AddHours(14.25),
					Canceled = false,
					DentistId = 1,
					PatientId = 2,
					ServiceId = 2,
				}
			);

			modelBuilder.Entity<Service>().HasData(
				new Service {
					Id = 1,
					Name = "Overview",
					Price = 100
				},
				new Service {
					Id = 2,
					Name = "Tooth Extraction",
					Price = 200
				},
				new Service {
					Id = 3,
					Name = "Root Canal Treatment",
					Price = 300
				},
				new Service {
					Id = 4,
					Name = "Euthanasia",
					Price = 500
				},
				new Service {
					Id = 5,
					Name = "Sasinowanie",
					Price = 70000000
				}
			);

			// modelBuilder.Entity<DentistService>().HasData(
			// 	new DentistService {
			// 		DentistId = 1,
			// 		ServiceId = 1
			// 	},
			// 	new DentistService {
			// 		DentistId = 1,
			// 		ServiceId = 2
			// 	},
			// 	new DentistService {
			// 		DentistId = 1,
			// 		ServiceId = 3
			// 	},
			// 	new DentistService {
			// 		DentistId = 1,
			// 		ServiceId = 4
			// 	},
			// 	new DentistService {
			// 		DentistId = 2,
			// 		ServiceId = 1
			// 	},
			// 	new DentistService {
			// 		DentistId = 2,
			// 		ServiceId = 2
			// 	}
			// );
		}
	}
}