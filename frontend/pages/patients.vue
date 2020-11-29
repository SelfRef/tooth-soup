<template>
	<v-layout>
		<v-container>
			<v-row>
				<v-col>
					<h2>Patient List</h2>
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
					<v-btn @click="refreshData">
						Refresh
						<v-icon right>mdi-refresh</v-icon>
					</v-btn>
					<v-btn @click="userDialog = true">
						Add user
						<v-icon right>mdi-account-plus</v-icon>
					</v-btn>
				</v-col>
			</v-row>

			<v-simple-table>
				<thead>
					<tr>
						<th>PESEL</th>
						<th>First Name</th>
						<th>Last Name</th>
						<th>Birth Date</th>
						<th>Email</th>
						<th>Actions</th>
					</tr>
				</thead>
				<tbody>
					<tr v-for="patient in patients" :key="patient.id">
						<td>{{patient.pesel}}</td>
						<td>{{patient.firstName}}</td>
						<td>{{patient.lastName}}</td>
						<td>{{patient.birthDate.split('T')[0]}}</td>
						<td>{{patient.email}}</td>
						<td>
							<v-btn icon color="blue" @click="editPatient(patient)"><v-icon>mdi-account-edit</v-icon></v-btn>
							<v-menu :close-on-content-click="false">
								<template #activator="{on, attrs}">
									<v-btn
										v-on="on"
										v-bind="attrs"
										icon
										color="orange"
									><v-icon>mdi-account-minus</v-icon></v-btn>
								</template>
								<v-card>
									<v-card-text>Are you sure you want to unlink this user?</v-card-text>
									<v-card-actions>
										<v-btn color="red" text @click="unlinkPatient(patient.id)">Unlink</v-btn>
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
									><v-icon>mdi-account-remove</v-icon></v-btn>
								</template>
								<v-card>
									<v-card-text>Are you sure you want to remove this user?</v-card-text>
									<v-card-actions>
										<v-btn color="red" text @click="removePatient(patient.id)">Remove</v-btn>
									</v-card-actions>
								</v-card>
							</v-menu>
						</td>
					</tr>
				</tbody>
			</v-simple-table>
		</v-container>
		<patient-edit-form :active.sync="userDialog" :patientData="patient" @refresh="refreshData"/>
	</v-layout>
</template>

<script lang='ts'>
	import { Vue, Component, Watch } from 'vue-property-decorator';
	import Patient from 'interfaces/Patient';
	import PatientEditForm from '~/components/PatientEditForm.vue';
	@Component({
		components: {
			PatientEditForm,
		}
	})
	export default class Patients extends Vue {
		private patients: Patient[] = [];
		private userDialog = false;
		private patient: Patient | null = null;
		private searchText: string = '';

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
			this.patients = await fetch(`${process.env.APIURL}/Dentist/Patients`, initData).then(response => response.json());
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

		@Watch('userDialog')
		resetPatient(userDialog: boolean) {
			if (!userDialog) this.patient = null;
			else if (this.patient?.birthDate) this.patient.birthDate = this.patient.birthDate.split('T')[0];
		}
	}
</script>