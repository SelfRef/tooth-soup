<template>
  <v-app>
    <v-app-bar fixed app>
      <v-toolbar-title v-text="title" />
      <v-tabs :value="tabNumber">
        <v-tab to="/">Home</v-tab>
        <v-tab v-if="role === 'Dentist'" to="/patients">Patients</v-tab>
        <v-tab v-if="role === 'Dentist'" to="/services">Services</v-tab>
        <v-tab v-if="role === 'Patient'" to="/appointments">Appointments</v-tab>
      </v-tabs>
      <v-spacer />
      <v-btn-toggle v-model="theme" class="mr-8">
        <v-btn><v-icon>mdi-brightness-auto</v-icon></v-btn>
        <v-btn><v-icon>mdi-brightness-7</v-icon></v-btn>
        <v-btn><v-icon>mdi-brightness-4</v-icon></v-btn>
      </v-btn-toggle>
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
      theme: 0,
    }
  },
  computed: {
    loggedIn() {
      return Boolean(this.$store.getters['auth/token']);
    },
    role() {
      return this.$store.getters['auth/userRole'];
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
    },
    changeTheme(theme) {
      switch (theme) {
        case 0:
          const mq = window.matchMedia('(prefers-color-scheme: dark)');
          this.$vuetify.theme.dark = mq.matches;
          mq.addEventListener('change', (e) => {
            this.$vuetify.theme.dark = e.matches;
            this.theme = 0;
          });
          break;
        case 1:
          this.$vuetify.theme.dark = false;
          break;
        case 2:
          this.$vuetify.theme.dark = true;
          break;
      }
    }
  },
  mounted() {
    this.theme = this.$store.getters['auth/theme'];
    this.changeTheme(this.theme);
    this.$store.dispatch('auth/checkToken');
  },
  watch: {
    theme(theme) {
      this.$store.dispatch('auth/setTheme', theme);
      this.changeTheme(theme);
    }
  }
}
</script>
