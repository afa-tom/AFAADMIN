import type { App, Directive } from 'vue'
import { useUserStore } from '@/store/modules/user'

/**
 * v-permission 指令
 * 用法: v-permission="'sys:user:add'"
 * 或: v-permission="['sys:user:add', 'sys:user:edit']"
 */
const permissionDirective: Directive = {
  mounted(el: HTMLElement, binding) {
    const userStore = useUserStore()
    const value = binding.value

    if (!value) return

    const codes = Array.isArray(value) ? value : [value]
    const hasPermission = userStore.permissions.includes('*:*:*')
      || codes.some(code => userStore.permissions.includes(code))

    if (!hasPermission) {
      el.parentNode?.removeChild(el)
    }
  }
}

export function setupPermissionDirective(app: App) {
  app.directive('permission', permissionDirective)
}
