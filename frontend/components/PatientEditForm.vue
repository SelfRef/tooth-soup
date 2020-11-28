<template>
  <v-row justify="center">
    <v-dialog
      v-model="active"
      persistent
      max-width="600px"
    >
      <v-card>
        <v-card-title>
          <span class="headline">{{ edit ? 'Edit' : 'Create' }} Patient</span>
        </v-card-title>
        <v-card-text>
          <v-container>
            <v-row>
              <v-col
                cols="12"
                sm="6"
              >
                <v-text-field
                  label="PESEL number*"
                  required
                  v-model="user.pesel"
                ></v-text-field>
              </v-col>
              <v-col
                cols="12"
                sm="6"
              >
                <v-menu
                  v-model="datePickerActive"
                >
                  <template v-slot:activator="{ on, attrs }">
                    <v-text-field
                      v-model="user.birthDate"
                      label="Birth Date*"
                      hint="YYYY-MM-DD format"
                      persistent-hint
                      prepend-icon="mdi-calendar"
                      v-bind="attrs"
                      v-on="on"
                    ></v-text-field>
                  </template>
                  <v-date-picker
                    v-model="user.birthDate"
                    no-title
                    @input="datePickerActive = false"
                  ></v-date-picker>
                </v-menu>
              </v-col>
              <v-col
                cols="12"
                sm="6"
              >
                <v-text-field
                  label="Legal first name*"
                  required
                  v-model="user.firstName"
                ></v-text-field>
              </v-col>
              <v-col
                cols="12"
                sm="6"
              >
                <v-text-field
                  label="Legal last name*"
                  required
                  v-model="user.lastName"
                ></v-text-field>
              </v-col>
              <v-col cols="12" sm="6">
                <v-text-field
                  label="Email*"
                  required
                  v-model="user.email"
                ></v-text-field>
              </v-col>
              <v-col cols="12" sm="6">
                <v-text-field
                  label="Password*"
                  type="password"
                  required
                  v-model="user.password"
                ></v-text-field>
              </v-col>
            </v-row>
          </v-container>
          <small>*indicates required field</small>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn
            color="blue darken-1"
            text
            @click="$emit('update:active', false)"
          >
            Close
          </v-btn>
          <v-btn
            color="blue darken-1"
            text
            @click="save"
          >
            Save
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-row>
</template>

<script lang="ts">
import { Vue, Component, Prop, Ref } from 'vue-property-decorator';
import Patient from '~/interfaces/Patient';

@Component
export default class PatientEditForm extends Vue {
  @Prop() active = false;
  @Prop() edit = false;
  @Prop() userData: Patient | null = null;
  private datePickerActive = false;

  mounted() {
    this.user = this.userData ? {...this.userData} : this.user;
  }

  private user: Patient = {
    pesel: '',
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    birthDate: null,
  }

  async save() {
    const fetchOptions: RequestInit = {
      method: 'POST',
      body: JSON.stringify(this.user),
      headers: {
          'Authorization': `Bearer ${this.$store.getters['auth/token']}`,
          'Content-Type': 'application/json'
				}
    }
    await fetch(`${process.env.APIURL}/Dentist/Patient`, fetchOptions);
  }
}
</script>