using System;

namespace ToothSoupAPI.Models {
	public class DentistUpdate {
		public string Email { get; set; }
		public string Password { get; set; }
		public bool? CanLink { get; set; }
		public bool? CanCreateAppointment { get; set; }
	}
}