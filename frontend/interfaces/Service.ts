export default interface Service {
	id?: number;
	name?: string | null;
	price?: number | null;
	appointmentsCount?: number | null;
	dentistsCount?: number | null;
	canEdit?: boolean | null;
	linked?: boolean | null;
}