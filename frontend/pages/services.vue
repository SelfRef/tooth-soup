<template>
	<service-list v-if="loggedIn"/>
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
		get loggedIn() {
			return Boolean(this.$store.getters['Auth/token']);
		}
	}
</script>