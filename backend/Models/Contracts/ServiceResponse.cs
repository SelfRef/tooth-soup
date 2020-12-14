namespace ToothSoupAPI.Models {
	public class ServiceResponse {
		public int Id { get; set; }
		public string Name { get; set; }
		public float Price { get; set; }
		public int AppointmentsCount { get; set; }
		public int DentistsCount { get; set; } = 0;
		public bool CanEdit { get; set; }
		public bool Linked { get; set; }
	}
}