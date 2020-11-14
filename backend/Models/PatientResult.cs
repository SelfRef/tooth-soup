namespace ToothSoupAPI.Models {
	public class PatientResult {
		public int Id { get; set; }
		public string Pesel { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public User User { get; set; }
		public int UserId { get; set; }
		public int DentistId { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
	}
}