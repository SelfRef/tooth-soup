<template>
	<v-row justify="center">
		<v-dialog
			v-model="active"
			persistent
			max-width="600px"
		>
			<v-tabs v-model="currentTab" v-if="!edit" centered fixed-tabs>
				<v-tab>Create</v-tab>
				<v-tab>Link</v-tab>
			</v-tabs>
			<v-tabs-items v-model="currentTab">
				<v-tab-item>
					<v-form ref="form">
						<v-card>
							<v-card-title>
								<span class="headline">{{ edit ? 'Edit' : 'Create' }} Patient</span>
							</v-card-title>
							<v-card-text>
								<v-container>
									<v-row>
										<v-col
											cols="12"
											sm="6"
										>
											<v-text-field
												label="PESEL number"
												:required="!Boolean(edit)"
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
														v-model="patient.birthDate"
														label="Birth Date"
														persistent-hint
														prepend-icon="mdi-calendar"
														v-bind="attrs"
														v-on="on"
														:required="!Boolean(edit)"
														:rules="[rules.required]"
													></v-text-field>
												</template>
												<v-date-picker
													v-model="patient.birthDate"
													no-title
													@input="datePickerActive = false"
												></v-date-picker>
											</v-menu>
										</v-col>
										<v-col
											cols="12"
											sm="6"
										>
											<v-text-field
												label="Legal first name"
												:required="!Boolean(edit)"
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
												label="Legal last name"
												:required="!Boolean(edit)"
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
												:required="!Boolean(edit)"
												v-model="patient.password"
												prepend-icon="mdi-lock"
												:rules="[rules.required]"
											></v-text-field>
										</v-col>
									</v-row>
								</v-container>
							</v-card-text>
							<v-card-actions>
								<v-spacer></v-spacer>
								<v-btn
									color="blue darken-1"
									text
									@click="close"
								>
									Close
								</v-btn>
								<v-btn
									color="blue darken-1"
									text
									@click="save"
								>
									Save
								</v-btn>
							</v-card-actions>
						</v-card>
					</v-form>
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
								color="blue darken-1"
								text
								@click="close"
							>
								Close
							</v-btn>
							<v-btn
								color="blue darken-1"
								text
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
import { Vue, Component, Prop, Ref, Watch } from 'vue-property-decorator';
import Patient from '~/interfaces/Patient';

@Component
export default class PatientEditForm extends Vue {
	@Prop({default: false}) active!: boolean;
	@Prop({default: null}) patientData!: Patient | null;
	@Ref('form') form;
	private datePickerActive = false;
	private unlinkedUsers: Patient[] = [];
	private currentTab: number | null = null;
	private unlinkedUserSelected: number | null = null;
	private patient: Patient = {
		pesel: '',
		birthDate: '',
		firstName: '',
		lastName: '',
		email: '',
		password: '',
	}
	private rules = {
		required: (v: string) => this.edit || Boolean(v) || 'Required',
		pesel: (v: string) => {
			if (v.length !== 11) return 'Must be 11 digits long';
			if (!/^\d+$/.test(v)) return 'Must have only digits';
			return true;
		},
		email: (v: string) => {
			const pattern = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
			return pattern.test(v) || 'Invalid e-mail'
		},
	};

	close() {
		this.form.resetValidation();
		this.$emit('update:active', false);
	}

	async save() {
		if (!this.form.validate()) return;
		const fetchOptions: RequestInit = {
			method: this.edit ? 'PUT' : 'POST',
			body: JSON.stringify(this.patient),
			headers: {
					'Authorization': `Bearer ${this.$store.getters['auth/token']}`,
					'Content-Type': 'application/json'
				}
		}
		await fetch(`${process.env.APIURL}/Dentist/Patient`, fetchOptions);
		this.$emit('refresh');
		this.close();
	}

	async linkUser() {
		const fetchOptions: RequestInit = {
			method: 'GET',
			headers: {
					'Authorization': `Bearer ${this.$store.getters['auth/token']}`,
				}
		}
		await fetch(`${process.env.APIURL}/Dentist/Patient/${this.unlinkedUserSelected}/Link`, fetchOptions);
		this.$emit('refresh');
		this.close();
	}

	get edit() {
		return Boolean(this.patientData);
	}

	@Watch('currentTab')
	async inlinkedUsers(tab: number) {
		if (tab != 1) return;

		const fetchOptions: RequestInit = {
			method: 'GET',
			headers: {
					'Authorization': `Bearer ${this.$store.getters['auth/token']}`,
				},
		}
		this.unlinkedUsers = await fetch(`${process.env.APIURL}/Dentist/Patients?unlinked`, fetchOptions).then(r => r.json());
	}

	@Watch('active')
	onDialogShow() {
		setTimeout(() => this.currentTab = 0, 500);
		this.patient = this.patientData ? {...this.patientData} : {
			pesel: '',
			firstName: '',
			lastName: '',
			email: '',
			password: '',
			birthDate: '',
		};
	}
}
</script>