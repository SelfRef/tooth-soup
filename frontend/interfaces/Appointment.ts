export default interface Appointment {
	id?: number;
	dateTime: Date | null;
	duration: string | null;
	canceled: boolean | null;
	dentistId: number | null;
	patientId: number | null;
	serviceId: number | null;
}