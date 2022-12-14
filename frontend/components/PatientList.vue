<template>
	<v-container>
		<v-row>
			<v-col cols="12" lg="8" xl="6">
				<v-row>
					<v-col cols="auto">
						<h2>Patient List</h2>
					</v-col>
					<v-col cols="auto">
						<v-tooltip bottom :open-delay="500">
							Refresh
							<template #activator="{on}">
								<v-btn icon @click="refreshData" color="secondary" v-on="on">
									<v-icon>mdi-refresh</v-icon>
								</v-btn>
							</template>
						</v-tooltip>
					</v-col>
					<v-col>
						<v-text-field
							v-model="searchText"
							label="Search"
							clearable
							dense
						></v-text-field>
					</v-col>
					<v-col cols="auto">
						<v-switch
							hide-details
							height="0"
							v-model="showAll"
							label="All"
						/>
					</v-col>
					<v-col cols="12" sm="auto">
						<v-btn @click="userDialog = true" color="success">
							Add patient
							<v-icon right>mdi-account-plus</v-icon>
						</v-btn>
					</v-col>
				</v-row>
				<v-data-table
					:headers="dynamicHeaders"
					:items="$store.getters[`${this.role}/patients`]"
					show-select
					single-select
					v-model="selectedPatients"
					:search="searchText"
				>
					<template #item.birthDate="{value}">{{value | dateTime}}</template>
					<template #item.linkedToMe="{value}">
						<v-simple-checkbox readonly :value="value" />
					</template>
					<template #item.actions="{item}">
						<v-menu>
							<template #activator="{on: onMenu}">
								<v-tooltip bottom :open-delay="500">
									{{item.linkedToMe ? 'Unlink' : 'Link'}} patient
									<template #activator="{on: onTip}">
										<v-btn
											v-on="{...onTip, ...onMenu}"
											icon
											:color="item.linkedToMe ? 'warning' : 'success'"
											:disabled="!item.linkedToMe && item.dentistId"
										><v-icon>mdi-account-{{item.linkedToMe ? 'minus' : 'plus'}}</v-icon></v-btn>
									</template>
								</v-tooltip>
							</template>
							<v-card>
								<v-card-text>Are you sure you want to {{item.linkedToMe ? 'unlink' : 'link'}} this user?</v-card-text>
								<v-card-actions>
									<v-btn :color="item.linkedToMe ? 'warning' : 'success'" text @click="linkPatient(item.id, !item.linkedToMe)">
										{{item.linkedToMe ? 'Unlink' : 'Link'}}
									</v-btn>
								</v-card-actions>
							</v-card>
						</v-menu>
						<v-tooltip bottom :open-delay="500">
							Edit patient
							<template #activator="{on, attrs}">
								<v-btn v-on="on" v-bind="attrs" icon color="info" @click="editPatient(item)"><v-icon>mdi-account-edit</v-icon></v-btn>
							</template>
						</v-tooltip>
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
		private showAll = false;
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

		get selectedPatientId() {
			if (this.selectedPatients.length === 1) {
				return this.selectedPatients[0].id;
			} else {
				return null;
			}
		}

		get dynamicHeaders() {
			if (this.showAll) {
				const headers = [...this.headers];
				headers.splice(5, 0, {
					text: 'Linked',
					value: 'linkedToMe',
				});
				return headers;
			}
			return this.headers;
		}

		async mounted() {
			await this.refreshData();
		}

		editPatient(patient: Patient) {
			this.patient = patient;
			this.userDialog = true;
		}

		async linkPatient(id: number, link: boolean) {
			let initData: RequestInit = {
				method: 'PUT',
				headers: {
					'Authorization': `Bearer ${this.$store.getters['Auth/token']}`,
				}
			}
			await fetch(`${process.env.APIURL}/${this.role}/Patients/${id}/${link ? 'Link' : 'Unlink'}`, initData);
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

		@Watch('showAll')
		async refreshData() {
			this.$store.dispatch(`${this.role}/updatePatients`, this.showAll);
		}

		@Watch('userDialog')
		resetPatient(userDialog: boolean) {
			if (!userDialog) this.patient = null;
			else if (this.patient?.birthDate) this.patient.birthDate = this.patient.birthDate.split('T')[0];
		}
	}
</script>