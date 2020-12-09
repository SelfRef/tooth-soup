using Microsoft.AspNetCore.Identity;

namespace ToothSoupAPI.Models {
	public class UserContract : User {
		public PatientResult Patient { get; set; }
		public DentistResult Dentist { get; set; }
	}
}