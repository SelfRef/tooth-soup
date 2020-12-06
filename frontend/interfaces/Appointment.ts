export default interface Appointment {
	id?: number;
	startDate: string | null;
	endDate: string | null;
	duration: string | null;
	canceled: boolean | null;
	dentistId: number | null;
	patientId: number | null;
	patientName: string | null;
	serviceId: number | null;
	serviceName: string | null;
}