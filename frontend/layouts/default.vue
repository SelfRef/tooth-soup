<template>
  <v-app>
    <v-app-bar :clipped-left="clipped" fixed app>
      <v-toolbar-title v-text="title" />
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
      <v-container>
        <nuxt />
      </v-container>
    </v-main>
    <v-footer :absolute="!fixed" app>
      <span>&copy; {{copyName}} {{ new Date().getFullYear() }}</span>
    </v-footer>
    <login-form :active.sync="loginDialog"/>
  </v-app>
</template>

<script>
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

    this.$store.dispatch('checkToken');
  }
}
</script>
