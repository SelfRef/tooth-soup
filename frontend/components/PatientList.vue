<template>
	<v-container>
		<v-row>
			<v-col cols="12" lg="8" xl="6">
				<v-row>
					<v-col cols="auto">
						<h2>Patient List</h2>
					</v-col>
					<v-col>
						<v-text-field
							v-model="searchText"
							label="Search"
							clearable
							dense
						></v-text-field>
					</v-col>
					<v-col cols="12" sm="auto">
						<v-btn @click="refreshData" color="info">
							Refresh
							<v-icon right>mdi-refresh</v-icon>
						</v-btn>
						<v-btn @click="userDialog = true" color="success">
							Add user
							<v-icon right>mdi-account-plus</v-icon>
						</v-btn>
					</v-col>
				</v-row>
				<v-data-table
					:headers="headers"
					:items="$store.getters[`${this.role}/patients`]"
					show-select
					single-select
					v-model="selectedPatients"
					:search="searchText"
				>
					<template #item.birthDate="{value}">{{value | dateTime}}</template>
					<template #item.actions="{item}">
						<v-tooltip bottom>
							Edit patient
							<template #activator="{on, attrs}">
								<v-btn v-on="on" v-bind="attrs" icon color="info" @click="editPatient(item)"><v-icon>mdi-account-edit</v-icon></v-btn>
							</template>
						</v-tooltip>
						<v-menu :close-on-content-click="false">
							<template #activator="{on: onMenu}">
								<v-tooltip bottom>
									Unlink patient
									<template #activator="{on: onTip}">
										<v-btn
											v-on="{...onTip, ...onMenu}"
											icon
											color="warning"
										><v-icon>mdi-account-minus</v-icon></v-btn>
									</template>
								</v-tooltip>
							</template>
							<v-card>
								<v-card-text>Are you sure you want to unlink this user?</v-card-text>
								<v-card-actions>
									<v-btn color="error" text @click="unlinkPatient(item.id)">Unlink</v-btn>
								</v-card-actions>
							</v-card>
						</v-menu>
					</template>
				</v-data-table>
				<patient-form :active.sync="userDialog" :patientData="patient" @refresh="refreshData"/>
				<appointment-list :patientId="selectedPatientId" />
			</v-col>
			<v-col>
				<appointment-calendar />
			</v-col>
		</v-row>
	</v-container>
</template>

<script lang='ts'>
	import { Vue, Component, Watch } from 'vue-property-decorator';
	import Patient from 'interfaces/Patient';
	import PatientForm from '~/components/PatientForm.vue';
	import AppointmentList from '~/components/AppointmentList.vue';
	import AppointmentCalendar from '~/components/AppointmentCalendar.vue';
	@Component({
		components: {
			PatientForm,
			AppointmentList,
			AppointmentCalendar,
		},
		filters: {
			duration(value: string) {
				const parts = value.split(':');
				return `${Number(parts[0])}h ${parts[1]}m`;
			},
			dateTime(value: string) {
				return value.substr(0, 10);
			}
		}
	})
	export default class PatientList extends Vue {
		private selectedPatients: Patient[] = [];
		private userDialog = false;
		private patient: Patient | null = null;
		private searchText: string = '';
		private headers = [
			{
				text: 'PESEL',
				value: 'pesel',
			},
			{
				text: 'First Name',
				value: 'firstName',
			},
			{
				text: 'Last Name',
				value: 'lastName',
			},
			{
				text: 'Birth Date',
				value: 'birthDate',
			},
			{
				text: 'Email',
				value: 'email',
			},
			{
				text: 'Actions',
				value: 'actions',
				width: 110,
			},
		];

		get role() {
			return this.$store.getters['Auth/userRole'];
		}

		async mounted() {
			await this.refreshData();
		}

		async refreshData() {
			this.$store.dispatch(`${this.role}/updatePatients`);
		}

		editPatient(patient: Patient) {
			this.patient = patient;
			this.userDialog = true;
		}

		async unlinkPatient(id: number) {
			let initData: RequestInit = {
				method: 'PUT',
				headers: {
					'Authorization': `Bearer ${this.$store.getters['Auth/token']}`,
				}
			}
			await fetch(`${process.env.APIURL}/${this.role}/Patients/${id}/Unlink`, initData);
			await this.refreshData();
		}

		async removePatient(id: number) {
			let initData: RequestInit = {
				method: 'DELETE',
				headers: {
					'Authorization': `Bearer ${this.$store.getters['Auth/token']}`,
				}
			}
			await fetch(`${process.env.APIURL}/${this.role}/Patients/${id}`, initData);
			await this.refreshData();
		}

		get selectedPatientId() {
			if (this.selectedPatients.length === 1) {
				return this.selectedPatients[0].id;
			} else {
				return null;
			}
		}

		@Watch('userDialog')
		resetPatient(userDialog: boolean) {
			if (!userDialog) this.patient = null;
			else if (this.patient?.birthDate) this.patient.birthDate = this.patient.birthDate.split('T')[0];
		}
	}
</script>