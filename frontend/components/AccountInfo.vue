<template>
	<v-card class="mt-10" width="600">
		<v-row>
			<v-col cols="auto" class="pa-10" align-self="center"><v-icon size="150">mdi-account-heart</v-icon></v-col>
			<v-col>
				<v-card-title>{{name}}</v-card-title>
				<v-card-subtitle><v-icon small left>mdi-tooth</v-icon>Patient</v-card-subtitle>
				<v-card-text>
					<v-form>
						<v-text-field
							v-model="data.email"
							label="Email"
							:append-outer-icon="emailUpdated ? 'mdi-email-check' : null"
							@click:append-outer="pushData"
						/>
						<v-text-field
							v-model="data.password"
							type="password"
							label="Password"
							placeholder="(unchanged)"
							:append-outer-icon="passwordUpdated ? 'mdi-lock-check' : null"
							@click:append-outer="pushData"
						/>
						<v-select
							:items="$store.getters['patient/dentists']"
							:item-text="d => d.name"
							:item-value="d => d.id"
							label="Linked dentist"
							v-model="data.dentistId"
							:append-outer-icon="dentistUpdated ? 'mdi-account-check' : null"
							@click:append-outer="pushData"
						/>
					</v-form>
				</v-card-text>
			</v-col>
		</v-row>
	</v-card>
</template>

<script lang="ts">
import { Vue, Component, Watch } from 'vue-property-decorator';
import Patient from '~/interfaces/Patient';
@Component
export default class AccountInfo extends Vue {
	private data: Patient = {
		email: '',
		password: '',
		dentistId: 0,
	};

	get account(): Patient {
		return this.$store.getters['patient/account'];
	}

	get name() {
		if (this.account) return `${this.account.firstName} ${this.account.lastName}`
	}

	get emailUpdated() {
		return this.data.email !== this.account.email;
	}

	get passwordUpdated() {
		return this.data.password !== '';
	}

	get dentistUpdated() {
		return this.data.dentistId !== this.account.dentistId;
	}

	async mounted() {
		this.$store.dispatch('patient/updateAccount')
		this.$store.dispatch('patient/updateDentists')
	}

	async pushData() {
		this.$store.dispatch('patient/pushPatient', this.data);
	}

	@Watch('account')
	pullData(account: Patient) {
		this.data.email = account.email;
		this.data.password = '';
		this.data.dentistId = account.dentistId;
	}
}
</script>