using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToothSoupAPI.Models;
using ToothSoupAPI.Seed;

namespace ToothSoupAPI {
	public class Database : DbContext {
		public Database(DbContextOptions<Database> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			SeedData.Seed(modelBuilder);
		}

		public DbSet<User> Users { get; set; }
		public DbSet<Dentist> Dentists { get; set; }
		public DbSet<Patient> Patients { get; set; }
		public DbSet<Appointment> Appointments { get; set; }
		public DbSet<Service> Services { get; set; }
	}
}