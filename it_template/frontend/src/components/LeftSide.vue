
<template>
  <div class="left-sidenav">
    <ul class="metismenu left-sidenav-menu">
      <li>
        <router-link to="/">
          <i class="ti-bar-chart"></i>
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
          <span>Hệ thống</span>
          <span class="menu-arrow"><i class="mdi mdi-chevron-right"></i></span>
        </a>

        <ul class="nav-second-level" aria-expanded="false">
          <li class="nav-item">
            <router-link class="nav-link" to="/user"><i class="ti-control-record"></i>Tài khoản</router-link>
          </li>
        </ul>
      </li>
    </ul>
  </div>
</template>
<script setup>

import { useAuth } from '../stores/auth';
const store = useAuth();
const is_admin = store.is_admin;
// console.log(in_groups);
// import { onMounted } from 'vue'
</script>