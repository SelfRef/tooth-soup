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
				<v-btn @click="refreshData" color="blue">
					Refresh
					<v-icon right>mdi-refresh</v-icon>
				</v-btn>
				<v-btn @click="dialog = true" color="green">
					Add user
					<v-icon right>mdi-account-plus</v-icon>
				</v-btn>
			</v-col>
		</v-row>

		<v-simple-table>
			<thead>
				<tr>
					<th>Date</th>
					<th>Duration</th>
					<th>Service</th>
					<th>Actions</th>
				</tr>
			</thead>
			<tbody>
				<tr v-for="appointment in appointments" :key="appointment.id">
					<td>{{appointment.dateTime.replace('T', ' ').split('.')[0]}}</td>
					<td>{{appointment.duration}}</td>
					<td>[service]</td>
					<td>
						<v-btn icon color="blue" @click="edit(appointment)"><v-icon>mdi-calendar-plus</v-icon></v-btn>
						<v-menu :close-on-content-click="true">
							<template #activator="{on, attrs}">
								<v-btn
									v-on="on"
									v-bind="attrs"
									icon
									color="orange"
								><v-icon>mdi-calendar-minus</v-icon></v-btn>
							</template>
							<v-card>
								<v-card-text>Are you sure you want to {{ appointment.canceled ? 'uncancel' : 'cancel' }} this appointment?</v-card-text>
								<v-card-actions>
									<v-btn
										color="red"
										text
										@click="cancel(appointment.id, !appointment.canceled)"
									>
										{{ appointment.canceled ? 'Uncancel' : 'Cancel' }}
									</v-btn>
								</v-card-actions>
							</v-card>
						</v-menu>
						<v-menu :close-on-content-click="false">
							<template #activator="{on, attrs}">
								<v-btn
									v-on="on"
									v-bind="attrs"
									icon
									color="red"
								><v-icon>mdi-calendar-remove</v-icon></v-btn>
							</template>
							<v-card>
								<v-card-text>Are you sure you want to remove this appointment?</v-card-text>
								<v-card-actions>
									<v-btn color="red" text @click="remove(appointment.id)">Remove</v-btn>
								</v-card-actions>
							</v-card>
						</v-menu>
					</td>
				</tr>
			</tbody>
		</v-simple-table>
		<patient-edit-form :active.sync="dialog" :itemData="appointment" @refresh="refreshData"/>
	</v-container>
</template>

<script lang="ts">
import { Vue, Component, Prop, Watch } from 'vue-property-decorator';
import Appointment from '~/interfaces/Appointment';

@Component
export default class AppointmentList extends Vue {
	@Prop() patientId!: number;
	private appointments: Appointment[] = [];
	private appointment: Appointment | null = null;
	private dialog = false;
	//private searchText: string = '';
	//private searchTextTimeout: NodeJS.Timeout | null = null;

	async mounted() {
		await this.refreshData();
	}

	async refreshData() {
		let initData: RequestInit = {
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${this.$store.getters['auth/token']}`,
			}
		}
		//const filterQuery = this.searchText ? `?filter=${this.searchText}` : '';
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

	@Watch('dialog')
	resetPatient(dialog: boolean) {
		if (!dialog) this.appointment = null;
		//else if (this.appointment?.dateTime) this.appointment.dateTime = this.appointment.dateTime.split('T')[0];
	}

	// @Watch('searchText')
	// refreshSearch(userDialog: boolean) {
	// 	if (this.searchTextTimeout) clearTimeout(this.searchTextTimeout);
	// 	this.searchTextTimeout = setTimeout(() => this.refreshData(), 1000);
	// }
}
</script>
