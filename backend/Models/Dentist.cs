using System.Collections.Generic;

namespace ToothSoupAPI.Models {
	public class Dentist {
		public int Id { get; set; }
		public List<Patient> Patients { get; set; }
		public User User { get; set; }
		public int UserId { get; set; }
		public ICollection<Service> Services { get; set; }

		/// <summary>Allow patient to link this dentist in account page</summary>
		public bool CanLink { get; set; }

		/// <summary>Allow unlinked patient for creating appointment</summary>
		public bool CanCreateAppointment { get; set; }
	}
}