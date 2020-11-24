using System;
using System.Collections.Generic;

namespace ToothSoupAPI.Models {
	public class Appointment {
		public int Id { get; set; }
		public DateTime DateTime { get; set; }
		public TimeSpan Duration { get; set; }
		public bool Canceled { get; set; }
		public Dentist Dentist { get; set; }
		public int DentistId { get; set; }
		public Patient Patient { get; set; }
		public int PatientId { get; set; }
	}
}