<template>
	<v-container class="px-0">
		<v-row>
			<v-col cols="auto">
				<h2>Appointment List</h2>
			</v-col>
			<v-col cols="auto">
				<div>
					<v-tooltip bottom :open-delay="500">
						Refresh
						<template #activator="{on}">
							<v-btn icon @click="refreshData" color="secondary" v-on="on" :disabled="disableActions">
								<v-icon>mdi-refresh</v-icon>
							</v-btn>
						</template>
					</v-tooltip>
					<v-tooltip bottom :open-delay="500">
						Export appointments to XML
						<template #activator="{on}">
							<v-btn
								icon
								@click="$store.dispatch(`${role}/exportAppointmentsXml`, {id: patientId})"
								color="teal"
								v-on="on"
								:disabled="emptyList"
							>
								<v-icon>mdi-file-code</v-icon>
							</v-btn>
						</template>
					</v-tooltip>
					<v-tooltip bottom :open-delay="500">
						Export appointments to PDF
						<template #activator="{on}">
							<v-btn
								icon
								@click="$store.dispatch(`${role}/exportAppointmentsPdf`, {id: patientId})"
								color="orange"
								v-on="on"
								:disabled="emptyList"
							>
								<v-icon>mdi-file-pdf</v-icon>
							</v-btn>
						</template>
					</v-tooltip>
				</div>
			</v-col>
			<v-spacer></v-spacer>
			<v-col cols="auto">
				<v-switch
					hide-details
					height="0"
					v-model="showPast"
					label="Show past"
					:disabled="emptyList"
				/>
			</v-col>
			<v-col cols="auto">
				<v-btn @click="dialog = true" color="success" :disabled="disableActions">
					Add appointment
					<v-icon right>mdi-calendar-plus</v-icon>
				</v-btn>
			</v-col>
		</v-row>

		<v-data-table
			:headers="headers"
			:items="filteredAppointments"
			:item-class="canceledRow"
			sort-by="startDate"
			sort-desc
		>
			<template #item.startDate="{value}">{{value | dateTime}}</template>
			<template #item.duration="{value}">{{value | duration}}</template>
			<template #item.canceled="{item, value}">
				<v-menu>
					<template #activator="{on}">
						<v-simple-checkbox readonly v-on="on" :value="value" :disabled="!canEditOrCancel(item)" />
					</template>
					<v-card v-if="role === 'Dentist' || (!value && canEditOrCancel(item))">
						<v-card-text>Are you sure you want to {{ item.canceled ? 'uncancel' : 'cancel' }} this appointment?</v-card-text>
						<v-card-actions>
							<v-btn
								color="error"
								text
								@click="cancel(item.id, !item.canceled)"
							>
								{{ item.canceled ? 'Uncancel' : 'Cancel' }}
							</v-btn>
						</v-card-actions>
					</v-card>
				</v-menu>
			</template>
			<template #item.actions="{item}">
				<v-tooltip bottom :open-delay="500">
					Export invoice to PDF
					<template #activator="{on}">
						<v-btn
							icon
							@click="$store.dispatch(`${role}/exportAppointmentsPdf`, {id: patientId, appointment: item})"
							color="orange"
							v-on="on"
							:disabled="!pastAppointment(item)"
						>
							<v-icon>mdi-file-pdf</v-icon>
						</v-btn>
					</template>
				</v-tooltip>
				<v-tooltip bottom :open-delay="500">
					Edit appointment
					<template #activator="{on, attrs}">
						<v-btn
							:disabled="item.canceled || !canEditOrCancel(item)"
							v-on="on"
							v-bind="attrs"
							icon
							color="info"
							@click="edit(item)"
						>
							<v-icon>mdi-calendar-edit</v-icon>
						</v-btn>
					</template>
				</v-tooltip>
				<v-menu v-if="role === 'Dentist'">
					<template #activator="{on: onMenu}">
						<v-tooltip bottom :open-delay="500">
							Remove appointment
							<template #activator="{on: onTip}">
								<v-btn
									v-on="{...onTip, ...onMenu}"
									icon
									color="error"
								><v-icon>mdi-calendar-remove</v-icon></v-btn>
							</template>
						</v-tooltip>
					</template>
					<v-card>
						<v-card-text>Are you sure you want to remove this appointment?</v-card-text>
						<v-card-actions>
							<v-btn color="error" text @click="remove(item.id)">Remove</v-btn>
						</v-card-actions>
					</v-card>
				</v-menu>
			</template>
			<template #no-data v-if="patientId">No appointments available for this patient.</template>
			<template #no-data v-else>Select patient to show appointments.</template>
		</v-data-table>
		<appointment-form :active.sync="dialog" :item="appointment" @refresh="refreshData" :selectedPatientId="patientId"/>
	</v-container>
