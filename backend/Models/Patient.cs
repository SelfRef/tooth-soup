using System;

namespace ToothSoupAPI.Models {
	public class Patient {
		public int Id { get; set; }
		public string Pesel { get; set; }
		public DateTime BirthDate { get; set; }
		public User User { get; set; }
		public int UserId { get; set; }
		public int DentistId { get; set; }
	}
}