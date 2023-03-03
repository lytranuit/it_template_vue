<template>
    <div class="row clearfix">
        <div class="col-12">
            <section class="card card-fluid">
                <div class="card-body" style="overflow:auto;position: relative;">
                    <Toast />
                    <DataTable showGridlines :value="datatable" :lazy="true" ref="dt" :paginator="true"
                        class="p-datatable-customers" :rows="10" :totalRecords="totalRecords" @page="onPage($event)"
                        :rowHover="true" filterDisplay="menu" :loading="loading" responsiveLayout="scroll"
                        :resizableColumns="true" columnResizeMode="expand">
                        <template #header>
                            <div style="width: 200px;">
                                <treeselect :options="columns" v-model="showing" multiple :limit="0"
                                    :limitText="(count) => 'Hiển thị: ' + count + ' cột'">
                                </treeselect>
                            </div>
                        </template>
                        <Column v-for="col of selectedColumns" :field="col.data" :header="col.label" :key="col.data">
                            <template #body="slotProps">
                                <div v-html="slotProps.data[col.data]"></div>
                            </template>
                        </Column>

                    </DataTable>
                </div>
            </section>
        </div>
    </div>
</template>

<script setup>
import { onMounted, ref, watch, computed } from 'vue';
import moment from 'moment';
import { useAxios } from "../../service/axios";
import Treeselect from 'vue3-acies-treeselect'
// import the styles
import 'vue3-acies-treeselect/dist/vue3-treeselect.css'
// import the component

import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import Toast from 'primevue/toast';
import { useToast } from "primevue/usetoast";
const toast = useToast();

const { axiosinstance } = useAxios();
const columns = ref([
    {
        id: 0,
        label: "ID",
        data: "id",
        className: "text-center",
    },
    {
        id: 1,
        label: "Người thay đổi",
        data: "user",
        className: "text-center",
    },
    {
        id: 2,
        label: "Ngày thay đổi",
        data: "datetime",
        className: "text-center",
    }, {
        id: 3,
        label: "Loại",
        "data": "type",
        "className": "text-center",
    }, {
        id: 4,
        label: "Mô tả",
        "data": "description",
        "className": "text-center",
    },
    {
        id: 5,
        label: "Bảng",
        "data": "tableName",
        "className": "text-center",
    },
    {
        id: 6,
        label: "Key",
        "data": "primaryKey",
        "className": "text-center",
    },
    {
        id: 7,
        label: "Giá trị cũ",
        "data": "oldValues",
        "className": "text-center",
    },
    {
        id: 8,
        label: "Giá trị mới",
        "data": "newValues",
        "className": "text-center",
    }
])
const datatable = ref();
const totalRecords = ref(0);
const loading = ref(true);
const showing = ref([]);
const selectedColumns = computed(() => {
    return columns.value.filter(col => showing.value.includes(col.id));
});
const column_cache = "columns_history";
const first = ref(0);
const rows = ref(10);
const draw = ref(0);
const lazyParams = computed(() => {
    return {
        draw: draw.value,
        start: first.value,
        length: rows.value
    }
});

const onPage = (event) => {
    first.value = event.first;
    rows.value = event.rows;
    draw.value = draw.value + 1;
    loadLazyData();
};
const loadLazyData = () => {
    loading.value = true;
    axiosinstance.post("/v1/history/table", lazyParams.value, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    }).then((res) => {
        return res.data;
    }).then((res) => {
        // console.log(res);
        datatable.value = res.data;
        totalRecords.value = res.recordsFiltered;
        loading.value = false;
    });
}


const dt = ref(null);

onMounted(() => {
    let cache = localStorage.getItem(column_cache);
    if (!cache) {
        showing.value = columns.value.map((item) => {
            return item.id;
        });
    } else {
        showing.value = JSON.parse(cache);
    }
    loadLazyData();
})
watch(showing, async (newa, old) => {
    localStorage.setItem(column_cache, JSON.stringify(newa));
})
</script>