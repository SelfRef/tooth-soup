<template>
	<v-layout column v-if="isLoggedIn">
		<v-container>
			<v-row>
				<v-col>
					<h2>Patient List {{ searchText ? '(filtered)' : '' }}</h2>
				</v-col>
				<v-spacer></v-spacer>
				<v-col>
					<v-text-field
						v-model="searchText"
						label="Search"
						hint="(PESEL, Name, Email)"
						clearable
						dense
					></v-text-field>
				</v-col>
				<v-col cols="auto">
					<v-btn @click="refreshData" color="blue">
						Refresh
						<v-icon right>mdi-refresh</v-icon>
					</v-btn>
					<v-btn @click="userDialog = true" color="green">
						Add user
						<v-icon right>mdi-account-plus</v-icon>
					</v-btn>
				</v-col>
			</v-row>

			<v-data-table
				:headers="headers"
				:items="patients"
				show-select
				single-select
				v-model="selectedPatients"
			>
				<template #item.birthDate="{value}">{{value | dateTime}}</template>
				<template #item.actions="{item}">
					<v-tooltip bottom>
						Edit patient
						<template #activator="{on, attrs}">
							<v-btn v-on="on" v-bind="attrs" icon color="blue" @click="editPatient(item)"><v-icon>mdi-account-edit</v-icon></v-btn>
						</template>
					</v-tooltip>
					<v-menu :close-on-content-click="false">
						<template #activator="{on, attrs}">
							<v-tooltip bottom v-on="on" v-bind="attrs">
								Unlink patient
								<template #activator="{on, attrs}">
									<v-btn
										v-on="on"
										v-bind="attrs"
										icon
										color="orange"
									><v-icon>mdi-account-minus</v-icon></v-btn>
								</template>
							</v-tooltip>
						</template>
						<v-card>
							<v-card-text>Are you sure you want to unlink this user?</v-card-text>
							<v-card-actions>
								<v-btn color="red" text @click="unlinkPatient(item.id)">Unlink</v-btn>
							</v-card-actions>
						</v-card>
					</v-menu>
					<v-menu :close-on-content-click="false">
						<template #activator="{on, attrs}">
							<v-tooltip bottom v-on="on" v-bind="attrs">
								Remove patient
								<template #activator="{on, attrs}">
									<v-btn
										v-on="on"
										v-bind="attrs"
										icon
										color="red"
									><v-icon>mdi-account-remove</v-icon></v-btn>
								</template>
							</v-tooltip>
						</template>
						<v-card>
							<v-card-text>Are you sure you want to remove this user?</v-card-text>
							<v-card-actions>
								<v-btn color="red" text @click="removePatient(item.id)">Remove</v-btn>
							</v-card-actions>
						</v-card>
					</v-menu>
				</template>
			</v-data-table>
			<patient-edit-form :active.sync="userDialog" :patientData="patient" @refresh="refreshData"/>
		</v-container>
		<appointment-list :patientId="selectedPatientId" />
	</v-layout>
</template>

<script lang='ts'>
	import { Vue, Component, Watch } from 'vue-property-decorator';
	import Patient from 'interfaces/Patient';
	import PatientEditForm from '~/components/PatientEditForm.vue';
	import AppointmentList from '~/components/AppointmentList.vue';
	@Component({
		components: {
			PatientEditForm,
			AppointmentList,
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
	export default class PatientList extends Vue {
		private patients: Patient[] = [];
		private selectedPatients: Patient[] = [];
		private userDialog = false;
		private patient: Patient | null = null;
		private searchText: string = '';
		private searchTextTimeout: NodeJS.Timeout | null = null;
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
				width: 150,
			},
		];

		async mounted() {
			if (!this.isLoggedIn) {
				return this.$router.replace('/');
			}
			await this.refreshData();
		}

		async refreshData() {
			let initData: RequestInit = {
				method: 'GET',
				headers: {
					'Authorization': `Bearer ${this.$store.getters['auth/token']}`,
				}
			}
			const filterQuery = this.searchText ? `?filter=${this.searchText}` : '';
			this.patients = await fetch(`${process.env.APIURL}/Dentist/Patients${filterQuery}`, initData).then(response => response.json());
		}

		editPatient(patient: Patient) {
			this.patient = patient;
			this.userDialog = true;
		}

		async unlinkPatient(id: number) {
			let initData: RequestInit = {
				method: 'GET',
				headers: {
					'Authorization': `Bearer ${this.$store.getters['auth/token']}`,
				}
			}
			await fetch(`${process.env.APIURL}/Dentist/Patient/${id}/Unlink`, initData);
			await this.refreshData();
		}

		async removePatient(id: number) {
			let initData: RequestInit = {
				method: 'DELETE',
				headers: {
					'Authorization': `Bearer ${this.$store.getters['auth/token']}`,
				}
			}
			await fetch(`${process.env.APIURL}/Dentist/Patient/${id}`, initData);
			await this.refreshData();
		}

		get isLoggedIn() {
			return this.$store.getters['auth/isLoggedIn'];
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

		@Watch('searchText')
		refreshSearch(userDialog: boolean) {
			if (this.searchTextTimeout) clearTimeout(this.searchTextTimeout);
			this.searchTextTimeout = setTimeout(() => this.refreshData(), 1000);
		}
	}
</script>