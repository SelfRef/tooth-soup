import Appointment from "~/interfaces/Appointment";
import Patient from "~/interfaces/Patient";
import Service from "~/interfaces/Service";

type State = {
	patients: Patient[],
	services: Service[],
	appointments: Appointment[],
}

export const state = (): State => ({
	patients: [],
	services: [],
	appointments: [],
})

export const getters = {
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