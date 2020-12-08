import Appointment from "~/interfaces/Appointment";
import Dentist from "~/interfaces/Dentist";
import Patient from "~/interfaces/Patient";

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
	async updateAccount({commit, rootGetters}) {
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
}