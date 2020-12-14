using System;
using System.Collections.Generic;

namespace ToothSoupAPI.Models {
	public class PatientAppointmentList {
		public List<AppointmentPatientResponse> Appointments { get; set; }
	}
}