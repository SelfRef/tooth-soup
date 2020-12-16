<template>
	<v-row justify="center">
		<v-dialog
			v-model="active"
			persistent
			max-width="600px"
		>
			<v-tabs v-model="currentTab" v-if="!edit && !register" centered fixed-tabs>
				<v-tab>Create</v-tab>
				<v-tab>Link</v-tab>
			</v-tabs>
			<v-tabs-items v-model="currentTab">
				<v-tab-item>
					<v-card>
						<v-card-title>
							<span v-if="register" class="headline">Register as patient</span>
							<span v-else class="headline">{{ edit ? 'Edit' : 'Create' }} Patient</span>
						</v-card-title>
						<v-card-text>
							<v-form ref="form" v-if="patient">
								<v-container>
									<v-row>
										<v-col
											cols="12"
											sm="6"
										>
											<v-text-field
												label="PESEL number"
												v-model="patient.pesel"
												prepend-icon="mdi-numeric"
												:rules="[rules.required, rules.pesel]"
											></v-text-field>
										</v-col>
										<v-col
											cols="12"
											sm="6"
										>
											<v-menu
												v-model="datePickerActive"
											>
												<template v-slot:activator="{ on, attrs }">
													<v-text-field
														:value="patient.birthDate | date"
														label="Birth date"
														persistent-hint
														prepend-icon="mdi-calendar"
														v-bind="attrs"
														v-on="on"
														:rules="[rules.required]"
													></v-text-field>
												</template>
												<v-date-picker
													:value="patient.birthDate | date"
													@change="patient.birthDate = $event"
													no-title
													@input="datePickerActive = false"
													:max="now.substr(0, 10)"
												></v-date-picker>
											</v-menu>
										</v-col>
										<v-col
											cols="12"
											sm="6"
										>
											<v-text-field
												label="First name"
												v-model="patient.firstName"
												prepend-icon="mdi-card-account-details"
												:rules="[rules.required]"
											></v-text-field>
										</v-col>
										<v-col
											cols="12"
											sm="6"
										>
											<v-text-field
												label="Last name"
												v-model="patient.lastName"
												prepend-icon="mdi-card-account-details"
												:rules="[rules.required]"
											></v-text-field>
										</v-col>
										<v-col cols="12" sm="6">
											<v-text-field
												label="Email"
												:required="!Boolean(edit)"
												v-model="patient.email"
												prepend-icon="mdi-email"
												:rules="[rules.required, rules.email]"
											></v-text-field>
										</v-col>
										<v-col cols="12" sm="6">
											<v-text-field
												label="Password"
												type="password"
												v-model="patient.password"
												prepend-icon="mdi-lock"
												:rules="!edit ? [rules.required] : undefined"
												:placeholder="edit ? '(unchanged)' : undefined"
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
							<v-spacer></v-spacer>
							<v-btn
								color="secondary"
								text
								@click="close"
							>
								Close
							</v-btn>
							<v-btn
								v-if="register"
								color="primary"
								@click="registerPatient"
							>
								Register
							</v-btn>
							<v-btn
								v-else
								color="primary"
								@click="save"
							>
								Save
							</v-btn>
						</v-card-actions>
					</v-card>
				</v-tab-item>
				<v-tab-item>
					<v-card>
						<v-card-title>
							<span class="headline">Link Patient</span>
						</v-card-title>
						<v-card-text>
							<v-container>
								<v-row>
									<v-col
										cols="12"
									>
										<v-select
											:items="unlinkedUsers"
											:item-text="u => `(${u.pesel}) ${u.firstName} ${u.lastName}`"
											item-value="id"
											v-model="unlinkedUserSelected"
											prepend-icon="mdi-account-search"
										></v-select>
									</v-col>
								</v-row>
							</v-container>
						</v-card-text>
						<v-card-actions>
							<v-spacer></v-spacer>
							<v-btn
								color="secondary"
								text
								@click="close"
							>
								Close
							</v-btn>
							<v-btn
								color="primary"
								@click="linkUser"
							>
								Link
							</v-btn>
						</v-card-actions>
					</v-card>
				</v-tab-item>
			</v-tabs-items>
		</v-dialog>
	</v-row>
