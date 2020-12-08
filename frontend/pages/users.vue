<template>
	<user-list/>
</template>

<script lang='ts'>
	import { Vue, Component, Watch } from 'vue-property-decorator';
	import UserList from '~/components/UserList.vue';
	@Component({
		components: {
			UserList,
		}
	})
	export default class Users extends Vue {
	}
</script>