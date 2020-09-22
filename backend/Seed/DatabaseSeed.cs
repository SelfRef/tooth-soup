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
					Password = "qwerty"
				},
				new User {
					Id = 2,
					Email = "dentist2@test.pl",
					Password = "qwerty"
				},
				new User {
					Id = 3,
					Email = "patient1@test.pl",
					Password = "qwerty"
				},
				new User {
					Id = 4,
					Email = "patient2@test.pl",
					Password = "qwerty"
				}
			);

			modelBuilder.Entity<Dentist>().HasData(
				new Dentist {
					Id = 1,
					FirstName = "Jan",
					LastName = "Kowalski",
					UserId = 1,
				}
			);

			modelBuilder.Entity<Patient>().HasData(
				new Patient {
					Id = 1,
					FirstName = "Janusz",
					LastName = "Nosacz",
					UserId = 3,
					DentistId = 1
				},
				new Patient {
					Id = 2,
					FirstName = "Zbigniew",
					LastName = "Stonoga",
					UserId = 4,
					DentistId = 1
				}
			);
		}
	}
}