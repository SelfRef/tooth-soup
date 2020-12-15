<template>
	<v-card class="mt-10" width="650" :loading="loading">
		<v-row>
			<v-col cols="auto" class="pa-10" align-self="center">
				<v-icon size="150">{{ role === 'Patient' ? 'mdi-account-heart' : 'mdi-doctor' }}</v-icon>
			</v-col>
			<v-col>
				<v-card-title>{{name}}</v-card-title>
				<v-card-subtitle>
					<v-icon small left>mdi-tooth</v-icon>
					{{ role === "Patient" ? 'Patient' : 'Dentist' }}
				</v-card-subtitle>
				<v-card-text>
					<v-form v-if="data">
						<v-text-field
							v-model="data.email"
							label="Email"
							prepend-icon="mdi-email"
						/>
						<v-text-field
							v-model="data.password"
							type="password"
							label="Password"
							placeholder="(unchanged)"
							prepend-icon="mdi-lock"
						/>
						<v-select
							v-if="role === 'Patient'"
							:items="dentistsForLinking"
							:item-text="d => d.name"
							:item-value="d => d.id"
							label="Linked dentist"
							v-model="data.dentistId"
							prepend-icon="mdi-account-star"
						/>
						<v-switch
							v-if="role === 'Dentist'"
							label="Patients can link me as the main dentist"
							v-model="data.canLink"
							:prepend-icon="data.canLink ? 'mdi-link' : 'mdi-link-lock'"
						/>
						<v-switch
							v-if="role === 'Dentist'"
							label="Not mine patients can create appointments"
							v-model="data.canCreateAppointment"
							:prepend-icon="data.canCreateAppointment ? 'mdi-calendar-account' : 'mdi-calendar-lock'"
						/>
					</v-form>
				</v-card-text>
				<v-card-actions class="mr-2">
					<v-spacer></v-spacer>
					<v-btn
						color="primary"
						@click="pushData"
						:disabled="!isUpdated"
					>
						<v-icon left>mdi-content-save-cog</v-icon>
						Update
					</v-btn>
					<v-btn
						color="error"
						@click="resetData"
						:disabled="!isUpdated"
					>
						<v-icon left>mdi-content-save-off</v-icon>
						Reset
					</v-btn>
				</v-card-actions>
			</v-col>
		</v-row>
	</v-card>
</template>

<script lang="ts">
import { Vue, Component, Watch } from 'vue-property-decorator';
import Dentist from '~/interfaces/Dentist';
import Patient from '~/interfaces/Patient';
@Component
export default class AccountInfo extends Vue {
	private loading = true;
	private data: Patient | Dentist = {
		email: '',
		password: '',
		dentistId: 0,
		canLink: false,
		canCreateAppointment: false,
	};

	get role() {
		return this.$store.getters['Auth/userRole'];
	}

	get account(): Patient | Dentist {
		if (this.role === 'Patient') {
			return this.$store.getters[`${this.role}/account`];
		} else {
			return this.$store.getters[`${this.role}/account`];
		}
	}

	get name() {
		if (this.account) return `${this.account.firstName} ${this.account.lastName}`
	}

	get isUpdated(): boolean {
		return Object.keys(this.data).some(k => {
			return (this.data[k] || '') !== (this.account[k] || '');
		});
	}

	get dentistsForLinking() {
		const dentists = this.$store.getters[`${this.role}/dentists`].filter(d => d.canLink);
		
	}

	async mounted() {
		this.loading = true
		await this.$store.dispatch(`${this.role}/pullAccount`);
		if (this.role === 'Patient') {
			await this.$store.dispatch(`${this.role}/updateDentists`);
		}
	}

	async pushData() {
		this.loading = true
		switch (this.role) {
			case 'Patient':
				await this.$store.dispatch(`${this.role}/pushPatient`, this.data);
				break;
			case 'Dentist':
				await this.$store.dispatch(`${this.role}/pushAccount`, this.data);
				break;
		}
	}

	async resetData() {
		await this.$store.dispatch(`${this.role}/pullAccount`);
	}

	@Watch('account')
	pullData(account: Patient | Dentist) {
		if (!account) return;
		this.data.email = account.email;
		this.data.password = '';
		if (this.role === 'Patient') {
			this.data.dentistId = account.dentistId;
		}
		if (this.role === 'Dentist') {
			this.data.canLink = account.canLink;
			this.data.canCreateAppointment = account.canCreateAppointment;
		}
		this.loading = false;
	}
}
</script>