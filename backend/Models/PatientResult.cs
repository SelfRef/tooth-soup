namespace ToothSoupAPI.Models {
	public class PatientResult {
		public int Id { get; set; }
		public string Pesel { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public int DentistId { get; set; }
	}
}