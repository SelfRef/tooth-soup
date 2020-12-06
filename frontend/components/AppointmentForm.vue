<template>
	<v-row justify="center">
		<v-dialog
			v-if="active"
			v-model="active"
			persistent
			max-width="600px"
		>
			<v-form ref="form">
				<v-card>
					<v-card-title>
						<span class="headline">{{ edit ? 'Edit' : 'Create' }} Appointment</span>
					</v-card-title>
					<v-card-text>
						<v-container>
							<v-row v-if="!dentistId">
								<v-col cols="12">
									<v-select
										:items="dentists"
										:item-text="u => `${u.firstName} ${u.lastName}`"
										item-value="id"
										v-model="appointment.dentistId"
										prepend-icon="mdi-account"
										label="Dentist"
									></v-select>
								</v-col>
							</v-row>
							<v-row>
								<v-col cols="12">
									<v-select
										:items="patients"
										:item-text="u => `(${u.pesel}) ${u.firstName} ${u.lastName}`"
										item-value="id"
										v-model="appointment.patientId"
										prepend-icon="mdi-account"
										label="Patient"
									></v-select>
								</v-col>
							</v-row>
							<v-row>
								<v-col cols="12">
									<v-select
										:items="services"
										:item-text="u => `${u.name} - ${u.price}zł`"
										item-value="id"
										v-model="appointment.serviceId"
										prepend-icon="mdi-puzzle"
										label="Service"
										:rules="[rules.required]"
									></v-select>
								</v-col>
							</v-row>
							<v-row>
								<v-col cols="12">
									<v-menu
										v-model="datePickerActive"
										ref="datePicker"
										:close-on-content-click="false"
									>
										<template v-slot:activator="{ on, attrs }">
											<v-text-field
												v-model="date"
												label="Date"
												persistent-hint
												prepend-icon="mdi-calendar"
												v-bind="attrs"
												v-on="on"
												:required="!Boolean(edit)"
												:rules="[rules.required]"
											></v-text-field>
										</template>
										<v-date-picker
											v-model="date"
											no-title
											@change="$refs.datePicker.save(date)"
										></v-date-picker>
									</v-menu>
								</v-col>
								<v-col cols="12" sm="4">
									<v-menu
										v-model="timeStartPickerActive"
										ref="timeStartPicker"
										:close-on-content-click="false"
									>
										<template v-slot:activator="{ on, attrs }">
											<v-text-field
												v-model="timeStart"
												label="Time Start"
												persistent-hint
												prepend-icon="mdi-calendar"
												v-bind="attrs"
												v-on="on"
												:required="!Boolean(edit)"
												:rules="[rules.required]"
											></v-text-field>
										</template>
										<v-time-picker
											v-if="timeStartPickerActive"
											v-model="timeStart"
											@click:minute="$refs.timeStartPicker.save(timeStart)"
											:max="timeEnd"
											format="24hr"
										></v-time-picker>
									</v-menu>
								</v-col>
								<v-col cols="12" sm="4">
									<v-menu
										v-model="timeEndPickerActive"
										ref="timeEndPicker"
										:close-on-content-click="false"
									>
										<template v-slot:activator="{ on, attrs }">
											<v-text-field
												v-model="timeEnd"
												label="Time End"
												persistent-hint
												prepend-icon="mdi-calendar"
												v-bind="attrs"
												v-on="on"
												:required="!Boolean(edit)"
												:rules="[rules.required]"
											></v-text-field>
										</template>
										<v-time-picker
											v-if="timeEndPickerActive"
											v-model="timeEnd"
											@click:minute="$refs.timeEndPicker.save(timeEnd)"
											:min="timeStart"
											format="24hr"
										></v-time-picker>
									</v-menu>
								</v-col>
								<v-col cols="12" sm="4">
									<v-text-field
										label="Duration"
										:required="!Boolean(edit)"
										v-model="duration"
										prepend-icon="mdi-card-account-details"
										readonly
									></v-text-field>
								</v-col>
							</v-row>
						</v-container>
					</v-card-text>
					<v-card-actions>
						<v-spacer></v-spacer>
						<v-btn
							color="primary"
							text
							@click="close"
						>
							Close
						</v-btn>
						<v-btn
							color="primary"
							text
							@click="save"
						>
							Save
						</v-btn>
					</v-card-actions>
				</v-card>
			</v-form>
		</v-dialog>
	</v-row>
</template>

<script lang="ts">
import { Vue, Component, Prop, Ref, Watch, Emit } from 'vue-property-decorator';
import Appointment from '~/interfaces/Appointment';
import Dentist from '~/interfaces/Dentist';
import Patient from '~/interfaces/Patient';
import Service from '~/interfaces/Service';

