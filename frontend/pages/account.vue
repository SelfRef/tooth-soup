<template>
	<v-container>
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
export default class account extends Vue {

}
</script>