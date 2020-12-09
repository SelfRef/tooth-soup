<template>
	<v-container v-if="loggedIn">
		<v-row justify="center">
			<v-col cols="auto">
				<account-info />
			</v-col>
		</v-row>
	</v-container>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import AccountInfo from '~/components/AccountInfo.vue';
@Component({
	components: {
		AccountInfo,
	}
})
export default class Account extends Vue {
	get loggedIn() {
		return Boolean(this.$store.getters['Auth/token']);
	}
}
</script>