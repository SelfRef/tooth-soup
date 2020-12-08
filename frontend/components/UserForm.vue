<template>
	<v-row justify="center">
		<v-dialog
			v-if="active"
			v-model="active"
			persistent
			max-width="600px"
		>
			<v-card>
				<v-card-title>
					<span class="headline">{{ edit ? 'Edit' : 'Create' }} User</span>
				</v-card-title>
				<v-card-text>
					<v-form ref="form" v-if="data">
						<v-container>
							<v-row><v-col><h3>General</h3></v-col></v-row>
							<v-row>
								<v-col
									cols="12"
									sm="6"
								>
									<v-text-field
										label="First name"
										v-model="data.firstName"
										prepend-icon="mdi-form-textbox"
										:rules="[rules.required]"
									></v-text-field>
								</v-col>
								<v-col
									cols="12"
									sm="6"
								>
									<v-text-field
										label="Last name"
										v-model="data.lastName"
										prepend-icon="mdi-form-textbox"
										:rules="[rules.required]"
									></v-text-field>
								</v-col>
							</v-row>
							<v-row>
								<v-col
									cols="12"
									sm="6"
								>
									<v-text-field
										label="Email"
										type="email"
										v-model="data.email"
										prepend-icon="mdi-email"
										:rules="[rules.required]"
									></v-text-field>
								</v-col>
								<v-col
									cols="12"
									sm="6"
								>
									<v-text-field
										label="Password"
										type="password"
										v-model="data.password"
										prepend-icon="mdi-lock"
										:placeholder="edit ? '(unchanged)' : undefined"
										:rules="!edit ? [rules.required] : undefined"
									></v-text-field>
								</v-col>
							</v-row>
							<v-row>
								<v-col
									cols="12"
								>
									<v-select
										label="Role"
										:readonly="edit"
										:items="roles"
										v-model="data.role"
										prepend-icon="mdi-account-question"
										:rules="[rules.required]"
									></v-select>
								</v-col>
							</v-row>
							<v-divider v-if="data.role && data.role !== 'Admin'" />
							<template v-if="data.role === 'Dentist'">
								<v-row><v-col><h3>Dentist</h3></v-col></v-row>
								<v-row>
									<v-col
										cols="12"
									>
										<v-switch
											label="Patients can link me as the main dentist"
											v-model="data.dentist.canLink"
											hide-details
										/>
									</v-col>
								</v-row>
								<v-row>
									<v-col
										cols="12"
									>
										<v-switch
											label="Not mine patients can create appointments"
											v-model="data.dentist.canCreateAppointment"
											hide-details
										/>
									</v-col>
								</v-row>
							</template>
							<template v-if="data.role === 'Patient'">
								<v-row><v-col><h3>Patient</h3></v-col></v-row>
								<v-row>
									<v-col
										cols="12"
										sm="6"
									>
										<v-text-field
											label="PESEL number"
											v-model="data.patient.pesel"
											prepend-icon="mdi-numeric"
											:rules="[rules.required, rules.number]"
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
													:value="data.patient.birthDate | date"
													label="Birth date"
													persistent-hint
													prepend-icon="mdi-calendar"
													v-bind="attrs"
													v-on="on"
													:rules="[rules.required]"
												></v-text-field>
											</template>
											<v-date-picker
												:value="data.patient.birthDate | date"
												@change="data.patient.birthDate = $event"
												no-title
												@input="datePickerActive = false"
											></v-date-picker>
										</v-menu>
									</v-col>
								</v-row>
								<v-row>
									<v-col
										cols="12"
									>
										<v-select
											:items="dentists"
											:item-text="d => d.name"
											item-value="id"
											v-model="data.patient.dentistId"
											prepend-icon="mdi-doctor"
										></v-select>
									</v-col>
								</v-row>
							</template>
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
		</v-dialog>
	</v-row>
</template>

<script lang="ts">
import { Vue, Component, Prop, Ref, Watch, Emit } from 'vue-property-decorator';
import User from '~/interfaces/User';
import Dentist from '~/interfaces/Dentist';
import Patient from '~/interfaces/Patient';

@Component({
	filters: {
		date: (isoDate: string) => isoDate ? isoDate.substr(0, 10) : null,
	}
})
export default class UserForm extends Vue {
	@Prop({default: false}) active!: boolean;
	@Prop({default: null}) item!: User | null;
	@Prop() dentists!: Array;
	@Ref('form') form;
	private datePickerActive = false;
	private alert: string | null = null;
	private data: User | null = null;

	private roles = [ 'Patient', 'Dentist', 'Admin' ];

	private rules = {
		required: (v: string) => Boolean(v) || 'Required',
		number: (v: string) => /^\d+$/.test(v) || 'Must be a number',
	};

	get edit() {
		return Boolean(this.item);
	}

	get role() {
		return this.$store.getters['Auth/userRole'];
	}

	@Emit('update:active')
	close() {
		return false;
	}

	@Emit('refresh')
	async save() {
		if (!this.form.validate()) return;
		switch (this.data?.role) {
			case 'Patient':
				if (this.data) this.data.dentist = null;
				break;
			case 'Dentist':
				if (this.data) this.data.patient = null;
				break;
			default:
				if (this.data) {
					this.data.dentist = null;
					this.data.patient = null;
				}
				break;
		}
		const result = await this.$store.dispatch(`${this.role}/pushUser`, this.data) as Response;
		if (result.ok) this.close();
		else this.alert = await result.text();
	}

	@Watch('active')
	async onDialogShow(active: boolean) {
		if (active) {
			this.data = this.item ? {...this.item} : {
				firstName: null,
				lastName: null,
				email: null,
				password: null,
				role: null,
			};
			if (this.data) {
				this.data.dentist = this.item?.dentist ? {...this.item?.dentist} : {
					canLink: true,
					canCreateAppointment: true
				};
				this.data.patient = this.item?.patient ? {...this.item?.patient} : {
					pesel: null,
					birthDate: null,
					dentistId: null
				};
			}
		} else this.form?.reset();
	}
}
</script>