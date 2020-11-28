type State = {
	token: string | null
	userId: number | null
	userRole: string | null
}

export const state = (): State => ({
	token: null,
	userId: null,
	userRole: null,
})

export const getters = {
	token(state: State) {
		return state.token;
	},
	userId(state: State) {
		return state.userId;
	},
	userRole(state: State) {
		return state.userRole;
	},
};

export const mutations = {
	setToken(state: State, token: string) {
		state.token = token;
	},
	setUserId(state: State, userId: string) {
		state.userId = userId ? Number(userId) : null;
	},
	setUserRole(state: State, userRole: string) {
		state.userRole = userRole;
	},
}

export const actions = {
	setToken({commit}, token: string) {
		commit('setToken', token);
		let id = null, role = null;

		if (token) {
			const dataStr = token.split('.')[1];
			const data = JSON.parse(atob(dataStr));

			id = data['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
			role = data['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
		}

		commit('setUserId', id);
		commit('setUserRole', role);
	}
}