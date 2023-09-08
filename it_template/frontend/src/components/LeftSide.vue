<template>
  <div class="left-sidenav">
    <ul class="metismenu left-sidenav-menu">
      <li v-if="is_admin">
        <router-link class="nav-link" to="/">
          <i class="fas fa-chart-bar"></i>
          <span>Tổng quan</span>
        </router-link>
      </li>
      
      <li v-if="is_admin">
        <router-link class="nav-link" to="/history">
          <i class="fas fa-history"></i>
          <span>Audittrails</span>
        </router-link>
      </li>

      <li v-if="is_admin">
        <a href="javascript: void(0);">
          <i class="ti-briefcase"></i>
          <span>{{ $t("system") }}</span>
          <span class="menu-arrow"><i class="mdi mdi-chevron-right"></i></span>
        </a>

        <ul class="nav-second-level" aria-expanded="false">
          <li class="nav-item">
            <router-link class="nav-link" to="/user"
              ><i class="ti-control-record"></i>{{ $t("user") }}</router-link
            >
          </li>
        </ul>
      </li>
    </ul>
  </div>
</template>
<script setup>
import { useAuth } from "../stores/auth";
import { onMounted, ref } from "vue";
import { useLayout } from "../layouts/composables/layout";
import { storeToRefs } from "pinia";

const { initMetisMenu, initActiveMenu } = useLayout();
const store = useAuth();
const { is_admin } = storeToRefs(store);

onMounted(() => {
  initMetisMenu();
  initActiveMenu();
});
</script>
