<template>
	<v-container v-if="loggedIn">
		<v-row>
			<v-col cols="12" lg="8">
				<appointment-list />
			</v-col>
			<v-col>
				<appointment-calendar />
			</v-col>
		</v-row>
	</v-container>
</template>

<script lang="ts">
	import { Vue, Component } from 'vue-property-decorator';
	import AppointmentList from '~/components/AppointmentList.vue';
	import AppointmentCalendar from '~/components/AppointmentCalendar.vue';
	@Component({
		components: {
			AppointmentList,
		}
	})
	export default class Appointments extends Vue {
		get loggedIn() {
			return Boolean(this.$store.getters['Auth/token']);
		}
	}
</script>