import jsPDF from "jspdf";
import Appointment from "~/interfaces/Appointment";
import Dentist from "~/interfaces/Dentist";
import Patient from "~/interfaces/Patient";
import Service from "~/interfaces/Service";

type State = {
	account: Dentist | null,
	patients: Patient[],
	services: Service[],
	appointments: Appointment[],
}

export const state = (): State => ({
	account: null,
	patients: [],
	services: [],
	appointments: [],
})

export const getters = {
	account(state: State) {
		return state.account;
	},
	patients(state: State) {
		return state.patients;
	},
	services(state: State) {
		return state.services;
	},
	appointments(state: State) {
		return state.appointments;
	},
};

export const mutations = {
	setAccount(state: State, data: Dentist) {
		state.account = data;
	},
	setPatients(state: State, data: Patient[]) {
		state.patients = data;
	},
	setServices(state: State, data: Service[]) {
		state.services = data;
	},
	setAppointments(state: State, data: Appointment[]) {
		state.appointments = data;
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
		const data = await fetch(`${process.env.APIURL}/Dentist/Me`, initData).then(response => response.json());
		commit('setAccount', data);
	},
	async pushAccount({commit, rootGetters}, newData) {
		if (!rootGetters['Auth/isLoggedIn']) return false;
		const initData: RequestInit = {
			method: 'PUT',
			headers: {
				'Authorization': `Bearer ${rootGetters['Auth/token']}`,
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(newData)
		}
		const data = await fetch(`${process.env.APIURL}/Dentist/Me`, initData).then(response => response.json());
		commit('setAccount', data);
	},
	async updatePatients({commit, rootGetters}, all) {
		const initData: RequestInit = {
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${rootGetters['Auth/token']}`,
			}
		}
		const data = await fetch(`${process.env.APIURL}/Dentist/Patients${all ? '?all' : ''}`, initData).then(response => response.json());
		commit('setPatients', data);
	},


	async updateAppointments({commit, rootGetters}) {
		const initData: RequestInit = {
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${rootGetters['Auth/token']}`,
			}
		}
		const data = await fetch(`${process.env.APIURL}/Dentist/Appointments`, initData).then(response => response.json());
		commit('setAppointments', data);
	},


	async pullServices({commit, rootGetters}) {
		if (!rootGetters['Auth/isLoggedIn']) return false;
		const initData: RequestInit = {
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${rootGetters['Auth/token']}`,
			}
		}
		const data = await fetch(`${process.env.APIURL}/Dentist/Services`, initData).then(response => response.json());
		commit('setServices', data);
	},
	async pushService({dispatch, rootGetters}, newData: Service) {
		if (!rootGetters['Auth/isLoggedIn']) return false;
		const initData: RequestInit = {
			method: newData.id ? 'PUT' : 'POST',
			headers: {
				'Authorization': `Bearer ${rootGetters['Auth/token']}`,
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(newData)
		}
		await fetch(`${process.env.APIURL}/Dentist/Services`, initData);
		dispatch('pullServices');
	},
	async dropService({dispatch, rootGetters}, id: number) {
		if (!rootGetters['Auth/isLoggedIn']) return false;
		const initData: RequestInit = {
			method: 'DELETE',
			headers: {
				'Authorization': `Bearer ${rootGetters['Auth/token']}`,
			},
		}
		await fetch(`${process.env.APIURL}/Dentist/Services/${id}`, initData);
		dispatch('pullServices');
	},
	async linkService({dispatch, rootGetters}, {id, link}: {id: number, link: boolean}) {
		if (!rootGetters['Auth/isLoggedIn']) return false;
		const initData: RequestInit = {
			method: 'PUT',
			headers: {
				'Authorization': `Bearer ${rootGetters['Auth/token']}`,
			},
		}
		await fetch(`${process.env.APIURL}/Dentist/Services/${id}/${link ? 'Link' : 'Unlink'}`, initData);
		dispatch('pullServices');
	},

	async exportAppointmentsPdf({getters, rootGetters}, {id, appointment}) {
		if (!rootGetters['Auth/isLoggedIn']) return false;

		const inputData = appointment ? [appointment] : getters.appointments.filter(a => a.patientId === id);
		let data = [];
		inputData.forEach(appointment => {
			data.push({...appointment});
		});
		console.log(getters.account)
		data = data.map(a => ([
			a.startDate.substr(0, 10),
			a.startDate.substr(11, 5),
			`${a.duration.substr(0, 5).replace(':', 'h ')}m`,
			getters.account.name,
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

		const patient = getters.patients.find(p => p.id === id) as Patient;
		doc.setFontSize(14);
		doc.text(`${patient.firstName} ${patient.lastName}`, 20, 48);
		doc.text(patient.email, 20, 68);
		doc.text(getters.account.name, 100, 48);
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

	async exportAppointmentsXml({getters, rootGetters}, {id}) {
		if (!rootGetters['Auth/isLoggedIn']) return false;

		const initData: RequestInit = {
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${rootGetters['Auth/token']}`,
			}
		}
		const data = await fetch(`${process.env.APIURL}/Dentist/Appointments/Patients/${id}/Xml`, initData).then(response => response.text());
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