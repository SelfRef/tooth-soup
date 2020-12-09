<template>
	<patient-list v-if="loggedIn"/>
</template>

<script lang='ts'>
import { Vue, Component, Watch } from 'vue-property-decorator';
import PatientList from '~/components/PatientList.vue';
@Component({
	components: {
		PatientList,
	}
})
export default class Patients extends Vue {
	get loggedIn() {
		return Boolean(this.$store.getters['Auth/token']);
	}
}
</script>