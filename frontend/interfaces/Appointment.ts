export default interface Appointment {
	id?: number;
	dateTime: Date | null;
	duration: Date | null;
	canceled: boolean | null;
	dentistId: number | null;
	patientId: number | null;
}