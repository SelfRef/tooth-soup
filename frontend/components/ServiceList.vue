<template>
	<v-layout column v-if="isLoggedIn">
		<v-container>
			<v-row>
				<v-col>
					<h2>Service List</h2>
				</v-col>
				<v-spacer></v-spacer>
				<v-col cols="auto">
					<v-btn @click="refreshData" color="blue">
						Refresh
						<v-icon right>mdi-refresh</v-icon>
					</v-btn>
					<v-btn @click="dialog = true" color="green">
						Add service
						<v-icon right>mdi-puzzle-plus</v-icon>
					</v-btn>
				</v-col>
			</v-row>

			<v-data-table
				:headers="headers"
				:items="services"
			>
				<template #item.price="{value}">{{value | price}}</template>
				<template #item.actions="{item}">
					<v-btn icon color="blue" @click="edit(item)"><v-icon>mdi-puzzle-edit</v-icon></v-btn>
					<v-menu :close-on-content-click="false">
						<template #activator="{on, attrs}">
							<v-btn
								v-on="on"
								v-bind="attrs"
								icon
								color="red"
							><v-icon>mdi-puzzle-remove</v-icon></v-btn>
						</template>
						<v-card>
							<v-card-text>Are you sure you want to remove this service?</v-card-text>
							<v-card-actions>
								<v-btn color="red" text @click="remove(item.id)">Remove</v-btn>
							</v-card-actions>
						</v-card>
					</v-menu>
				</template>
			</v-data-table>
			<service-form :active.sync="dialog" :item="itemToEdit" @refresh="refreshData"/>
		</v-container>
	</v-layout>
</template>

<script lang='ts'>
	import { Vue, Component, Watch } from 'vue-property-decorator';
	import Service from 'interfaces/Service';
	import ServiceForm from '~/components/ServiceForm.vue';
	@Component({
		components: {

		},
		filters: {
			price(value: number) {
				const full = Math.floor(value);
				const rest = (value - full) * 100;
				if (rest > 0) return `${full}zł ${rest}gr`;
				else return `${full}zł`;
			},
		}
	})
	export default class ServiceList extends Vue {
		private services: Service[] = [];
		private itemToEdit: Service | null = null;
		private dialog = false;
		private headers = [
			{
				text: 'Name',
				value: 'name',
			},
			{
				text: 'Price',
				value: 'price',
			},
			{
				text: 'Appointments Count',
				value: 'appointmentsCount',
			},
			{
				text: 'Actions',
				value: 'actions',
				width: 150,
			},
		];

		async mounted() {
			if (!this.isLoggedIn) {
				return this.$router.replace('/');
			}
			await this.refreshData();
		}

		async refreshData() {
			let initData: RequestInit = {
				method: 'GET',
				headers: {
					'Authorization': `Bearer ${this.$store.getters['auth/token']}`,
				}
			}
			this.services = await fetch(`${process.env.APIURL}/Dentist/Services`, initData).then(response => response.json());
		}

		edit(item: Service) {
			this.itemToEdit = item;
			this.dialog = true;
		}

		async remove(id: number) {
			let initData: RequestInit = {
				method: 'DELETE',
				headers: {
					'Authorization': `Bearer ${this.$store.getters['auth/token']}`,
				}
			}
			await fetch(`${process.env.APIURL}/Dentist/Services/${id}`, initData);
			await this.refreshData();
		}

		get isLoggedIn() {
			return this.$store.getters['auth/isLoggedIn'];
		}

		@Watch('dialog')
		resetPatient(dialog: boolean) {
			if (!dialog) this.itemToEdit = null;
		}
	}
</script>