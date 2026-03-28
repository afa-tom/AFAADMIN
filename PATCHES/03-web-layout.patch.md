# Web Layout 修改

在 `web/afaadmin-web/src/layout/index.vue` 中:

## template 部分: 在 `</a-layout>` 最后一个关闭标签前新增:
```vue
<AIAssistant />
```

## script 部分: 新增 import:
```typescript
import AIAssistant from './components/AIAssistant.vue'
```
