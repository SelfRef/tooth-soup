using System;
using System.Collections.Generic;

namespace ToothSoupAPI.Models {
	public class DentistResult {
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Name => $"{FirstName} {LastName}";
		public List<Service> Services { get; set; }
		public bool CanLink { get; set; }
		public bool CanCreateAppointment { get; set; }
	}
}