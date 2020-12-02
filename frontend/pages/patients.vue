<template>
	<patient-list/>
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
	}
</script>