@Component
export default class AppointmentForm extends Vue {
	@Prop({default: false}) active!: boolean;
	@Prop({default: null}) item!: Appointment | null;
	@Prop({default: null}) selectedPatientId!: number | null;
	@Ref('form') form;
	private dentists: Dentist[] = []
	private patients: Patient[] = []
	private services: Service[] = []
	private datePickerActive = false;
	private timeStartPickerActive = false;
	private timeEndPickerActive = false;
	private date: string = '';
	private timeStart: string = '';
	private timeEnd: string = '';
	private duration: string = '';
	private appointment: Appointment = {
		dentistId: null,
		patientId: null,
		patientName: null,
		serviceId: null,
		serviceName: null,
		startDate: null,
		endDate: null,
		duration: null,
		canceled: false,
	}
	private rules = {
		required: (v: string) => this.edit || Boolean(v) || 'Required',
	};

	mounted() {
		this.refreshPatients();
		this.refreshServices();
	}

	@Emit('update:active')
	close() {
		this.form.resetValidation();
		return false;
	}

	@Emit('refresh')
	async save() {
		if (!this.form.validate()) return;
		const fetchOptions: RequestInit = {
			method: this.edit ? 'PUT' : 'POST',
			body: JSON.stringify(this.appointment),
			headers: {
					'Authorization': `Bearer ${this.$store.getters['auth/token']}`,
					'Content-Type': 'application/json'
				}
		}
		await fetch(`${process.env.APIURL}/Dentist/Appointment`, fetchOptions);
		this.close();
	}

	setUserIds() {
		this.appointment.dentistId = this.dentistId;
		this.appointment.patientId = this.patientId;
	}

	get edit() {
		return Boolean(this.item);
	}

	get role() {
		return this.$store.getters['auth/userRole'];
	}

	get id() {
		return this.$store.getters['auth/userId'];
	}

	get dentistId() {
		if (this.role === 'Dentist') return this.id;
		else return null;
	}

	get patientId() {
		if (this.role === 'Patient') return this.id;
		else return this.selectedPatientId || null;
	}

	@Watch('active')
	onDialogShow(active: boolean) {
		this.appointment = this.item && active ? {...this.item} : {
			dentistId: null,
			patientId: null,
			patientName: null,
			serviceId: null,
			serviceName: null,
			startDate: null,
			endDate: null,
			duration: null,
			canceled: false,
		}
		this.setUserIds();
	}

	@Watch('item')
	setDateTime(item: Appointment) {
		if (item) {
			this.date = item.startDate?.substr(0, 10) ?? '';
			this.timeStart = item.startDate?.substr(11, 5) ?? '';
			this.timeEnd = item.endDate?.substr(11, 5) ?? '';
		} else {
			this.date = '';
			this.timeStart = '';
			this.timeEnd = '';
			this.duration = '';
		}
	}

	@Watch('date')
	@Watch('timeStart')
	@Watch('timeEnd')
	calcDate() {
		if (this.date && this.timeStart) {
			this.appointment.startDate = new Date(`${this.date} ${this.timeStart}`).toISOString();
		}
		if (this.date && this.timeEnd) {
			this.appointment.endDate = new Date(`${this.date} ${this.timeEnd}`).toISOString();
		}
		if (this.timeStart && this.timeEnd) {
			const [timeStartHours, timeStartMinutes] = this.timeStart.split(':')
			const [timeEndHours, timeEndMinutes] = this.timeEnd.split(':')
			const start = new Date(0), end = new Date(0);
			start.setHours(Number(timeStartHours));
			start.setMinutes(Number(timeStartMinutes));
			end.setHours(Number(timeEndHours));
			end.setMinutes(Number(timeEndMinutes));
			const diff = end.getTime() - start.getTime();
			const diffHours = Math.floor(diff / 1000 / 60 / 60);
			const diffMinutes = Math.floor(diff / 1000 / 60 % 60);
			this.duration = `${diffHours < 10 ? '0' : ''}${diffHours}:${diffMinutes < 10 ? '0' : ''}${diffMinutes}`;
		}
	}

	async refreshServices() {
		let initData: RequestInit = {
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${this.$store.getters['auth/token']}`,
			}
		}
		this.services = await fetch(`${process.env.APIURL}/Dentist/Services`, initData).then(response => response.json());
	}

	async refreshPatients() {
		let initData: RequestInit = {
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${this.$store.getters['auth/token']}`,
			}
		}
		this.patients = await fetch(`${process.env.APIURL}/Dentist/Patients`, initData).then(response => response.json());
	}
}
</script>