<template>
  <md-toolbar md-elevation="0" class="md-transparent">
    <div class="md-toolbar-row">
      <div class="md-toolbar-section-start">
        <h3 class="md-title">{{$route.name}}</h3>
      </div>
      <div class="md-toolbar-section-end">

        <div class="md-collapse">

          <md-list>
            <md-list-item to="/">
              <i class="material-icons">dashboard</i>
              <p class="hidden-lg hidden-md">Dashboard</p>
            </md-list-item>

            <md-list-item to="/acceuil/followers" class="dropdown">
              <drop-down>
                <a slot="title" class="dropdown-toggle" data-toggle="dropdown">
                  <i class="material-icons">notifications</i>
                  <span v-show="this.counterWait!=0" class="notification">{{counterWait}}</span>
                  <p class="hidden-lg hidden-md">Notifications</p>
                </a>

              </drop-down>
            </md-list-item>

            <md-list-item to="/acceuil/user">
              <i class="material-icons">person</i>
              <p class="hidden-lg hidden-md">Profile</p>
            </md-list-item>

            <md-button v-on:click="signOut()" class="md-round md-danger">Déconnexion</md-button>

          </md-list>
        </div>
      </div>
    </div>

  </md-toolbar>
</template>

<script>

export default{
  data () {
    return {
      profil:JSON.parse(localStorage.getItem('user2')),
      counterWait:0
    }
  },
  methods: {
    toggleSidebar () {
      this.$sidebar.displaySidebar(!this.$sidebar.showSidebar)
    },
    signOut:function(){
      localStorage.clear();
      this.$cookies.remove("token","/");
      this.$router.push({
          name: 'Connexion'
      });
    },
    notif:function(){
      this.counterWait=this.profil.Waiting_List.length

    }
  },
  mounted:function() {
      this.notif()
  },
}
</script>

<style lang="css">
</style>
