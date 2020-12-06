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
	private appointments: Appointment[] = [];
	private ready: boolean = false;

	get items() {
		return this.appointments.map(a => ({
			name: a.serviceName,
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
		let initData: RequestInit = {
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${this.$store.getters['auth/token']}`,
			}
		}
		this.appointments = await fetch(`${process.env.APIURL}/Dentist/Appointments`, initData).then(response => response.json());
	}

	formatDate(dateIso: string) {
		return dateIso.substr(0, 16).replace('T', ' ');
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