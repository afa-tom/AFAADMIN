import { defineStore } from 'pinia'

export interface TagItem {
  name: string
  path: string
  title: string
  affix?: boolean
}

export const useAppStore = defineStore('app', {
  state: () => ({
    collapsed: false,
    tagList: [{ name: 'Dashboard', path: '/dashboard', title: '首页', affix: true }] as TagItem[]
  }),

  actions: {
    toggleCollapse() {
      this.collapsed = !this.collapsed
    },

    addTag(tag: TagItem) {
      if (!this.tagList.find(t => t.path === tag.path)) {
        this.tagList.push(tag)
      }
    },

    removeTag(path: string) {
      this.tagList = this.tagList.filter(t => t.affix || t.path !== path)
    },

    removeOtherTags(path: string) {
      this.tagList = this.tagList.filter(t => t.affix || t.path === path)
    }
  }
})
