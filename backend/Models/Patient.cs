namespace ToothSoupAPI.Models {
	public class Patient {
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public User User { get; set; }
		public int UserId { get; set; }
		public int DentistId { get; set; }
	}
}