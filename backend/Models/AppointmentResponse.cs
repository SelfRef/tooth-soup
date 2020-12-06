using System;
using System.Collections.Generic;

namespace ToothSoupAPI.Models {
	public class AppointmentResponse {
		public int Id { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public TimeSpan Duration { get; set; }
		public bool Canceled { get; set; }
		public int PatientId { get; set; }
		public string PatientName { get; set; }
		public int ServiceId { get; set; }
		public string ServiceName { get; set; }
	}
}