<template>
	<v-container class="mt-10">
		<v-card>
			<v-row>
				<v-col cols="4">
					<v-img src="https://thumbs.dreamstime.com/z/senior-doctor-holding-papers-smiling-23096004.jpg"></v-img>

				</v-col>
				<v-col>
					<v-card-title>Welcome to our clinic!</v-card-title>
					<v-card-text>
						<p>Our facility guarantees the highest quality of service and modern solutions ensuring quick and convenient registration of the visit.</p>
						<template v-if="loggedIn">
							<p><v-icon left>mdi-star-face</v-icon>You are already logged in, go to the selected tab at the top of the page.</p>
							<p><v-icon left>mdi-logout</v-icon>You can always log out by selecting an option from the menu after clicking on your avatar.</p>
						</template>
						<template v-else>
							<p><v-icon left>mdi-account-key</v-icon>If you already have an account, please login to access your data.</p>
							<p><v-icon left>mdi-account-plus</v-icon>If not, do not hesitate to register as a new patient.</p>
							<p><v-icon left>mdi-calendar-multiple</v-icon>After login you can create and manage your appointments.</p>
							<p><v-icon left>mdi-calendar-star</v-icon>Make sure you have selected your primary dentist in your account settings page.</p>
							<p><v-icon left>mdi-doctor</v-icon>This way your dentist will be able to manage your data.</p>
						</template>
					</v-card-text>
				</v-col>
			</v-row>
		</v-card>
	</v-container>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';

@Component
export default class index extends Vue {
	get loggedIn() {
		return Boolean(this.$store.getters['Auth/token']);
	}
}
</script>