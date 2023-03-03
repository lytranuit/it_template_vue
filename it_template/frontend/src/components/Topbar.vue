<script setup>
import { ref, computed, onMounted, onBeforeUnmount } from 'vue';
import { useLayout } from '../layouts/composables/layout';
import { useRouter } from 'vue-router';
import { useAuth } from '../stores/auth';
import TieredMenu from 'primevue/tieredmenu';
const store = useAuth();
const user = store.user;
const { layoutConfig, onMenuToggle, contextPath } = useLayout();


const outsideClickListener = ref(null);
const topbarMenuActive = ref(false);
const router = useRouter();

onMounted(() => {
    bindOutsideClickListener();
});

onBeforeUnmount(() => {
    unbindOutsideClickListener();
});

const logoUrl = computed(() => {
    return `${contextPath}layout/images/${layoutConfig.darkTheme.value ? 'logo-white' : 'logo-dark'}.svg`;
});

const onTopBarMenuButton = () => {
    menu1.value.toggle(event);
}
const topbarMenuClasses = computed(() => {
    return {
        'layout-topbar-menu-mobile-active': topbarMenuActive.value
    };
});

const bindOutsideClickListener = () => {
    if (!outsideClickListener.value) {
        outsideClickListener.value = (event) => {
            if (isOutsideClicked(event)) {
                topbarMenuActive.value = false;
            }
        };
        document.addEventListener('click', outsideClickListener.value);
    }
};
const unbindOutsideClickListener = () => {
    if (outsideClickListener.value) {
        document.removeEventListener('click', outsideClickListener);
        outsideClickListener.value = null;
    }
};
const isOutsideClicked = (event) => {
    if (!topbarMenuActive.value) return;

    const sidebarEl = document.querySelector('.layout-topbar-menu');
    const topbarEl = document.querySelector('.layout-topbar-menu-button');

    return !(sidebarEl.isSameNode(event.target) || sidebarEl.contains(event.target) || topbarEl.isSameNode(event.target) || topbarEl.contains(event.target));
};
const menu = ref();
const menu1 = ref();
const items = ref([
    {
        label: 'Thông tin tài khoản',
        icon: 'pi pi-fw pi-user text-muted ',
        to: "/member"
    },
    {
        label: 'Đổi mật khẩu',
        icon: 'dripicons-anchor text-muted',
        to: "/member/changepassword"
    },
    {
        separator: true
    },
    {
        label: 'Đăng xuất',
        icon: 'pi pi-fw pi-power-off text-muted',
        command: (event) => {
            // event.originalEvent: Browser event
            // event.item: Menuitem instance
            store.logout();

        }
    }
]);

const toggle = (event) => {
    menu.value.toggle(event);
}
</script>

<template>
    <div class="layout-topbar">
        <router-link to="/" class="layout-topbar-logo justify-content-center">

            <span>
                <img src="../assets/images/PMP_Stada_Group.png" alt="logo-large" class="logo-lg logo-light">
            </span>
        </router-link>

        <button class="p-link layout-menu-button layout-topbar-button" @click="onMenuToggle()">
            <i class="pi pi-bars"></i>
        </button>

        <button class="p-link layout-topbar-menu-button layout-topbar-button" @click="onTopBarMenuButton()">
            <i class="pi pi-ellipsis-v"></i>
        </button>
        <TieredMenu id="overlay_tmenu" ref="menu1" :model="items" :popup="true" style="width:200px"></TieredMenu>
        <div class="layout-topbar-menu" :class="topbarMenuClasses">
            <button class="p-link" @click="toggle" aria-haspopup="true" aria-controls="overlay_tmenu">
                <img :src="user.image_url" alt="profile-user" class="rounded-circle" width="40" />
                {{ user.fullName }}
            </button>
            <TieredMenu id="overlay_tmenu" ref="menu" :model="items" :popup="true" style="width:200px"></TieredMenu>

        </div>
    </div>
</template>
<style lang="scss" scoped></style>
