using System;
using System.Collections.Generic;

namespace ToothSoupAPI.Models {
	public class AppointmentRequest {
		public int? Id { get; set; }
		public DateTime? DateTime { get; set; }
		public TimeSpan? Duration { get; set; }
		public bool? Canceled { get; set; }
		public int? PatientId { get; set; }
	}
}