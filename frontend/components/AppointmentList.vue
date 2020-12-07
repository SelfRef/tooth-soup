<template>
	<v-container class="px-0">
		<v-row>
			<v-col cols="auto">
				<h2>Appointment List</h2>
			</v-col>
			<v-spacer></v-spacer>
			<v-col cols="auto">
				<v-switch
					hide-details
					height="0"
					v-model="activeOnly"
					label="Active only"
					:disabled="!patientId && role !== 'Patient'"
				/>
			</v-col>
			<v-col cols="auto">
				<v-btn @click="refreshData" color="info" :disabled="!patientId && role !== 'Patient'">
					Refresh
					<v-icon right>mdi-refresh</v-icon>
				</v-btn>
				<v-btn @click="dialog = true" color="success" :disabled="!patientId && role !== 'Patient'">
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
				<v-menu :close-on-content-click="true">
					<template #activator="{on}">
						<v-simple-checkbox readonly v-on="on" :value="value" />
					</template>
					<v-card v-if="role === 'Dentist' || !value">
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
				<v-tooltip bottom>
					Edit appointment
					<template #activator="{on, attrs}">
						<v-btn :disabled="item.canceled" v-on="on" v-bind="attrs" icon color="info" @click="edit(item)"><v-icon>mdi-calendar-edit</v-icon></v-btn>
					</template>
				</v-tooltip>
				<v-menu :close-on-content-click="false" v-if="role === 'Dentist'">
					<template #activator="{on: onMenu}">
						<v-tooltip bottom>
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
	private activeOnly = false;
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
			width: this.role === 'Dentist' ? 110 : null,
		},
	];

	get filteredAppointments() {
		if (this.role === 'Patient') {
			return this.$store.getters['patient/appointments'].filter(a => !this.activeOnly || !a.canceled);
		} else {
			return this.appointments.filter(a => !this.activeOnly || !a.canceled);
		}
	}

	get role() {
		return this.$store.getters['auth/userRole'];
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

	@Emit('refreshData')
	async refreshData() {
		if (this.role === 'Patient') {
			this.$store.dispatch('patient/updateAppointments');
		}
		if (this.role === 'Dentist') {
			this.$store.dispatch('dentist/updateAppointments');
			if (!this.patientId) {
				this.appointments = [];
				return;
			}
			let initData: RequestInit = {
				method: 'GET',
				headers: {
					'Authorization': `Bearer ${this.$store.getters['auth/token']}`,
				}
			}
			this.appointments = await fetch(`${process.env.APIURL}/Dentist/Appointments/Patient/${this.patientId}`, initData).then(response => response.json());
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
				'Authorization': `Bearer ${this.$store.getters['auth/token']}`,
				'Content-Type': 'application/json',
			},
			body: JSON.stringify({
				id,
				canceled: cancel,
			})
		}
		await fetch(`${process.env.APIURL}/${this.role}/Appointment`, initData);
		await this.refreshData();
	}

	async remove(id: number) {
		let initData: RequestInit = {
			method: 'DELETE',
			headers: {
				'Authorization': `Bearer ${this.$store.getters['auth/token']}`,
			}
		}
		await fetch(`${process.env.APIURL}/Dentist/Appointment/${id}`, initData);
		await this.refreshData();
	}

	canceledRow(item: Appointment) {
		let colorClass = '';
		if (item.canceled) {
			colorClass += 'red '
			colorClass += this.$vuetify.theme.dark ? 'darken-4' : 'lighten-5'
		}
		return colorClass;
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
