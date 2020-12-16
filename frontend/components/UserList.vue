<template>
	<v-container>
		<v-row>
			<v-col cols="auto">
				<h2>User List</h2>
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
			<v-col cols="auto">
				<v-btn @click="dialog = true" color="green">
					Add user
					<v-icon right>mdi-account-plus</v-icon>
				</v-btn>
			</v-col>
		</v-row>

		<v-data-table
			:headers="headers"
			:items="users"
		>
			<template #item.patient.dentistName="{item, value}">{{value || (item.role === 'Patient' ? '[none]' : '[n/a]')}}</template>
			<template #item.actions="{item}">
				<v-tooltip bottom :open-delay="500">
					Edit user
					<template #activator="{on, attrs}">
						<v-btn v-on="on" v-bind="attrs" icon color="blue" @click="edit(item)"><v-icon>mdi-account-edit</v-icon></v-btn>
					</template>
				</v-tooltip>
				<v-menu>
					<template #activator="{on: onMenu}">
						<v-tooltip bottom :open-delay="500">
							Remove user
							<template #activator="{on: onTip}">
								<v-btn
									v-on="{...onTip, ...onMenu}"
									icon
									color="red"
								><v-icon>mdi-account-remove</v-icon></v-btn>
							</template>
						</v-tooltip>
					</template>
					<v-card>
						<v-card-text>Are you sure you want to remove this user?</v-card-text>
						<v-card-actions>
							<v-btn color="red" text @click="remove(item.id)">Remove</v-btn>
						</v-card-actions>
					</v-card>
				</v-menu>
			</template>
		</v-data-table>
		<user-form :active.sync="dialog" :item="itemToEdit" @refresh="refreshData" :dentists="dentists"/>
	</v-container>
</template>

<script lang='ts'>
	import { Vue, Component, Watch } from 'vue-property-decorator';
	import User from 'interfaces/User';
	import UserForm from '~/components/UserForm.vue';
	@Component({
		components: {
			UserForm,
		},
		filters: {
		}
	})
	export default class UserList extends Vue {
		private itemToEdit: User | null = null;
		private dialog = false;
		private headers = [
			{
				text: 'Id',
				value: 'id',
			},
			{
				text: 'Name',
				value: 'name',
			},
			{
				text: 'Role',
				value: 'role',
			},
			{
				text: 'Email',
				value: 'email',
			},
			{
				text: 'Dentist',
				value: 'patient.dentistName',
			},
			{
				text: 'Actions',
				value: 'actions',
				width: 110,
			},
		];

		get role() {
			return this.$store.getters['Auth/userRole'];
		}

		get users() {
			return this.$store.getters[`${this.role}/users`];
		}

		get dentists() {
			const dentists = this.users.filter(u => u.role === 'Dentist').map(u => ({
				id: u.id,
				name: u.name
			}));
			dentists.unshift({
				id: 0,
				name: '[none]'
			});
			return dentists;
		}

		async mounted() {
			await this.refreshData();
		}

		edit(item: User) {
			this.itemToEdit = item;
			this.dialog = true;
		}

		async refreshData() {
			await this.$store.dispatch(`${this.role}/pullUsers`);
		}

		async remove(id: number) {
			await this.$store.dispatch(`${this.role}/dropUser`, id);
		}

		@Watch('dialog')
		reset(dialog: boolean) {
			if (!dialog) this.itemToEdit = null;
		}
	}
</script>