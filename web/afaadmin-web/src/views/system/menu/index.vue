<template>
  <div>
    <a-card style="margin-bottom: 16px">
      <a-space>
        <a-button v-permission="'sys:menu:add'" type="primary" status="success" @click="handleAdd(0)">
          <icon-plus /> 新增菜单
        </a-button>
        <a-button @click="expandAll = !expandAll">
          {{ expandAll ? '收起' : '展开' }}全部
        </a-button>
      </a-space>
    </a-card>

    <a-card>
      <a-table
        :data="tableData"
        :loading="loading"
        :pagination="false"
        row-key="id"
        :default-expand-all-rows="expandAll"
      >
        <template #columns>
          <a-table-column title="菜单名称" data-index="menuName" />
          <a-table-column title="类型" data-index="menuType" :width="80">
            <template #cell="{ record }">
              <a-tag :color="typeColors[record.menuType]">{{ typeLabels[record.menuType] }}</a-tag>
            </template>
          </a-table-column>
          <a-table-column title="权限标识" data-index="permission" />
          <a-table-column title="路由" data-index="path" />
          <a-table-column title="组件" data-index="component" />
          <a-table-column title="排序" data-index="sort" :width="70" />
          <a-table-column title="可见" data-index="visible" :width="70">
            <template #cell="{ record }">
              <a-tag :color="record.visible ? 'green' : 'gray'">{{ record.visible ? '是' : '否' }}</a-tag>
            </template>
          </a-table-column>
          <a-table-column title="操作" :width="200">
            <template #cell="{ record }">
              <a-space>
                <a-button v-if="record.menuType !== 3" v-permission="'sys:menu:add'" type="text" size="small" @click="handleAdd(record.id)">添加</a-button>
                <a-button v-permission="'sys:menu:edit'" type="text" size="small" @click="handleEdit(record)">编辑</a-button>
                <a-popconfirm content="确定删除？" @ok="handleDelete(record.id)">
                  <a-button v-permission="'sys:menu:delete'" type="text" size="small" status="danger">删除</a-button>
                </a-popconfirm>
              </a-space>
            </template>
          </a-table-column>
        </template>
      </a-table>
    </a-card>

    <a-modal v-model:visible="dialogVisible" :title="isEdit ? '编辑菜单' : '新增菜单'" :width="560" @ok="handleSubmit">
      <a-form :model="formData" layout="vertical">
        <a-form-item label="父级菜单">
          <a-tree-select
            v-model="formData.parentId"
            :data="[{ id: 0, menuName: '顶级菜单', children: tableData }]"
            :field-names="{ key: 'id', title: 'menuName', children: 'children' }"
            placeholder="选择父级"
          />
        </a-form-item>
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="菜单名称" required><a-input v-model="formData.menuName" /></a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="菜单类型" required>
              <a-radio-group v-model="formData.menuType">
                <a-radio :value="1">目录</a-radio>
                <a-radio :value="2">菜单</a-radio>
                <a-radio :value="3">按钮</a-radio>
              </a-radio-group>
            </a-form-item>
          </a-col>
        </a-row>
        <a-form-item v-if="formData.menuType !== 3" label="路由地址">
          <a-input v-model="formData.path" />
        </a-form-item>
        <a-form-item v-if="formData.menuType === 2" label="组件路径">
          <a-input v-model="formData.component" placeholder="如 system/user/index" />
        </a-form-item>
        <a-form-item label="权限标识">
          <a-input v-model="formData.permission" placeholder="如 sys:user:add" />
        </a-form-item>
        <a-row :gutter="16">
          <a-col :span="8"><a-form-item label="排序"><a-input-number v-model="formData.sort" /></a-form-item></a-col>
          <a-col :span="8">
            <a-form-item label="是否可见"><a-switch v-model="formData.visible" /></a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item label="状态">
              <a-radio-group v-model="formData.status">
                <a-radio :value="1">正常</a-radio>
                <a-radio :value="0">停用</a-radio>
              </a-radio-group>
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>
    </a-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { Message } from '@arco-design/web-vue'
import { IconPlus } from '@arco-design/web-vue/es/icon'
import { getMenuTree, createMenu, updateMenu, deleteMenu, type MenuItem } from '@/api/menu'

const loading = ref(false)
const tableData = ref<MenuItem[]>([])
const dialogVisible = ref(false)
const isEdit = ref(false)
const expandAll = ref(true)

const typeLabels: Record<number, string> = { 1: '目录', 2: '菜单', 3: '按钮' }
const typeColors: Record<number, string> = { 1: 'blue', 2: 'green', 3: 'orange' }

const formData = reactive({
  id: 0, parentId: 0, menuName: '', menuType: 1, permission: '',
  path: '', component: '', icon: '', sort: 0, visible: true, status: 1
})

onMounted(() => loadData())

async function loadData() {
  loading.value = true
  try {
    const { data } = await getMenuTree()
    tableData.value = data.data
  } finally { loading.value = false }
}

function handleAdd(parentId: number) {
  isEdit.value = false
  Object.assign(formData, { id: 0, parentId, menuName: '', menuType: 1, permission: '', path: '', component: '', icon: '', sort: 0, visible: true, status: 1 })
  dialogVisible.value = true
}

function handleEdit(record: MenuItem) {
  isEdit.value = true
  Object.assign(formData, record)
  dialogVisible.value = true
}

async function handleSubmit() {
  try {
    if (isEdit.value) await updateMenu(formData)
    else await createMenu(formData)
    Message.success(isEdit.value ? '修改成功' : '创建成功')
    dialogVisible.value = false
    loadData()
  } catch {}
}

async function handleDelete(id: number) {
  await deleteMenu(id)
  Message.success('删除成功')
  loadData()
}
</script>
