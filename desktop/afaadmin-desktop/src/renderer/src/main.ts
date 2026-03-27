import { createApp } from 'vue'
import App from './App.vue'

/**
 * 桌面端渲染进程入口
 *
 * 生产环境中，此处会加载与 afaadmin-web 相同的业务页面。
 * 开发阶段先提供一个精简版 Shell，展示桌面端增强能力。
 * 正式集成时，将 afaadmin-web 作为 git submodule 或 workspace 引入，
 * 替换此处的 App.vue 为 web 端的完整应用。
 */

const app = createApp(App)
app.mount('#app')
