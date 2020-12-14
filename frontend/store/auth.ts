type State = {
	token: string | null
	userId: number | null
	userRole: string | null
	userInitials: string | null
	tokenExpiration: string | null
	theme: number
}

export const state = (): State => ({
	token: null,
	userId: null,
	userRole: null,
	userInitials: null,
	tokenExpiration: null,
	theme: 0,
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
	userInitials(state: State) {
		return state.userInitials;
	},
	tokenExpiration(state: State) {
		return state.tokenExpiration ? new Date(state.tokenExpiration) : null;
	},
	isLoggedIn(state: State) {
		return Boolean(state.token);
	},
	theme(state: State) {
		return state.theme;
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
	setUserInitials(state: State, userInitials: string | null) {
		state.userInitials = userInitials;
	},
	setTokenExpiration(state: State, tokenExpiration: Date | null) {
		state.tokenExpiration = tokenExpiration ? dateToLocalISO(tokenExpiration) : null;
	},
	setTheme(state: State, theme: number) {
		state.theme = theme;
	},
}

export const actions = {
	setToken({commit}, token: string) {
		commit('setToken', token);
		let id = null, role = null, exp = null, initials = null;

		if (token) {
			const dataStr = token.split('.')[1];
			const data = JSON.parse(atob(dataStr));

			id = data['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
			role = data['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
			initials = data['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
			exp = new Date(data['exp'] * 1000);
		}

		commit('setUserId', id);
		commit('setUserRole', role);
		commit('setUserInitials', initials);
		commit('setTokenExpiration', exp);
	},

	checkToken({getters, dispatch}) {
		if (getters['tokenExpiration'] < new Date()) {
			dispatch('setToken', null);
		}
	},

	setTheme({commit}, theme: number) {
		commit('setTheme', theme);
	}
}

function dateToLocalISO(date) {
	const offsetMs = date.getTimezoneOffset() * 60 * 1000;
	const msLocal = date.getTime() - offsetMs;
	const dateLocal = new Date(msLocal);
	const iso = dateLocal.toISOString();
	const isoLocal = iso.slice(0, 19);
	return isoLocal;
}