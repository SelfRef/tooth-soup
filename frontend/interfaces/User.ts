import Dentist from "./Dentist";
import Patient from "./Patient";

export default interface User {
	id?: number;
	firstName: string;
	lastName: string;
	name: string;
	email: string;
	role: string;
	password: string | null;
	dentist: Dentist | null;
	patient: Patient | null;
}