</template>

<script lang="ts">
import { Vue, Component, Prop, Watch, Emit } from 'vue-property-decorator';
import Appointment from '~/interfaces/Appointment';
import AppointmentForm from '~/components/AppointmentForm.vue';

@Component({
	components: {
		AppointmentForm,
	},
	filters: {
		duration(value: string) {
			const parts = value.split(':');
			return `${Number(parts[0])}h ${parts[1]}m`;
		},
		dateTime(value: string) {
			return value.substr(0, 16).replace('T', ' ');
		}
	}
})
export default class AppointmentList extends Vue {
	@Prop({default: null}) patientId!: number | null;
	private appointments: Appointment[] = [];
	private appointment: Appointment | null = null;
	private dialog = false;
	private showPast = false;
	private headers = [
		{
			text: 'Date',
			value: 'startDate',
		},
		{
			text: 'Duration',
			value: 'duration',
		},
		{
			text: 'Service',
			value: 'serviceName',
		},
		{
			text: 'Canceled',
			value: 'canceled',
		},
		{
			text: 'Actions',
			value: 'actions',
			width: this.role === 'Dentist' ? 150 : null,
		},
	];

	get filteredAppointments() {
		let data;
		if (this.role === 'Patient') {
			data = this.$store.getters[`${this.role}/appointments`]
		} else {
			data = this.appointments;
		}
		return data.filter(a => this.showPast || (!this.pastAppointment(a)));
	}

	get role() {
		return this.$store.getters['Auth/userRole'];
	}

	get emptyList() {
		return this.filteredAppointments.length === 0 || (!this.patientId && this.role !== 'Patient');
	}

	get disableActions() {
		return !this.patientId && this.role !== 'Patient';
	}

	async mounted() {
		await this.refreshData();
		if (this.role === 'Patient') {
			this.headers.splice(2, 0, {
				text: 'Dentist',
				value: 'dentistName',
			},)
		}
	}

	edit(appointment: Appointment) {
		this.appointment = appointment;
		this.dialog = true;
	}

	async cancel(id: number, cancel: boolean) {
		let initData: RequestInit = {
			method: 'PUT',
			headers: {
				'Authorization': `Bearer ${this.$store.getters['Auth/token']}`,
				'Content-Type': 'application/json',
			},
			body: JSON.stringify({
				id,
				canceled: cancel,
			})
		}
		await fetch(`${process.env.APIURL}/${this.role}/Appointments`, initData);
		await this.refreshData();
	}

	async remove(id: number) {
		let initData: RequestInit = {
			method: 'DELETE',
			headers: {
				'Authorization': `Bearer ${this.$store.getters['Auth/token']}`,
			}
		}
		await fetch(`${process.env.APIURL}/${this.role}/Appointments/${id}`, initData);
		await this.refreshData();
	}

	canceledRow(item: Appointment) {
		if (new Date(item.endDate) < new Date()) {
			return this.$vuetify.theme.dark ? 'black' : 'grey lighten-2'
		} else if (item.canceled) {
			return this.$vuetify.theme.dark ? 'brown darken-4' : 'red lighten-5'
		}
		return '';
	}

	canEditOrCancel(appointment: Appointment) {
		if (this.role === 'Patient') {
			return new Date(appointment.startDate) > new Date();
		} else return true;
	}

	pastAppointment(appointment: Appointment) {
		return new Date(appointment.endDate) < new Date();
	}

	@Emit('refreshData')
	async refreshData() {
		if (this.role === 'Patient') {
			this.$store.dispatch(`${this.role}/updateAppointments`);
		}
		if (this.role === 'Dentist') {
			this.$store.dispatch(`${this.role}/updateAppointments`);
			if (!this.patientId) {
				this.appointments = [];
				return;
			}
			let initData: RequestInit = {
				method: 'GET',
				headers: {
					'Authorization': `Bearer ${this.$store.getters['Auth/token']}`,
				}
			}
			this.appointments = await fetch(`${process.env.APIURL}/${this.role}/Appointments/Patients/${this.patientId}`, initData).then(response => response.json());
		}
	}

	@Watch('dialog')
	resetPatient(dialog: boolean) {
		if (!dialog) this.appointment = null;
	}

	@Watch('patientId')
	patientIdChanged() {
		this.refreshData();
	}
}
</script>
