using Microsoft.EntityFrameworkCore;
using ToothSoupAPI.Models;

namespace ToothSoupAPI {
	public class Database : DbContext {
		public DbSet<Dentist> Dentists { get; set; }
		public DbSet<Patient> Patients { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Appointment> Appointments { get; set; }
	}
}