<template>
    <div class="row clearfix">
        <div class="col-12">
            <section class="card card-fluid">
                <h5 class="card-header drag-handle">
                    <a class="btn btn-success btn-sm text-white" href="/user/add">Thêm</a>
                </h5>
                <div class="card-body">
                    <div class="table-responsive-md">
                        <table id="quanlytin" class="table table-striped table-bordered table-hover" cellspacing="0"
                            width="100%">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                    <th>Hình đại diện</th>
                                    <th>Email</th>
                                    <th>Họ và tên</th>
                                    <th>Hành động</th>
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                    </div>
                </div>
            </section>
        </div>
    </div>
</template>

<script setup>
import { onMounted } from 'vue';
onMounted(() => {
    let table = $('#quanlytin').DataTable({
        "stateSave": true,
        "processing": true,
        "serverSide": true,
        // "ordering": false,
        "ajax": {
            "url": "/v1/user/table",
            "dataType": "json",
            "type": "POST",
            'data': function (data) {
            }
        },
        "columns": [{
            "data": "id",
        }, {
            "data": "image",
            "className": "text-center",
            "orderable": false
        }, {
            "data": "email",
            "className": "text-center",
            "orderable": false
        }, {
            "data": "name",
            "className": "text-center",
            "orderable": false
        },
        {
            "data": "action",
            "orderable": false
        }
        ],
    });
})
</script>