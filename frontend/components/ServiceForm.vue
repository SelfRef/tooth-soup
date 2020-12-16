<template>
	<v-row justify="center">
		<v-dialog
			v-if="active"
			v-model="active"
			persistent
			max-width="600px"
		>
			<v-form ref="form">
				<v-card>
					<v-card-title>
						<span class="headline">{{ edit ? 'Edit' : 'Create' }} Service</span>
					</v-card-title>
					<v-card-text>
						<v-container>
							<v-row>
								<v-col
									cols="12"
									sm="6"
								>
									<v-text-field
										label="Service name"
										:required="!Boolean(edit)"
										v-model="service.name"
										prepend-icon="mdi-form-textbox"
										:rules="[rules.required]"
									></v-text-field>
								</v-col>
								<v-col
									cols="12"
									sm="6"
								>
									<v-text-field
										type="number"
										suffix="zł"
										label="Service price"
										:required="!Boolean(edit)"
										v-model="service.price"
										prepend-icon="mdi-cash"
										:rules="[rules.required, rules.number]"
									></v-text-field>
								</v-col>
							</v-row>
						</v-container>
					</v-card-text>
					<v-card-actions>
						<v-spacer></v-spacer>
						<v-btn
							color="secondary"
							text
							@click="close"
						>
							Close
						</v-btn>
						<v-btn
							color="primary"
							@click="save"
						>
							Save
						</v-btn>
					</v-card-actions>
				</v-card>
			</v-form>
		</v-dialog>
	</v-row>
</template>

<script lang="ts">
import { Vue, Component, Prop, Ref, Watch, Emit } from 'vue-property-decorator';
import Service from '~/interfaces/Service';
import { rules } from "~/lib/helpers";

@Component
export default class ServiceForm extends Vue {
	@Prop({default: false}) active!: boolean;
	@Prop({default: null}) item!: Service | null;
	@Ref('form') form;
	private rules = rules;
	private service: Service = {
		name: '',
		price: 0,
	}

	get role() {
		return this.$store.getters['Auth/userRole'];
	}

	get edit() {
		return Boolean(this.item);
	}

	@Emit('update:active')
	close() {
		this.form.resetValidation();
		return false;
	}

	@Emit('refresh')
	async save() {
		if (!this.form.validate()) return;
		this.$store.dispatch(`${this.role}/pushService`, this.service);
		this.close();
	}

	@Watch('active')
	onDialogShow(active: boolean) {
		this.service = this.item && active ? {...this.item} : {
			name: '',
			price: 0,
		};
	}
}
</script>