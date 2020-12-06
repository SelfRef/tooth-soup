using System;
using System.Collections.Generic;

namespace ToothSoupAPI.Models {
	public class Appointment {
		public int Id { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public TimeSpan Duration => EndDate - StartDate;
		public bool Canceled { get; set; }
		public Dentist Dentist { get; set; }
		public int DentistId { get; set; }
		public Patient Patient { get; set; }
		public int PatientId { get; set; }
		public Service Service { get; set; }
		public int ServiceId { get; set; }
	}
}