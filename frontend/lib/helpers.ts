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

export const rules = {
	required: (v: string): boolean | string => Boolean(v) || 'Required',
	number: (v: string): boolean | string => /^\d+$/.test(v) || 'Must be a number',
	pesel: (v: string): boolean | string => {
		if (!v) return false;
		if (v.length !== 11) return 'Must be 11 digits long';
		if (!/^\d+$/.test(v)) return 'Must have only digits';
		return true;
	},
	email: (v: string): boolean | string => {
		const pattern = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
		return pattern.test(v) || 'Invalid e-mail'
	},
}