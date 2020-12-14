<template>
	<v-container class="mt-10">
		<v-card>
			<v-row>
				<v-col cols="4">
					<v-img v-if="statusCode === 404" src="https://coubsecure-s.akamaihd.net/get/b34/p/coub/simple/cw_timeline_pic/92a3eb1e9bb/b3a03ea93f14ae2ee7791/ios_large_1517510363_image.jpg"></v-img>
					<v-img v-else src="https://imgflip.com/s/meme/Hide-the-Pain-Harold.jpg"></v-img>
				</v-col>
				<v-col>
					<v-card-title v-if="statusCode === 404">You are in the wrong place<v-icon right>mdi-emoticon-angry</v-icon></v-card-title>
					<v-card-title v-else>Something went wrong<v-icon right>mdi-emoticon-sad</v-icon></v-card-title>
					<v-card-text>
						<p v-if="statusCode === 404">The page you are trying to visit does not exist, did not exist, and will not exist. Better forget about it and go elsewhere.</p>
						<p v-else>There appears to be an error on the page. We apologize for the inconvenience and we will try to fix it as soon as possible.</p>
						<p v-if="statusCode"><v-icon left>mdi-home</v-icon>Go back to the <nuxt-link to="/">home page</nuxt-link>, or try refreshing the browser tab.</p>
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
import { NuxtError } from '@nuxt/types';
import { Vue, Component, Prop } from 'vue-property-decorator';

@Component
export default class error extends Vue {
	@Prop() error!: NuxtError
	get loggedIn() {
		return Boolean(this.$store.getters['Auth/token']);
	}
	get statusCode() {
		return this.error?.statusCode;
	}
}
</script>