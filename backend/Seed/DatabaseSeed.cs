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
			modelBuilder.Entity<User>().HasData(
				new User {
					Id = 1,
					Email = "dentist1@test.pl",
					Password = "qwe",
					FirstName = "Jan",
					LastName = "Kowalski",
					Role = UserRole.DENTIST,
				},
				new User {
					Id = 2,
					Email = "dentist2@test.pl",
					Password = "qwe",
					FirstName = "Marcin",
					LastName = "Nowak",
					Role = UserRole.DENTIST,
				},
				new User {
					Id = 3,
					Email = "patient1@test.pl",
					Password = "qwe",
					FirstName = "Janusz",
					LastName = "Nosacz",
					Role = UserRole.PATIENT,
				},
				new User {
					Id = 4,
					Email = "patient2@test.pl",
					Password = "qwe",
					FirstName = "Zbigniew",
					LastName = "Stonoga",
					Role = UserRole.PATIENT,
				},
				new User {
					Id = 5,
					Email = "patient3@test.pl",
					Password = "qwe",
					FirstName = "Karol",
					LastName = "Wojtyła",
					Role = UserRole.PATIENT,
				}
			);

			modelBuilder.Entity<Dentist>().HasData(
				new Dentist {
					Id = 1,
					UserId = 1,
				},
				new Dentist
				{
					Id = 2,
					UserId = 2,
				}
			);

			modelBuilder.Entity<Patient>().HasData(
				new Patient {
					Id = 1,
					Pesel = "01234567891",
					BirthDate = DateTime.Now.AddYears(-40),
					UserId = 3,
					DentistId = 1
				},
				new Patient {
					Id = 2,
					Pesel = "12345678901",
					BirthDate = DateTime.Now.AddYears(-30),
					UserId = 4,
					DentistId = 1
				},
				new Patient {
					Id = 3,
					Pesel = "23456789012",
					BirthDate = DateTime.Now.AddYears(-60),
					UserId = 5,
					DentistId = null
				}
			);

			var now = DateTime.Now;
			modelBuilder.Entity<Appointment>().HasData(
				new Appointment {
					Id = 1,
					StartDate = now.AddDays(-2),
					EndDate = now.AddDays(-2).AddHours(1.5),
					Canceled = false,
					DentistId = 1,
					PatientId = 1,
					ServiceId = 1,
				},
				new Appointment {
					Id = 2,
					StartDate = now.AddDays(-1),
					EndDate = now.AddDays(-1).AddHours(1),
					Canceled = true,
					DentistId = 1,
					PatientId = 1,
					ServiceId = 2,
				},
				new Appointment {
					Id = 3,
					StartDate = now,
					EndDate = now.AddHours(1),
					Canceled = false,
					DentistId = 1,
					PatientId = 1,
					ServiceId = 1,
				},
				new Appointment {
					Id = 4,
					StartDate = now.AddHours(1.5),
					EndDate = now.AddHours(2),
					Canceled = false,
					DentistId = 1,
					PatientId = 1,
					ServiceId = 2,
				},
				new Appointment {
					Id = 5,
					StartDate = now.AddDays(1),
					EndDate = now.AddDays(1).AddHours(2.5),
					Canceled = false,
					DentistId = 1,
					PatientId = 1,
					ServiceId = 3,
				},
				new Appointment {
					Id = 6,
					StartDate = now.AddDays(2),
					EndDate = now.AddDays(2).AddHours(2.25),
					Canceled = true,
					DentistId = 1,
					PatientId = 1,
					ServiceId = 4,
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
				}
			);
		}
	}
}