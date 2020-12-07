export default interface Dentist {
	id: number;
	firstName: string;
	lastName: string;
	name: string;
	email?: string;
	password?: string;
	canLink?: boolean;
	canCreateAppointment?: boolean;
}