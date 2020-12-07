using System.Collections.Generic;

namespace ToothSoupAPI.Models {
	public class Dentist {
		public int Id { get; set; }
		public List<Patient> Patients { get; set; }
		public User User { get; set; }
		public int UserId { get; set; }

		/// <summary>Prevent patient from linking this dentist in account page</summary>
		public bool PreventPatientLinking { get; set; }

		/// <summary>Prevent unlinked patient from creating appointment</summary>
		public bool PreventUnlinkedPatient { get; set; }
	}
}