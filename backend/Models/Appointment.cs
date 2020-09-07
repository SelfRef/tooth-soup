using System;
using System.Collections.Generic;

namespace ToothSoupAPI.Models {
	public class Appointment {
		public int AppointmentId { get; set; }
		public DateTime DateTime { get; set; }
		public TimeSpan Duration { get; set; }
		public Dentist Doctor { get; set; }
		public Patient Patient { get; set; }
	}
}