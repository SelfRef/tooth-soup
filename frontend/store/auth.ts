type State = {
	token: string | null
}

export const state = (): State => ({
	token: null,
})

export const getters = {
	token(state: State) {
		return state.token;
	}
}

export const mutations = {
	setToken(state: State, token: string) {
		state.token = token;
	}
}

export const actions = {
}