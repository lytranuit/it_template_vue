import { ref, computed } from "vue";
import { defineStore } from "pinia";
// import VueJwtDecode from "vue-jwt-decode";

import { useCookies } from "vue3-cookies";

import { useAxios } from "../service/axios";
const { axiosinstance } = useAxios();
export const useAuth = defineStore("auth", () => {
  const data = ref({});
  const roles = ref([]);
  const departments = ref([]);
  const isAuth = computed(() => {
    const { cookies } = useCookies();
    const Token = cookies.get("Auth-Token");
    return Token ? true : false;
  });
  const user = computed(() => {
    const cacheUser = localStorage.getItem("me") || "{}";
    return JSON.parse(cacheUser);
  });
  const is_admin = computed(() => {
    return in_groups(["Administrator"]);
  });
  // const getRoles = computed(() => {
  //   return roles.value;
  // });

  async function getUser() {
    const { cookies } = useCookies();
    const Token = cookies.get("Auth-Token");
    const cacheUser = localStorage.getItem("me");
    if (!cacheUser || JSON.parse(cacheUser).token != Token) {
      return axiosinstance
        .get("/v1/auth/TokenInfo?token=" + Token)
        .then((res) => res.data)
        .then((response) => {
          console.log(localStorage.getItem("me"));
          if (response.success) {
            localStorage.setItem("me", JSON.stringify(response));
          }
          return response;
        });
    }
    return JSON.parse(cacheUser);
  }
  async function logout() {
    localStorage.removeItem("me");
    // document.getElementById("logoutForm").submit();
    axiosinstance.post("/V1/Auth/Logout").then((res) => {
      location.href = "/Identity/Account/Login";
    });
  }
  async function fetchRoles() {
    if (roles.value.length) return;
    return axiosinstance
      .get("/v1/user/roles")
      .then((res) => res.data)
      .then((response) => {
        roles.value = response;
        return response;
      });
  }
  async function fetchDepartment() {
    if (departments.value.length) return;
    return axiosinstance
      .get("/v1/user/departments")
      .then((res) => res.data)
      .then((response) => {
        departments.value = response;
        return response;
      });
  }
  async function fetchData(id) {
    return axiosinstance
      .get("/v1/user/get/" + id)
      .then((res) => res.data)
      .then((response) => {
        data.value = response;
        return response;
      });
  }
  function in_groups(groups) {
    let re = false;
    let user_roles = user.value.roles;
    if (user_roles) {
      for (let d of user_roles) {
        if (groups.indexOf(d) != -1) {
          re = true;
          break;
        }
      }
    }
    return re;
  }
  return {
    data,
    roles,
    departments,
    isAuth,
    user,
    is_admin,
    getUser,
    logout,
    fetchRoles,
    fetchDepartment,
    fetchData,
  };
});
