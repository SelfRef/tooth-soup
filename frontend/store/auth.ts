type State = {
	token: string | null
	userId: number | null
	userRole: string | null
	tokenExpiration: string | null
}

export const state = (): State => ({
	token: null,
	userId: null,
	userRole: null,
	tokenExpiration: null,
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
	tokenExpiration(state: State) {
		return state.tokenExpiration ? new Date(state.tokenExpiration) : null;
	},
	isLoggedIn(state: State) {
		return Boolean(state.token);
	}
};

export const mutations = {
	setToken(state: State, token: string | null) {
		state.token = token;
	},
	setUserId(state: State, userId: string | null) {
		state.userId = userId ? Number(userId) : null;
	},
	setUserRole(state: State, userRole: string | null) {
		state.userRole = userRole;
	},
	setTokenExpiration(state: State, tokenExpiration: Date | null) {
		state.tokenExpiration = tokenExpiration ? tokenExpiration.toISOString() : null;
	},
}

export const actions = {
	setToken({commit}, token: string) {
		commit('setToken', token);
		let id = null, role = null, exp = null;

		if (token) {
			const dataStr = token.split('.')[1];
			const data = JSON.parse(atob(dataStr));

			id = data['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
			role = data['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
			exp = new Date(data['exp'] * 1000);
		}

		commit('setUserId', id);
		commit('setUserRole', role);
		commit('setTokenExpiration', exp);
	},

	checkToken({getters, dispatch}) {
		if (getters['tokenExpiration'] < new Date()) {
			dispatch('setToken', null);
		}
	}
}