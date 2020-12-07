using Microsoft.AspNetCore.Identity;

namespace ToothSoupAPI.Models {
	public class UserUpdate {
		public string Password { get; set; }
		public string Email { get; set; }
		public int? DentistId { get; set; }
	}
}