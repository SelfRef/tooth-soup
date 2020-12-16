declare global {
	interface Date {
		toLocalISO(): string;
		toDateString(): string;
		toTimeString(): string;
		toDateTimeString(): string;
	}

	interface Number {
		zeroPad(digits: number): string;
	}
}

Date.prototype.toLocalISO = function() {
	const offsetMs = this.getTimezoneOffset() * 60 * 1000;
	const msLocal = this.getTime() - offsetMs;
	const dateLocal = new Date(msLocal);
	const iso = dateLocal.toISOString();
	const isoLocal = iso.slice(0, 19);
	return isoLocal;
}

Date.prototype.toDateString = function() {
	return `${this.getFullYear()}-${(this.getMonth() + 1).zeroPad(2)}-${this.getDate().zeroPad(2)}`
}

Date.prototype.toTimeString = function() {
	return `${this.getHours().zeroPad(2)}:${this.getMinutes().zeroPad(2)}`
}

Date.prototype.toDateTimeString = function() {
	return `${this.toDateString()} ${this.toTimeString()}`
}

Number.prototype.zeroPad = function(digits) {
	let numberStr = String(this);
	const numberLen = numberStr.length;
	if (numberLen < digits) {
		for (let i = 0; i < digits - numberLen; i++) {
			numberStr = `0${numberStr}`;
		}
	}
	return numberStr;
}

export {};