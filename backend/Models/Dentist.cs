using System.Collections.Generic;

namespace ToothSoupAPI.Models {
	public class Dentist {
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public List<Patient> Patients { get; set; }
		public User User { get; set; }
		public int UserId { get; set; }
	}
}