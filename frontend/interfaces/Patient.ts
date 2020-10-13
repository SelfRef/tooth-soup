import User from "./User";

export default interface Patient extends User {
	id: number,
	firstName: string,
	lastName: string,
	user: User
}