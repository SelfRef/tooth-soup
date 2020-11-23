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
					UserId = 3,
					DentistId = 1
				},
				new Patient {
					Id = 2,
					UserId = 4,
					DentistId = 1
				}
			);
		}
	}
}