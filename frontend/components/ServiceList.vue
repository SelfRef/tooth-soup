<template>
	<v-container>
		<v-row>
			<v-col cols="auto">
				<h2>Service List</h2>
			</v-col>
			<v-col cols="auto">
				<v-tooltip bottom :open-delay="500">
					Refresh
					<template #activator="{on}">
						<v-btn icon @click="refreshData" color="secondary" v-on="on">
							<v-icon>mdi-refresh</v-icon>
						</v-btn>
					</template>
				</v-tooltip>
			</v-col>
			<v-spacer></v-spacer>
			<v-col cols="auto" v-if="role === 'Dentist'">
				<v-switch
					hide-details
					height="0"
					v-model="linkedOnly"
					label="Linked only"
				/>
			</v-col>
			<v-col cols="auto">
				<v-btn @click="dialog = true" color="success">
					Add service
					<v-icon right>mdi-puzzle-plus</v-icon>
				</v-btn>
			</v-col>
		</v-row>

		<v-data-table
			:headers="tableHeaders"
			:items="services"
			:item-class="unlinkedRow"
		>
			<template #item.price="{value}">{{value | price}}</template>
			<template #item.linked="{item, value}">
				<v-menu>
					<template #activator="{on}">
						<v-simple-checkbox readonly v-on="on" :value="value" />
					</template>
					<v-card>
						<v-card-text>Are you sure you want to {{ item.linked ? 'unlink' : 'link' }} this service?</v-card-text>
						<v-card-actions>
							<v-btn :color="item.linked ? 'warning' : 'success'" text @click="link(item.id, !item.linked)">{{ item.linked ? 'Unlink' : 'Link' }}</v-btn>
						</v-card-actions>
					</v-card>
				</v-menu>
			</template>
			<template #item.actions="{item}">
				<v-tooltip bottom :open-delay="500">
					Edit service
					<template #activator="{on, attrs}">
						<v-btn
							v-on="on"
							v-bind="attrs"
							icon
							color="primary"
							@click="edit(item)"
							:disabled="!item.canEdit"
							>
								<v-icon>mdi-puzzle-edit</v-icon>
							</v-btn>
					</template>
				</v-tooltip>
				<v-menu>
					<template #activator="{on: onMenu}">
						<v-tooltip bottom :open-delay="500">
							Remove service
							<template #activator="{on: onTip}">
								<v-btn
									v-on="{...onTip, ...onMenu}"
									icon
									color="error"
									:disabled="!item.canEdit"
								><v-icon>mdi-puzzle-remove</v-icon></v-btn>
							</template>
						</v-tooltip>
					</template>
					<v-card>
						<v-card-text>Are you sure you want to remove this service?</v-card-text>
						<v-card-actions>
							<v-btn color="error" text @click="remove(item.id)">Remove</v-btn>
						</v-card-actions>
					</v-card>
				</v-menu>
			</template>
		</v-data-table>
		<service-form :active.sync="dialog" :item="itemToEdit" @refresh="refreshData"/>
	</v-container>
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
		private itemToEdit: Service | null = null;
		private dialog = false;
		private linkedOnly = false;
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
				text: 'Dentists',
				value: 'dentistsCount',
			},
			{
				text: 'Appointments',
				value: 'appointmentsCount',
			},
			{
				text: 'Linked',
				value: 'linked',
			},
			{
				text: 'Actions',
				value: 'actions',
				width: 110,
			},
		];

		get tableHeaders() {
			if (this.role === 'Dentist') return this.headers;
			else {
				const newHeaders = [...this.headers];
				newHeaders.splice(4, 1);
				return newHeaders;
			}
		}

		get role() {
			return this.$store.getters['Auth/userRole'];
		}

		get services() {
			const all = this.$store.getters[`${this.role}/services`];
			if (this.linkedOnly) return all.filter(s => s.linked);
			else return all;
		}

		async mounted() {
			await this.refreshData();
		}

		async refreshData() {
			this.$store.dispatch(`${this.role}/pullServices`);
		}

		edit(item: Service) {
			this.itemToEdit = item;
			this.dialog = true;
		}

		async remove(id: number) {
			this.$store.dispatch(`${this.role}/dropService`, id);
		}

		async link(id: number, link: boolean) {
			this.$store.dispatch(`${this.role}/linkService`, {id, link});
		}

		unlinkedRow(item: Service) {
			if (this.role === 'Dentist' && !item.linked) {
				return this.$vuetify.theme.dark ? 'black' : 'grey lighten-2'
			}
			return '';
		}

		@Watch('dialog')
		resetPatient(dialog: boolean) {
			if (!dialog) this.itemToEdit = null;
		}
	}
</script>