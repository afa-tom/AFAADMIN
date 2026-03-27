/**
 * 构建树形结构
 */
export function buildTree<T extends { id: number; parentId: number; children?: T[] }>(
  list: T[],
  parentId: number = 0
): T[] {
  return list
    .filter(item => item.parentId === parentId)
    .map(item => ({
      ...item,
      children: buildTree(list, item.id)
    }))
    .sort((a: any, b: any) => (a.sort || 0) - (b.sort || 0))
}

/**
 * 防抖
 */
export function debounce<T extends (...args: any[]) => any>(fn: T, delay = 300) {
  let timer: ReturnType<typeof setTimeout>
  return function (this: any, ...args: Parameters<T>) {
    clearTimeout(timer)
    timer = setTimeout(() => fn.apply(this, args), delay)
  }
}
