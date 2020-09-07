namespace ToothSoupAPI.Models {
	public class Patient {
		public int PatientId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public User User { get; set; }
	}
}