using System;
using System.Collections.Generic;

namespace ToothSoupAPI.Models {
	public class AppointmentRequest {
		public int? Id { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public bool? Canceled { get; set; }
		public int? PatientId { get; set; }
		public int? ServiceId { get; set; }
	}
}