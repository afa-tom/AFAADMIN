import { createApp } from 'vue'
import ArcoVue from '@arco-design/web-vue'
import '@arco-design/web-vue/dist/arco.css'
import App from './App.vue'
import router from './router'
import { createPinia } from 'pinia'
import { setupPermissionDirective } from './directive/permission'
import './styles/global.less'

const app = createApp(App)

app.use(ArcoVue)
app.use(createPinia())
app.use(router)

setupPermissionDirective(app)

app.mount('#app')
