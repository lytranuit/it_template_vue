import { createApp } from "vue";
import { createPinia } from "pinia";

import App from "./App.vue";
import router from "./router";
import PrimeVue from "primevue/config";
import ToastService from 'primevue/toastservice';

// import "primevue/resources/themes/bootstrap4-light-blue/theme.css"; //theme
import "@/assets/styles.scss";

const app = createApp(App);

app.use(ToastService);
app.use(PrimeVue);
app.use(createPinia());
app.use(router);
app.mount("#app");

// app.config.errorHandler = () => null;
// app.config.warnHandler = () => null;

import "../src/assets/js/jquery.min.js";
import "../src/assets/js/jquery-ui.min.js";
import "../src/assets/js/bootstrap.bundle.min.js";
import "../src/assets/js/metisMenu.min.js";
import "../src/assets/lib/datatables/datatables.min.js";

import "../src/assets/lib/elfinder/js/elfinder.min.js";
import "../src/assets/lib/elfinder/js/jquery.image_v2.js";
import "../src/assets/lib/jquery-validation/dist/jquery.validate.js";
import "../src/assets/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.js";
