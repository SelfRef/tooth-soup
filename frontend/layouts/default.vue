<template>
	<v-app>
		<v-app-bar fixed app>
			<v-toolbar-title style="overflow: visible" class="mr-6">
				Tooth Soup
				<v-icon right>mdi-tooth</v-icon>
				<v-icon>mdi-bowl-mix</v-icon></v-toolbar-title>
			<v-tabs :value="tabNumber">
				<v-tab to="/">Home</v-tab>
				<v-tab v-if="role === 'Dentist'" to="/patients">Patients</v-tab>
				<v-tab v-if="role === 'Dentist'" to="/services">Services</v-tab>
				<v-tab v-if="role === 'Patient'" to="/appointments">Appointments</v-tab>
				<v-tab v-if="loggedIn" to="/account">Account</v-tab>
			</v-tabs>
			<v-spacer />
			<v-btn-toggle v-model="theme" class="mr-8">
				<v-btn><v-icon>mdi-brightness-auto</v-icon></v-btn>
				<v-btn><v-icon>mdi-brightness-7</v-icon></v-btn>
				<v-btn><v-icon>mdi-brightness-4</v-icon></v-btn>
			</v-btn-toggle>
			<template v-if="loggedIn">
				<v-menu offset-y>
					<template v-slot:activator="{ on, attrs }">
						<v-avatar
							color="primary"
							v-bind="attrs"
							v-on="on"
						>JN</v-avatar>
					</template>
					<v-list>
						<v-list-item to="/account">
							<v-list-item-title>Account</v-list-item-title>
						</v-list-item>
						<v-list-item @click="logout">
							<v-list-item-title>Logout</v-list-item-title>
						</v-list-item>
					</v-list>
				</v-menu>
			</template>
			<template v-else>
				<v-btn
					color="primary"
					@click="loginDialog = !loginDialog"
				>Login</v-btn>
			</template>
		</v-app-bar>
		<v-main>
			<nuxt />
		</v-main>
		<v-footer app>
			<span>&copy; {{copyName}} {{ new Date().getFullYear() }}</span>
		</v-footer>
		<login-form :active.sync="loginDialog"/>
	</v-app>
</template>

<script>
import 'reflect-metadata';
import { Vue, Component, Prop } from 'vue-property-decorator';
import LoginForm from "@/components/LoginForm";
export default {
	components: {
		LoginForm,
	},
	data() {
		return {
			title: 'Vuetify.js',
			copyName: 'Tooth Soup Corp.',
			loginDialog: false,
			theme: 0,
		}
	},
	computed: {
		loggedIn() {
			return Boolean(this.$store.getters['auth/token']);
		},
		role() {
			return this.$store.getters['auth/userRole'];
		},
		tabNumber() {
			switch(this.$route.path) {
				case '/':
					return 0;
				case '/patients':
					return 1;
				case '/services':
					return 2;
				case '/appointments':
					return 3;
				case '/account':
					return 4;
				default:
					return null;
			}
		}
	},
	methods: {
		logout() {
			this.$store.dispatch('auth/setToken', null);
		},
		changeTheme(theme) {
			switch (theme) {
				case 0:
					const mq = window.matchMedia('(prefers-color-scheme: dark)');
					this.$vuetify.theme.dark = mq.matches;
					mq.addEventListener('change', (e) => {
						this.$vuetify.theme.dark = e.matches;
						this.theme = 0;
					});
					break;
				case 1:
					this.$vuetify.theme.dark = false;
					break;
				case 2:
					this.$vuetify.theme.dark = true;
					break;
			}
		},
		checkLoggedIn(loggedIt) {
			if (!loggedIt) {
				this.$router.push('/');
			}
		}
	},
	mounted() {
		this.theme = this.$store.getters['auth/theme'];
		this.changeTheme(this.theme);
		this.$store.dispatch('auth/checkToken');
		this.checkLoggedIn(this.loggedIn);
	},
	watch: {
		theme(theme) {
			this.$store.dispatch('auth/setTheme', theme);
			this.changeTheme(theme);
		},
		loggedIn(loggedIn) {
			this.checkLoggedIn(loggedIn);
		}
	}
}
</script>
