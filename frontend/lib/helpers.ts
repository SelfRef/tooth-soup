import '~/lib/extensions';

export function peselToDateString(pesel: string): string {
	let year = Number(pesel.substr(0, 2));
	let month = Number(pesel.substr(2, 2));
	const day = Number(pesel.substr(4, 2));

	if (month > 80 && month < 93) {
		month -= 80;
		year += 1800; // already dead
	} else if (month > 60 && month < 73) {
		month -= 60;
		year += 2200; // collapse of society
	} else if (month > 40 && month < 53) {
		month -= 40;
		year += 2100; // flying cars?
	} else if (month > 20 && month < 33) {
		month -= 20;
		year += 2000; // zoomers
	} else if (month > 0 && month < 13) {
		year += 1900; // boomers
	} else {
		return '';
	}

	return `${year}-${month.zeroPad(2)}-${day.zeroPad(2)}`;
}