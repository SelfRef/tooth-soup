import Appointment from "~/interfaces/Appointment";
import Dentist from "~/interfaces/Dentist";
import Patient from "~/interfaces/Patient";
import jsPDF, { AcroFormComboBox } from "jspdf";
import 'jspdf-autotable'
import '~/assets/Roboto-Regular-normal';
import { js2xml } from 'xml-js';
import js2xmlparser from 'js2xmlparser';

type State = {
	account: Patient | null,
	appointments: Appointment[],
	dentists: Dentist[]
}

export const state = (): State => ({
	account: null,
	appointments: [],
	dentists: [],
})

export const getters = {
	account(state: State) {
		return state.account;
	},
	appointments(state: State) {
		return state.appointments;
	},
	dentists(state: State) {
		return state.dentists;
	},
};

export const mutations = {
	setAccount(state: State, data: Patient) {
		state.account = data;
	},
	setAppointments(state: State, data: Appointment[]) {
		state.appointments = data;
	},
	setDentists(state: State, data: Dentist[]) {
		state.dentists = data;
	},
}

export const actions = {
	async pullAccount({commit, rootGetters}) {
		if (!rootGetters['Auth/isLoggedIn']) return false;
		const initData: RequestInit = {
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${rootGetters['Auth/token']}`,
			}
		}
		const data = await fetch(`${process.env.APIURL}/Patient/Me`, initData).then(response => response.json());
		commit('setAccount', data);
	},
	async updateAppointments({commit, rootGetters}) {
		if (!rootGetters['Auth/isLoggedIn']) return false;
		const initData: RequestInit = {
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${rootGetters['Auth/token']}`,
			}
		}
		const data = await fetch(`${process.env.APIURL}/Patient/Appointments`, initData).then(response => response.json());
		commit('setAppointments', data);
	},
	async updateDentists({commit, rootGetters}) {
		if (!rootGetters['Auth/isLoggedIn']) return false;
		const initData: RequestInit = {
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${rootGetters['Auth/token']}`,
			}
		}
		const data = await fetch(`${process.env.APIURL}/Patient/Dentists`, initData).then(response => response.json());
		commit('setDentists', data);
	},
	async pushPatient({commit, rootGetters}, newData) {
		if (!rootGetters['Auth/isLoggedIn']) return false;
		const initData: RequestInit = {
			method: 'PUT',
			headers: {
				'Authorization': `Bearer ${rootGetters['Auth/token']}`,
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(newData)
		}
		const data = await fetch(`${process.env.APIURL}/Patient/Me`, initData).then(response => response.json());
		commit('setAccount', data);
	},

	async exportAppointmentsPdf({getters, rootGetters}, {appointment}) {
		if (!rootGetters['Auth/isLoggedIn']) return false;

		const inputData = appointment ? [appointment] : getters.appointments;
		let data = [];
		inputData.forEach(appointment => {
			data.push({...appointment});
		});
		data = data.map(a => ([
			a.startDate.substr(0, 10),
			a.startDate.substr(11, 5),
			`${a.duration.substr(0, 5).replace(':', 'h ')}m`,
			a.dentistName,
			a.serviceName,
			a.canceled ? 'Yes' : 'No',
			`${String(a.canceled ? 0 : a.servicePrice)} zł`
		]));

		const headers = [[
			'Date',
			'Time',
			'Duration',
			'Dentist Name',
			'Service Name',
			'Canceled',
			'Cost',
		]];
		
		const doc = new jsPDF();
		doc.setFont('Roboto-Regular');
		doc.setFontSize(16);
		doc.text('Patient info', 90, 20);

		doc.setFontSize(10);
		doc.text('Name', 20, 40);
		doc.text('Email', 20, 60);
		doc.text('Linked dentist', 100, 40);
		doc.text('Appointment Count', 100, 60);

		const patient = getters.account as Patient;
		doc.setFontSize(14);
		doc.text(`${patient.firstName} ${patient.lastName}`, 20, 48);
		doc.text(patient.email, 20, 68);
		doc.text(patient.dentistName, 100, 48);
		doc.text(String(getters.appointments.length), 100, 68);

		doc.setFontSize(16);
		doc.text(appointment ? 'Invoice' : 'Appointments', 90, 90);

		doc.autoTable({
			styles: {
				font: 'Roboto-Regular'
			},
			margin: {
				top: 100
			},
			head: headers,
			body: data,
		});
		
		doc.save(appointment ? 'invoice.pdf' : 'export.pdf');
	},

	async exportAppointmentsXml({getters, rootGetters}, {}) {
		if (!rootGetters['Auth/isLoggedIn']) return false;

		let initData: RequestInit = {
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${rootGetters['Auth/token']}`,
			}
		}
		const data = await fetch(`${process.env.APIURL}/Patient/Appointments/Xml`, initData).then(response => response.text());
		const file = new Blob([data], {type: 'application/xml'});
		const name = 'export.xml';
		if (window.navigator.msSaveOrOpenBlob) { // IE10+
			window.navigator.msSaveOrOpenBlob(file, name);
		} else {
			const a = document.createElement("a"),
			url = URL.createObjectURL(file);
			a.href = url;
			a.download = name;
			document.body.appendChild(a);
			a.click();
			setTimeout(function() {
				document.body.removeChild(a);
				window.URL.revokeObjectURL(url);  
			}, 0); 
		}
	}
}