<template>
	<v-container>
		<v-row>
			<v-col>
				<!-- <h2>Appointment List {{ searchText ? '(filtered)' : '' }}</h2> -->
				<h2>Appointment List</h2>
			</v-col>
			<v-spacer></v-spacer>
			<!-- <v-col>
				<v-text-field
					v-model="searchText"
					label="Search"
					hint="(PESEL, Name, Email)"
					clearable
					dense
				></v-text-field>
			</v-col> -->
			<v-col cols="auto">
				<v-btn @click="refreshData" color="blue" :disabled="!patientId">
					Refresh
					<v-icon right>mdi-refresh</v-icon>
				</v-btn>
				<v-btn @click="dialog = true" color="green" :disabled="!patientId">
					Add appointment
					<v-icon right>mdi-calendar-plus</v-icon>
				</v-btn>
			</v-col>
		</v-row>

		<v-data-table
			:headers="headers"
			:items="appointments"
			:item-class="canceledRow"
		>
			<template #item.dateTime="{value}">{{value | dateTime}}</template>
			<template #item.duration="{value}">{{value | duration}}</template>
			<template #item.actions="{item}">
				<v-tooltip bottom>
					Edit appointment
					<template #activator="{on, attrs}">
						<v-btn v-on="on" v-bind="attrs" icon color="blue" @click="edit(item)"><v-icon>mdi-calendar-edit</v-icon></v-btn>
					</template>
				</v-tooltip>
				<v-menu :close-on-content-click="true">
					<template #activator="{on: onMenu}">
						<v-tooltip bottom>
							Cancel appointment
							<template #activator="{on: onTip}">
								<v-btn
									v-on="{...onTip, ...onMenu}"
									icon
									color="orange"
								><v-icon>mdi-calendar-minus</v-icon></v-btn>
							</template>
						</v-tooltip>
					</template>
					<v-card>
						<v-card-text>Are you sure you want to {{ item.canceled ? 'uncancel' : 'cancel' }} this appointment?</v-card-text>
						<v-card-actions>
							<v-btn
								color="red"
								text
								@click="cancel(item.id, !item.canceled)"
							>
								{{ item.canceled ? 'Uncancel' : 'Cancel' }}
							</v-btn>
						</v-card-actions>
					</v-card>
				</v-menu>
				<v-menu :close-on-content-click="false">
					<template #activator="{on: onMenu}">
						<v-tooltip bottom>
							Remove appointment
							<template #activator="{on: onTip}">
								<v-btn
									v-on="{...onTip, ...onMenu}"
									icon
									color="red"
								><v-icon>mdi-calendar-remove</v-icon></v-btn>
								</template>
							</v-tooltip>
					</template>
					<v-card>
						<v-card-text>Are you sure you want to remove this appointment?</v-card-text>
						<v-card-actions>
							<v-btn color="red" text @click="remove(item.id)">Remove</v-btn>
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
import { Vue, Component, Prop, Watch } from 'vue-property-decorator';
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
			return value.replace('T', ' ').split('.')[0];
		}
	}
})
export default class AppointmentList extends Vue {
	@Prop({default: null}) patientId!: number | null;
	private appointments: Appointment[] = [];
	private appointment: Appointment | null = null;
	private dialog = false;
	private headers = [
			{
				text: 'Date',
				value: 'dateTime',
			},
			{
				text: 'Duration',
				value: 'duration',
			},
			{
				text: 'Service',
				value: 'service',
			},
			{
				text: 'Actions',
				value: 'actions',
				width: 150,
			},
		];

	async mounted() {
		await this.refreshData();
	}

	async refreshData() {
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
		await fetch(`${process.env.APIURL}/Dentist/Appointment`, initData);
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
		return item.canceled ? 'red lighten-5' : '';
	}

	@Watch('dialog')
	resetPatient(dialog: boolean) {
		if (!dialog) this.appointment = null;
		//else if (this.appointment?.dateTime) this.appointment.dateTime = this.appointment.dateTime.split('T')[0];
	}

	@Watch('patientId')
	patientIdChanged() {
		this.refreshData();
	}

	// @Watch('searchText')
	// refreshSearch(userDialog: boolean) {
	// 	if (this.searchTextTimeout) clearTimeout(this.searchTextTimeout);
	// 	this.searchTextTimeout = setTimeout(() => this.refreshData(), 1000);
	// }
}
</script>
