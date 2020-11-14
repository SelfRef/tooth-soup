using System.Collections.Generic;
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
					Role = UserRole.DENTIST,
					FirstName = "Jan",
					LastName = "Kowalski",
				},
				new User {
					Id = 2,
					Email = "dentist2@test.pl",
					Password = "qwe",
					Role = UserRole.DENTIST,
					FirstName = "Marcin",
					LastName = "Nowak",
				},
				new User {
					Id = 3,
					Email = "patient1@test.pl",
					Password = "qwe",
					Role = UserRole.PATIENT,
					FirstName = "Janusz",
					LastName = "Nosacz",
				},
				new User {
					Id = 4,
					Email = "patient2@test.pl",
					Password = "qwe",
					Role = UserRole.PATIENT,
					FirstName = "Zbigniew",
					LastName = "Stonoga",
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