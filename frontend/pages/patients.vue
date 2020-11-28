<template>
	<v-layout>
		<v-flex>
			<h2>Patient List</h2>
			<v-btn @click="userDialog = true">
				Add user
				<v-icon right>mdi-account-plus</v-icon>
			</v-btn>
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
							<v-btn icon color="blue"><v-icon>mdi-account-edit</v-icon></v-btn>
							<v-btn icon color="red"><v-icon>mdi-account-remove</v-icon></v-btn>
						</td>
					</tr>
				</tbody>
			</v-simple-table>
		</v-flex>
		<patient-edit-form :active.sync="userDialog"/>
	</v-layout>
</template>

<script lang='ts'>
	import { Vue, Component } from 'vue-property-decorator';
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

		async mounted() {
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