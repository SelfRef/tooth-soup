<template>
	<v-app>
		<v-app-bar fixed app>
			<v-toolbar-title style="overflow: visible" class="mr-6">
				ToothSoup
				<v-icon right>mdi-tooth</v-icon>
				<v-icon>mdi-bowl-mix</v-icon></v-toolbar-title>
			<v-tabs :value="tabNumber">
				<v-tab to="/">Home</v-tab>
				<v-tab v-if="role === 'Dentist'" to="/patients">Patients</v-tab>
				<v-tab v-if="role === 'Patient'" to="/appointments">Appointments</v-tab>
				<v-tab v-if="role === 'Admin'" to="/users">Users</v-tab>
				<v-tab v-if="role === 'Dentist' || role === 'Admin'" to="/services">Services</v-tab>
				<v-tab v-if="loggedIn && role !== 'Admin'" to="/account">Account</v-tab>
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
						>{{ initials }}</v-avatar>
					</template>
					<v-list>
						<v-list-item @click="logout" color="error">
							<v-list-item-title>Logout</v-list-item-title>
						</v-list-item>
					</v-list>
				</v-menu>
			</template>
			<template v-else>
				<v-btn
					class="mr-2"
					text
					@click="registerDialog = !registerDialog"
				>Register</v-btn>
				<v-btn
				right
					color="primary"
					@click="loginDialog = !loginDialog"
				>Login</v-btn>
			</template>
		</v-app-bar>
		<v-main>
			<nuxt />
		</v-main>
		<v-footer app>
			<span>&copy; ToothSoup Corp. {{ new Date().getFullYear() }}</span>
		</v-footer>
		<login-form :active.sync="loginDialog"/>
		<patient-form :active.sync="registerDialog" :register="true" />
	</v-app>
</template>

<script lang="ts">
import 'reflect-metadata';
import { Watch } from 'vue-property-decorator';
import { Vue, Component, Prop } from 'vue-property-decorator';
import LoginForm from "~/components/LoginForm.vue";
import PatientForm from "~/components/PatientForm.vue";
@Component({
	components: {
		LoginForm,
		PatientForm,
	}
})
export default class DefaultLayout extends Vue {
	private loginDialog = false;
	private registerDialog = false;
	private theme = 0;

	get loggedIn() {
		return Boolean(this.$store.getters['Auth/token']);
	}
	
	get role() {
		return this.$store.getters['Auth/userRole'];
	}

	get initials() {
		return this.$store.getters['Auth/userInitials'];
	}

	get tabNumber() {
		switch(this.$route.path) {
			case '/':
				return 0;
			case '/patients':
				return 1;
			case '/appointments':
				return 2;
			case '/users':
				return 3;
			case '/services':
				return 4;
			case '/account':
				return 5;
			default:
				return null;
		}
	}

	logout() {
		this.$store.dispatch('Auth/setToken', null);
	}

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
	}
	
	checkLoggedIn(loggedIt) {
		if (!loggedIt) {
			this.$router.push('/');
		}
	}

	mounted() {
		this.theme = this.$store.getters['Auth/theme'];
		this.changeTheme(this.theme);
		this.$store.dispatch('Auth/checkToken');
		this.checkLoggedIn(this.loggedIn);
	}

	@Watch('theme')
	themeChange(theme) {
		this.$store.dispatch('Auth/setTheme', theme);
		this.changeTheme(theme);
	}

	@Watch('loggedIn')
	loggedInChange(loggedIn) {
		this.checkLoggedIn(loggedIn);
	}
}
</script>