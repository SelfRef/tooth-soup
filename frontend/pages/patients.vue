<template>
	<v-layout>
		<v-flex>
			<h2>Patient List</h2>
			<v-simple-table>
				<thead>
					<tr>
						<th>First Name</th>
						<th>Last Name</th>
						<th>Email</th>
					</tr>
				</thead>
				<tbody>
					<tr v-for="patient in patients" :key="patient.id">
						<td>{{patient.firstName}}</td>
						<td>{{patient.lastName}}</td>
						<td>{{patient.email}}</td>
					</tr>
				</tbody>
			</v-simple-table>
		</v-flex>
	</v-layout>
</template>

<script lang='ts'>
	import Vue from 'vue';
	import { Component } from 'vue-property-decorator';
	import Patient from 'interfaces/Patient';
	@Component
	export default class Patients extends Vue {
		private patients: Patient[] = [];

		async mounted() {
			this.patients = await fetch(`${process.env.APIURL}/Dentist/Patients`).then(response => response.json());
		}
	}
</script>