</template>

<script lang="ts">
import { Vue, Component, Prop, Ref, Watch, Emit } from 'vue-property-decorator';
import Patient from '~/interfaces/Patient';
import '~/lib/extensions';
import { peselToDateString, rules } from "~/lib/helpers";

@Component({
	filters: {
		date: (isoDate: string) => isoDate ? isoDate.substr(0, 10) : null,
	}
})
export default class PatientForm extends Vue {
	@Prop({default: false}) active!: boolean;
	@Prop({default: false}) register!: boolean;
	@Prop({default: null}) patientData!: Patient | null;
	@Ref('form') form;
	private rules = rules;
	private alert: string | null = null;
	private datePickerActive = false;
	private unlinkedUsers: Patient[] = [];
	private currentTab: number | null = 0;
	private unlinkedUserSelected: number | null = null;
	private patient: Patient | null = null;

	get role() {
		return this.$store.getters['Auth/userRole'];
	}

	get edit() {
		return Boolean(this.patientData);
	}

	get now() {
		return new Date().toLocalISO();
	}

	async save() {
		if (!this.form.validate()) return;
		const fetchOptions: RequestInit = {
			method: this.edit ? 'PUT' : 'POST',
			body: JSON.stringify(this.patient),
			headers: {
					'Authorization': `Bearer ${this.$store.getters['Auth/token']}`,
					'Content-Type': 'application/json'
				}
		}
		await fetch(`${process.env.APIURL}/${this.role}/Patients`, fetchOptions);
		this.$emit('refresh');
		this.close();
	}

	async linkUser() {
		const fetchOptions: RequestInit = {
			method: 'PUT',
			headers: {
					'Authorization': `Bearer ${this.$store.getters['Auth/token']}`,
				}
		}
		await fetch(`${process.env.APIURL}/${this.role}/Patients/${this.unlinkedUserSelected}/Link`, fetchOptions);
		this.$emit('refresh');
		this.close();
	}

	async registerPatient() {
		if (!this.form.validate()) return;
		let fetchOptions: RequestInit = {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json',
			},
			body: JSON.stringify(this.patient),
		}
		let result = await fetch(`${process.env.APIURL}/Login/RegisterPatient`, fetchOptions) as Response;
		if (!result.ok) {
			this.alert = await result.text();
			return;
		}

		fetchOptions = {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json',
			},
			body: JSON.stringify(this.patient),
		}
		result = await fetch(`${process.env.APIURL}/Login`, fetchOptions) as Response;
		if (result.ok) {
			let tokenData = await result.json()
			await this.$store.dispatch('Auth/setToken', tokenData.token);
		}
		this.close();
	}

	@Emit('update:active')
	close() {
		this.form.reset();
		this.alert = null;
		return false;
	}

	@Watch('currentTab')
	async inlinkedUsers(tab: number) {
		if (tab != 1) return;

		const fetchOptions: RequestInit = {
			method: 'GET',
			headers: {
					'Authorization': `Bearer ${this.$store.getters['Auth/token']}`,
				},
		}
		this.unlinkedUsers = await fetch(`${process.env.APIURL}/${this.role}/Patients?unlinked`, fetchOptions).then(r => r.json());
	}

	@Watch('active')
	onDialogShow(active: boolean) {
		if (active) {
			this.patient = this.patientData ? {...this.patientData} : {
				pesel: null,
				firstName: null,
				lastName: null,
				email: null,
				password: null,
				birthDate: null,
			};
			this.form?.resetValidation();
		}
	}

	@Watch('patient.pesel')
	onPeselChange(pesel: string) {
		if (pesel && pesel.length >= 6 && this.patient) {
			this.patient.birthDate = peselToDateString(pesel);
		}
	}
}
</script>