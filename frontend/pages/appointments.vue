<template>
	<v-container>
		<appointment-list />
		<appointment-calendar />
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

}
</script>