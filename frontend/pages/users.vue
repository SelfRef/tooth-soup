<template>
	<user-list v-if="loggedIn"/>
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
	get loggedIn() {
		return Boolean(this.$store.getters['Auth/token']);
	}
}
</script>