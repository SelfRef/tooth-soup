<template>
	<v-row justify="center">
		<v-dialog
			v-model="active"
			persistent
			max-width="600px"
		>
			<v-card>
				<v-card-title>
					<span class="headline">User Login</span>
				</v-card-title>
				<v-card-text>
					<v-container>
						<v-row>
							<v-col cols="12">
								<v-text-field
									v-model="loginData.email"
									label="Email*"
									required
								></v-text-field>
							</v-col>
							<v-col cols="12">
								<v-text-field
									v-model="loginData.password"
									label="Password*"
									type="password"
									required
								></v-text-field>
							</v-col>
						</v-row>
					</v-container>
					<small>*indicates required field</small>
				</v-card-text>
				<v-card-actions>
					<v-btn
						color="blue darken-1"
						text
						@click="$emit('update:active', false)"
					>
						Close
					</v-btn>

					<v-spacer/>

					<v-btn
						color="primary"
						@click="login"
					>
						Login
					</v-btn>
				</v-card-actions>
			</v-card>
		</v-dialog>
	</v-row>
</template>

<script lang="ts">
import { Vue, Component, Prop } from 'vue-property-decorator';

@Component
export default class LoginForm extends Vue {
	@Prop() active = false;
	private loginData = {
		email: '',
		password: '',
	};

	async login() {
		let postData: RequestInit = {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json',
			},
			body: JSON.stringify(this.loginData),
		}
		try {
			let tokenResponse: Response = await fetch(`${process.env.APIURL}/Login`, postData);
			if (!tokenResponse.ok) throw new Error("Wrong login data");

			let tokenData = await tokenResponse.json()
			this.$store.dispatch('auth/setToken', tokenData.token);
			this.$emit('update:active', false);
		} catch(e) {

		}
	}
}
</script>