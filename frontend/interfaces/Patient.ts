import User from "./User";

export default interface Patient extends User {
	pesel: string;
	firstName: string;
	lastName: string;
	birthDate: Date | null;
}