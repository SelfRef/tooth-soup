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
							:append-icon="data.email !== account.email ? 'mdi-cog-sync' : null"
						/>
						<v-text-field
							v-model="data.password"
							type="password"
							label="Password"
							placeholder="(unchanged)"
							:append-icon="data.password !== '' ? 'mdi-cog-sync' : null"
						/>
						<v-select
							v-if="role === 'Patient'"
							:items="$store.getters['patient/dentists'].filter(d => d.canLink)"
							:item-text="d => d.name"
							:item-value="d => d.id"
							label="Linked dentist"
							v-model="data.dentistId"
							:append-icon="data.dentistId !== account.dentistId ? 'mdi-cog-sync' : null"
						/>
						<v-switch
							v-if="role === 'Dentist'"
							label="Patients can link me as the main dentist"
							v-model="data.canLink"
							:append-icon="data.canLink !== account.canLink ? 'mdi-cog-sync' : null"
						/>
						<v-switch
							v-if="role === 'Dentist'"
							label="Not mine patients can create appointments"
							v-model="data.canCreateAppointment"
							:append-icon="data.canCreateAppointment !== account.canCreateAppointment ? 'mdi-cog-sync' : null"
						/>
					</v-form>
				</v-card-text>
				<v-card-actions>
					<v-btn
						color="primary"
						@click="pushData"
						:disabled="!isUpdated"
					>
						<v-icon left>mdi-cog-sync</v-icon>
						Update
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
		return this.$store.getters['auth/userRole'];
	}

	get account(): Patient | Dentist {
		if (this.role === 'Patient') {
			return this.$store.getters['patient/account'];
		} else {
			return this.$store.getters['dentist/account'];
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

	async mounted() {
		this.loading = true
		if (this.role === 'Patient') {
			await this.$store.dispatch('patient/updateAccount');
			await this.$store.dispatch('patient/updateDentists');
		} else if (this.role === 'Dentist') {
			await this.$store.dispatch('dentist/pullAccount');
		}
	}

	async pushData() {
		this.loading = true
		switch (this.role) {
			case 'Patient':
				await this.$store.dispatch('patient/pushPatient', this.data);
				break;
			case 'Dentist':
				await this.$store.dispatch('dentist/pushAccount', this.data);
				break;
		}
	}

	@Watch('account')
	pullData(account: Patient | Dentist) {
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