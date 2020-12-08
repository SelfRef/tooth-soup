import Service from "~/interfaces/Service";
import User from "~/interfaces/User";

type State = {
	users: User[],
	services: Service[],
}

export const state = (): State => ({
	users: [],
	services: [],
})

export const getters = {
	users(state: State): User[] {
		return state.users;
	},
	services(state: State): Service[] {
		return state.services;
	},
};

export const mutations = {
	setUsers(state: State, data: User[]) {
		state.users = data;
	},
	setServices(state: State, data: Service[]) {
		state.services = data;
	},
}

export const actions = {
	async pullUsers({commit, rootGetters}) {
		if (!rootGetters['Auth/isLoggedIn']) return false;
		const initData: RequestInit = {
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${rootGetters['Auth/token']}`,
			}
		}
		const data = await fetch(`${process.env.APIURL}/Admin/Users`, initData).then(response => response.json());
		commit('setUsers', data);
	},
	async pushUser({dispatch, rootGetters}, newData: User) {
		if (!rootGetters['Auth/isLoggedIn']) return false;
		const initData: RequestInit = {
			method: newData.id ? 'PUT' : 'POST',
			headers: {
				'Authorization': `Bearer ${rootGetters['Auth/token']}`,
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(newData)
		}
		const result = await fetch(`${process.env.APIURL}/Admin/Users`, initData);
		dispatch('pullUsers');
		return result;
	},
	async dropUser({dispatch, rootGetters}, id: number) {
		if (!rootGetters['Auth/isLoggedIn']) return false;
		const initData: RequestInit = {
			method: 'DELETE',
			headers: {
				'Authorization': `Bearer ${rootGetters['Auth/token']}`,
			},
		}
		await fetch(`${process.env.APIURL}/Admin/Users/${id}`, initData);
		dispatch('pullUsers');
	},



	async pullServices({commit, rootGetters}) {
		if (!rootGetters['Auth/isLoggedIn']) return false;
		const initData: RequestInit = {
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${rootGetters['Auth/token']}`,
			}
		}
		const data = await fetch(`${process.env.APIURL}/Admin/Services`, initData).then(response => response.json());
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
		await fetch(`${process.env.APIURL}/Admin/Services`, initData);
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
		await fetch(`${process.env.APIURL}/Admin/Services/${id}`, initData);
		dispatch('pullServices');
	},
}