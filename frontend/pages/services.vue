<template>
	<service-list/>
</template>

<script lang='ts'>
	import { Vue, Component, Watch } from 'vue-property-decorator';
	import ServiceList from '~/components/ServiceList.vue';
	@Component({
		components: {
			ServiceList,
		}
	})
	export default class Services extends Vue {
	}
</script>