using System;

namespace ToothSoupAPI.Models {
	public class DentistResult {
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Name => $"{FirstName} {LastName}";
		public bool CanLink { get; set; }
		public bool CanCreateAppointment { get; set; }
	}
}