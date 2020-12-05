<template>
  <v-app>
    <v-app-bar fixed app>
      <v-toolbar-title v-text="title" />
      <v-tabs :value="tabNumber">
        <v-tab to="/">Home</v-tab>
        <v-tab to="/patients">Patients</v-tab>
        <v-tab to="/appointments">Appointments</v-tab>
        <v-tab to="/services">Services</v-tab>
      </v-tabs>
      <v-spacer />
      <template v-if="loggedIn">
        <v-menu offset-y>
          <template v-slot:activator="{ on, attrs }">
            <v-avatar
              color="primary"
              v-bind="attrs"
              v-on="on"
            >JN</v-avatar>
          </template>
          <v-list>
            <v-list-item @click="logout">
              <v-list-item-title>Logout</v-list-item-title>
            </v-list-item>
          </v-list>
        </v-menu>
      </template>
      <template v-else>
        <v-btn
          color="primary"
          @click="loginDialog = !loginDialog"
        >Login</v-btn>
      </template>
    </v-app-bar>
    <v-main>
      <nuxt />
    </v-main>
    <v-footer app>
      <span>&copy; {{copyName}} {{ new Date().getFullYear() }}</span>
    </v-footer>
    <login-form :active.sync="loginDialog"/>
  </v-app>
</template>

<script>
import 'reflect-metadata';
import { Vue, Component, Prop } from 'vue-property-decorator';
import LoginForm from "@/components/LoginForm";
export default {
  components: {
    LoginForm,
  },
  data() {
    return {
      title: 'Vuetify.js',
      copyName: 'Tooth Soup Corp.',
      loginDialog: false,
    }
  },
  computed: {
    loggedIn() {
      return Boolean(this.$store.getters['auth/token']);
    },
    tabNumber() {
      switch(this.$route.path) {
        case '/':
          return 0;
        case '/patients':
          return 1;
        case '/appointments':
          return 2;
        case '/services':
          return 3;
      }
    }
  },
  methods: {
    logout() {
      this.$store.dispatch('auth/setToken', null);
    }
  },
  mounted() {
    const mq = window.matchMedia('(prefers-color-scheme: dark)');
    this.$vuetify.theme.dark = mq.matches;
    mq.addEventListener('change', (e) => {
      this.$vuetify.theme.dark = e.matches;
    });

    this.$store.dispatch('auth/checkToken');
  }
}
</script>
