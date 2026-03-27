import { defineStore } from 'pinia'

export const useAppStore = defineStore('app', {
  state: () => ({
    /** 字典缓存 */
    dictCache: {} as Record<string, Array<{ label: string; value: string }>>,
    /** 网络状态 */
    networkType: 'wifi' as string
  }),

  actions: {
    setDictCache(code: string, items: Array<{ label: string; value: string }>) {
      this.dictCache[code] = items
    },

    getDictLabel(code: string, value: string): string {
      const items = this.dictCache[code]
      if (!items) return value
      const item = items.find(i => i.value === value)
      return item?.label || value
    },

    updateNetworkType() {
      uni.getNetworkType({
        success: (res) => { this.networkType = res.networkType }
      })
    }
  }
})
