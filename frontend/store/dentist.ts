import Appointment from "~/interfaces/Appointment";
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
	setAccount(state: State, data: Patient) {
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
		if (!rootGetters['auth/isLoggedIn']) return false;
		const initData: RequestInit = {
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${rootGetters['auth/token']}`,
			}
		}
		const data = await fetch(`${process.env.APIURL}/Dentist/Me`, initData).then(response => response.json());
		commit('setAccount', data);
	},
	async pushAccount({commit, rootGetters}, newData) {
		if (!rootGetters['auth/isLoggedIn']) return false;
		const initData: RequestInit = {
			method: 'PUT',
			headers: {
				'Authorization': `Bearer ${rootGetters['auth/token']}`,
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(newData)
		}
		const data = await fetch(`${process.env.APIURL}/Dentist/Me`, initData).then(response => response.json());
		commit('setAccount', data);
	},
	async updatePatients({commit, rootGetters}) {
		const initData: RequestInit = {
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${rootGetters['auth/token']}`,
			}
		}
		const data = await fetch(`${process.env.APIURL}/Dentist/Patients`, initData).then(response => response.json());
		commit('setPatients', data);
	},
	async updateServices({commit, rootGetters}) {
		const initData: RequestInit = {
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${rootGetters['auth/token']}`,
			}
		}
		const data = await fetch(`${process.env.APIURL}/Dentist/Services`, initData).then(response => response.json());
		commit('setServices', data);
	},
	async updateAppointments({commit, rootGetters}) {
		const initData: RequestInit = {
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${rootGetters['auth/token']}`,
			}
		}
		const data = await fetch(`${process.env.APIURL}/Dentist/Appointments`, initData).then(response => response.json());
		commit('setAppointments', data);
	},
}