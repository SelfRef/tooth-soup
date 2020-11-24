<template>
  <v-app dark>
    <v-app-bar :clipped-left="clipped" fixed app>
      <v-toolbar-title v-text="title" />
      <v-spacer />
      <template v-if="loggedIn">
        <v-avatar
          color="primary"
        >JN</v-avatar>
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
  }
}
</script>
