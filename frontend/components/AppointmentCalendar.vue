<template>
	<v-sheet height="800">
		<v-calendar
			ref="calendar"
			:events="items"
			:type="calendarType"
			:event-color="i => i.color"
			:event-name="getName"
		>
			<template v-slot:day-body="{ date, week }">
				<div
					class="v-current-time"
					:class="{ first: date === week[0].date }"
					:style="{ top: nowY }"
				></div>
			</template>
		</v-calendar>
	</v-sheet>
</template>

<script lang="ts">
import { Vue, Component, Ref, Prop } from 'vue-property-decorator';
import Appointment from '~/interfaces/Appointment';

@Component
export default class AppointmentCalendar extends Vue {
	@Ref() calendar;
	private ready: boolean = false;

	get role() {
		return this.$store.getters['auth/userRole'];
	}

	get items() {
		let appointments;
		if (this.role === 'Patient') {
			appointments = this.$store.getters['patient/appointments'];
		} else if (this.role === 'Dentist') {
			appointments = this.$store.getters['dentist/appointments'];
		}
		if (!appointments) return [];
		return appointments.map(a => ({
			name: this.eventName(a),
			start: this.formatDate(a.startDate),
			end: this.formatDate(a.endDate),
			color: a.canceled ? 'error' : 'primary',
			canceled: a.canceled,
		}))
	}

	get calendarType() {
		if (this.$vuetify.breakpoint.xlOnly) return '4day';
		if (this.$vuetify.breakpoint.mdOnly) return '4day';
		else return 'day';
	}

	get cal () {
		return this.ready ? this.calendar : null
	}

	get nowY () {
		return this.cal ? this.cal.timeToY(this.cal.times.now) + 'px' : '-10px'
	}

	getName(item) {
		if (item.input.canceled) return `❌ ${item.input.name}`;
		return item.input.name
	}

	async mounted() {
		await this.refreshData();
		this.ready = true;
		this.calendar.scrollToTime(new Date().toTimeString().substr(0, 5));
	}

	async refreshData() {
		if (this.role === 'Patient') {
			this.$store.dispatch('patient/updateAppointments');
		} else if (this.role === 'Patient') {
			this.$store.dispatch('dentist/updateAppointments');
		}
	}

	formatDate(dateIso: string) {
		return dateIso.substr(0, 16).replace('T', ' ');
	}

	eventName(appointment: Appointment) {
		if (appointment.serviceName) {
			let name;
			if (this.role === 'Patient') name = appointment.dentistName;
			else if (this.role === 'Dentist') name = appointment.patientName;
			return `${appointment.serviceName} (${name})`;
		} else {
			return 'Busy';
		}
	}
}
</script>

<style lang="scss">
.v-current-time {
  height: 2px;
  background-color: #ea4335;
  position: absolute;
  left: -1px;
  right: 0;
  pointer-events: none;

  &.first::before {
    content: '';
    position: absolute;
    background-color: #ea4335;
    width: 12px;
    height: 12px;
    border-radius: 50%;
    margin-top: -5px;
    margin-left: -6.5px;
  }
}
</style>