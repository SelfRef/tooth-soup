<template>
	<v-row justify="center">
		<v-dialog
			v-if="active"
			v-model="active"
			persistent
			max-width="1000px"
		>
			<v-form ref="form">
				<v-card>
					<v-card-title>
						<span class="headline">{{ edit ? 'Edit' : 'Create' }} Appointment</span>
					</v-card-title>
					<v-card-text>
						<v-container>
							<v-row>
								<v-col>
									<v-row v-if="role === 'Patient'">
										<v-col cols="12">
											<v-select
												:items="dentists.filter(d => d.canCreateAppointment)"
												:item-text="u => `${u.firstName} ${u.lastName}`"
												item-value="id"
												v-model="appointment.dentistId"
												prepend-icon="mdi-account"
												label="Dentist"
												:readonly="role === 'Patient' && edit"
											></v-select>
										</v-col>
									</v-row>
									<v-row v-if="role === 'Dentist'">
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
													:min="new Date().toISOString().substr(0, 10)"
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
									<v-row v-if="!edit && isCollision">
										<v-col>
											<v-alert type="warning">Appointment may collide</v-alert>
										</v-col>
									</v-row>
								</v-col>
								<v-col>
									<v-sheet height="400">
										<v-calendar
											type="day"
											hide-header
											:value="date"
											:events="items"
											:event-color="e => e.color"
											:event-name="getName"
											:first-interval="firstHour"
											:interval-count="hoursCount"
										/>
									</v-sheet>
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
	private dentistAppointments: Appointment[] = []
	private datePickerActive = false;
	private timeStartPickerActive = false;
	private timeEndPickerActive = false;
	private date: string = '';
	private timeStart: string = '';
	private timeEnd: string = '';
	private duration: string = '';
	private appointment: Appointment = {
		dentistId: null,
		dentistName: null,
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

	get role() {
		return this.$store.getters['Auth/userRole'];
	}

	get items() {
		const appointments = this.dentistAppointments.map(a => ({
			name: this.eventName(a),
			start: this.formatDate(a.startDate),
			end: this.formatDate(a.endDate),
			color: !a.serviceName ? 'secondary' : (a.canceled ? 'error' : 'primary'),
			canceled: a.canceled,
		}));

		if (this.date && this.timeStart && this.timeEnd) {
			appointments.push({
				name: this.edit ? 'After change' : 'New appointment',
				start: `${this.date} ${this.timeStart}`,
				end: `${this.date} ${this.timeEnd}`,
				color: 'success',
				canceled: false,
			})
		}

		return appointments;
	}

	getName(item) {
		if (item.input.canceled) return `❌ ${item.input.name}`;
		return item.input.name
	}

	mounted() {
		if (this.role === 'Dentist') this.refreshPatients();
		if (this.role === 'Patient') {
			this.refreshDentists();
			this.$store.dispatch(`${this.role}/updateAccount`);
		}
		this.refreshServices();
		this.updateAppointments();
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
					'Authorization': `Bearer ${this.$store.getters['Auth/token']}`,
					'Content-Type': 'application/json'
				}
		}
		await fetch(`${process.env.APIURL}/${this.role}/Appointments`, fetchOptions);
		this.form.reset();
		this.close();
	}

	setUserIds() {
		if (this.role === 'Patient') {
			this.appointment.dentistId = this.account?.dentistId;
			this.appointment.patientId = this.id;
		} else if (this.role === 'Dentist') {
			this.appointment.dentistId = this.dentistId;
			this.appointment.patientId = this.patientId;
		}
	}

	get edit() {
		return Boolean(this.item);
	}

	get account() {
		return this.$store.getters[`${this.role}/account`];
	}

	get id() {
		return this.$store.getters['Auth/userId'];
	}

	get dentistId() {
		if (this.role === 'Dentist') return this.id;
		else return this.appointment.dentistId;
	}

	get patientId() {
		if (this.role === 'Patient') return this.id;
		else return this.selectedPatientId || null;
	}

	get isCollision() {
		if (!this.date || !this.timeStart || !this.timeEnd) return false;
		const start = new Date(`${this.date}T${this.timeStart}`);
		const end = new Date(`${this.date}T${this.timeEnd}`);
		console.log('new', start, end);
		return this.dentistAppointments.some(a => {
			const aStart = new Date(a.startDate);
			const aEnd = new Date(a.endDate);
			console.log('old', aStart, aEnd);
			return (start > aStart && start < aEnd) || (end > aStart && end < aEnd) || (aStart > start && aEnd < end);
		});
	}

	get firstHour(): number {
		let first = 8;
		this.items.filter(i => {
			return i.start.substr(0, 10) === this.date;
		}).forEach(i => {
			const start = Number(i.start.substr(11, 2));
			if (start < first) first = start;
		});
		return first;
	}

	get hoursCount(): number {
		let last = 16;
		this.items.filter(i => {
			return i.start.substr(0, 10) === this.date;
		}).forEach(i => {
			const end = Number(i.end.substr(11, 2));
			if (end > last) last = end;
		});
		return ++last - this.firstHour;
	}

	@Watch('active')
	onDialogShow(active: boolean) {
		this.appointment = this.item && active ? {...this.item} : {
			dentistId: null,
			dentistName: null,
			patientId: null,
			patientName: null,
			serviceId: null,
			serviceName: null,
			startDate: null,
			endDate: null,
			duration: null,
			canceled: false,
		}
		if (active && !this.item) this.date = new Date().toISOString().substr(0, 10);
		this.setUserIds();
		this.updateAppointments()
	}

	@Watch('item')
	setDateTime(item: Appointment) {
		if (item) {
			this.date = item.startDate?.substr(0, 10) ?? '';
			this.timeStart = item.startDate?.substr(11, 5) ?? '';
			this.timeEnd = item.endDate?.substr(11, 5) ?? '';
		} else {
			this.date = new Date().toISOString().substr(0, 10);
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
			this.appointment.startDate = new Date(`${this.date}T${this.timeStart}Z`).toISOString();
		}
		if (this.date && this.timeEnd) {
			this.appointment.endDate = new Date(`${this.date}T${this.timeEnd}Z`).toISOString();
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

	@Watch('date')
	@Watch('dentistId')
	async updateAppointments() {
		if (this.date && this.dentistId) {
			if (this.role === 'Patient') {
				let initData: RequestInit = {
					method: 'GET',
					headers: {
						'Authorization': `Bearer ${this.$store.getters['Auth/token']}`,
					}
				}
				this.dentistAppointments = await fetch(`${process.env.APIURL}/${this.role}/Appointments/Dentists/${this.dentistId}/${this.date}`, initData).then(response => response.json());
			}
		} else if (this.role === 'Dentist') {
			this.dentistAppointments = this.$store.getters[`${this.role}/appointments`];
		}
	}

	async refreshServices() {
		let initData: RequestInit = {
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${this.$store.getters['Auth/token']}`,
			}
		}
		this.services = await fetch(`${process.env.APIURL}/${this.role}/Services`, initData).then(response => response.json());
	}

	async refreshPatients() {
		let initData: RequestInit = {
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${this.$store.getters['Auth/token']}`,
			}
		}
		this.patients = await fetch(`${process.env.APIURL}/${this.role}/Patients`, initData).then(response => response.json());
	}

	async refreshDentists() {
		let initData: RequestInit = {
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${this.$store.getters['Auth/token']}`,
			}
		}
		this.dentists = await fetch(`${process.env.APIURL}/${this.role}/Dentists`, initData).then(response => response.json());
	}

	formatDate(dateIso: string) {
		return dateIso.substr(0, 16).replace('T', ' ');
	}

	eventName(appointment: Appointment) {
		if (appointment.serviceName) {
			let name;
			if (this.role === 'Patient') name = appointment.dentistName;
			else if (this.role === 'Dentist') name = appointment.patientName;
			return `${appointment.serviceName} (${name})`;
		} else {
			return 'Busy';
		}
	}
}
</script>