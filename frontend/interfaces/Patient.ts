import User from "./User";

export default interface Patient extends User {
	pesel: string;
	firstName: string;
	lastName: string;
	email: string;
	birthDate: string;
	dentistName?: string;
	dentistId?: number;
}