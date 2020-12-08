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
					<v-form @keypress.native.enter="login" ref="form">
						<v-container>
							<v-row>
								<v-col cols="12">
									<v-text-field
										v-model="data.email"
										label="Email"
										prepend-icon="mdi-email"
										:rules=[rules.required]
									></v-text-field>
								</v-col>
								<v-col cols="12">
									<v-text-field
										v-model="data.password"
										label="Password"
										type="password"
										prepend-icon="mdi-lock"
										:rules=[rules.required]
									></v-text-field>
								</v-col>
							</v-row>
							<v-row v-if="alert">
								<v-col>
									<v-alert type="error">{{alert}}</v-alert>
								</v-col>
							</v-row>
						</v-container>
					</v-form>
				</v-card-text>
				<v-card-actions>
					<v-btn
						color="blue darken-1"
						text
						@click="close"
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
import { Vue, Component, Prop, Ref, Emit } from 'vue-property-decorator';

@Component
export default class LoginForm extends Vue {
	@Prop({default: false}) active;
	@Ref() form!;
	private alert: string | null = null;
	private data = {
		email: '',
		password: '',
	};

	private rules = {
		required: (v: string) => Boolean(v) || 'Required',
	}

	@Emit('update:active')
	close() {
		this.form.reset();
		this.alert = null;
		return false;
	}

	async login() {
		if (!this.form.validate()) return;
		let fetchOptions: RequestInit = {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json',
			},
			body: JSON.stringify(this.data),
		}
		const result = await fetch(`${process.env.APIURL}/Login`, fetchOptions) as Response;
		if (!result.ok) {
			this.alert = 'Please check you login data';
			return;
		}
		let tokenData = await result.json()
		await this.$store.dispatch('Auth/setToken', tokenData.token);
		this.close()
		this.$router.push('/');
	}
}
</script>