<template>
    <div class="row clearfix">
        <div class="col-12">
            <form method="POST" id="form">
                <AlertError :message="messageError" v-if="messageError" />
                <section class="card card-fluid">
                    <div class="card-header">
                        <div class="d-inline-block w-100">
                            <button type="submit" class="btn btn-sm btn-primary float-right"
                                @click.prevent="submit()">Save</button>
                        </div>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-8">
                                <div class="form-group row">
                                    <b class="col-12 col-lg-2 col-form-label">Email:<i class="text-danger">*</i></b>
                                    <div class="col-12 col-lg-4 pt-1">
                                        <input class="form-control form-control-sm" type="text" name="email"
                                            placeholder="Email" v-model="data.email" required />
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <b class="col-12 col-lg-2 col-form-label">Mât khẩu:<i class="text-danger">*</i></b>
                                    <div class="col-12 col-lg-4 pt-1">
                                        <input type="password" id="password" class="form-control" name="password"
                                            minlength="6" required="" autocomplete="off" />
                                    </div>
                                    <b class="col-12 col-lg-2 col-form-label">Xác nhận mật khẩu:<i
                                            class="text-danger">*</i></b>
                                    <div class="col-12 col-lg-4 pt-1">
                                        <input type="password" class="form-control" name="confirmpassword" minlength="6"
                                            data-rule-equalTo="#password" required="" autocomplete="off">
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <b class="col-12 col-lg-2 col-form-label">Họ và tên:<i class="text-danger">*</i></b>
                                    <div class="col-12 col-lg-4 pt-1">
                                        <input class="form-control form-control-sm" type="text" name="fullName"
                                            required="" placeholder="FullName" v-model="data.fullName" />
                                    </div>
                                    <b class="col-12 col-lg-2 col-form-label">Bộ phận:</b>
                                    <div class="col-lg-4 pt-1">
                                        <treeselect :options="departments" multiple
                                            value-consists-of="ALL_WITH_INDETERMINATE" v-model="data.departments">
                                        </treeselect>
                                        <select name="departments[]" v-model="data.departments" multiple class="d-none">
                                            <option v-for="option in data.departments" :key="option" :value="option">
                                            </option>
                                        </select>
                                    </div>
                                </div>


                                <div class="form-group row">
                                    <b class="col-12 col-lg-2 col-form-label">Nhóm:</b>
                                    <div class="col-lg-4 pt-1">
                                        <treeselect :options="roles" multiple v-model="data.groups"></treeselect>
                                        <select name="groups[]" v-model="data.groups" multiple class="d-none">
                                            <option v-for="option in data.groups" :key="option" :value="option">
                                            </option>
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group row">
                                    <div class="col-12">
                                        <div class="card no-shadow border">
                                            <div class="card-header">
                                                Hình đại diện
                                            </div>
                                            <div class="card-body text-center">
                                                <div id="image_url" class="image_ft"></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </section>
            </form>
        </div>
    </div>
</template>

<script setup>
import { ref } from 'vue';
import { onMounted, computed } from 'vue';
import { useAuth } from '../../stores/auth'
import { useAxios } from "../../service/axios";
// import the component
import Treeselect from 'vue3-acies-treeselect'
// import the styles
import 'vue3-acies-treeselect/dist/vue3-treeselect.css'

import AlertError from '../../components/AlertError.vue';
import { storeToRefs } from 'pinia'
import { useRouter } from 'vue-router';
const router = useRouter();
const messageError = ref();
const { axiosinstance } = useAxios();
const store = useAuth();
const { roles, departments } = storeToRefs(store)
const data = ref({});
onMounted(() => {
    store.fetchRoles();
    store.fetchDepartment();

    $("#image_url").imageFeature({
        'image': "/private/images/user.webp",
    });

})

const submit = () => {
    var vaild = $("#form").valid();
    if (!vaild) {
        return;
    }
    let formData = $("#form").serialize()
    axiosinstance.post("/v1/user/Create", formData).then((response) => {
        return response.data
    }).then((response) => {
        messageError.value = '';
        if (response.success) {
            router.push("/user")
        } else {
            messageError.value = response.message;
        }
        // console.log(response)
    })
}
// import store from './store'
// export default {
//     data() {
//         return {
//             data: data
//         }
//     },
//     computed: {
//         roles() {
//             return store.state.roles;
//         },
//         departments() {
//             return store.state.departments;
//         }
//     },
//     mounted() {
//         var that = this;
//         $("#image_url").imageFeature({
//             'image': "/private/images/user.webp",
//         });
//         if (this.data.image_url) {
//             $("#image_url").imageFeature("set_image", this.data.image_url);
//         }
//         store.dispatch("fetchRoles");
//         store.dispatch("fetchDepartment");
//     },
//     methods: {

//     },
// }
// </script>
