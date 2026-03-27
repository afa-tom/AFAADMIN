<template>
  <div>
    <a-card style="margin-bottom: 16px">
      <a-space>
        <a-button v-permission="'sys:role:add'" type="primary" status="success" @click="handleAdd">
          <icon-plus /> 新增角色
        </a-button>
      </a-space>
    </a-card>

    <a-card>
      <a-table :data="tableData" :loading="loading" :pagination="false">
        <template #columns>
          <a-table-column title="角色名称" data-index="roleName" />
          <a-table-column title="角色编码" data-index="roleCode" />
          <a-table-column title="排序" data-index="sort" :width="80" />
          <a-table-column title="状态" data-index="status" :width="80">
            <template #cell="{ record }">
              <a-tag :color="record.status === 1 ? 'green' : 'red'">
                {{ record.status === 1 ? '正常' : '停用' }}
              </a-tag>
            </template>
          </a-table-column>
          <a-table-column title="创建时间" data-index="createTime" />
          <a-table-column title="操作" :width="200">
            <template #cell="{ record }">
              <a-space>
                <a-button v-permission="'sys:role:edit'" type="text" size="small" @click="handleEdit(record)">编辑</a-button>
                <a-button v-permission="'sys:role:edit'" type="text" size="small" @click="handleMenuPerm(record)">菜单权限</a-button>
                <a-popconfirm content="确定删除？" @ok="handleDelete(record.id)">
                  <a-button v-permission="'sys:role:delete'" type="text" size="small" status="danger">删除</a-button>
                </a-popconfirm>
              </a-space>
            </template>
          </a-table-column>
        </template>
      </a-table>
    </a-card>

    <!-- 角色编辑弹窗 -->
    <a-modal v-model:visible="dialogVisible" :title="isEdit ? '编辑角色' : '新增角色'" @ok="handleSubmit">
      <a-form :model="formData" layout="vertical">
        <a-form-item label="角色名称" required><a-input v-model="formData.roleName" /></a-form-item>
        <a-form-item label="角色编码" required><a-input v-model="formData.roleCode" /></a-form-item>
        <a-form-item label="排序"><a-input-number v-model="formData.sort" /></a-form-item>
        <a-form-item label="状态">
          <a-radio-group v-model="formData.status">
            <a-radio :value="1">正常</a-radio>
            <a-radio :value="0">停用</a-radio>
          </a-radio-group>
        </a-form-item>
        <a-form-item label="备注"><a-textarea v-model="formData.remark" /></a-form-item>
      </a-form>
    </a-modal>

    <!-- 菜单权限弹窗 -->
    <a-modal v-model:visible="menuDialogVisible" title="分配菜单权限" @ok="handleSubmitMenus">
      <a-tree
        v-model:checked-keys="checkedMenuIds"
        :data="menuTree"
        :field-names="{ key: 'id', title: 'menuName', children: 'children' }"
        checkable
        check-strictly
      />
    </a-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { Message } from '@arco-design/web-vue'
import { IconPlus } from '@arco-design/web-vue/es/icon'
import { getRoleList, createRole, updateRole, deleteRole, setRoleMenus, type RoleItem } from '@/api/role'
import { getMenuTree, getMenuIdsByRole, type MenuItem } from '@/api/menu'

const loading = ref(false)
const tableData = ref<RoleItem[]>([])
const menuTree = ref<MenuItem[]>([])
const dialogVisible = ref(false)
const menuDialogVisible = ref(false)
const isEdit = ref(false)
const currentRoleId = ref(0)
const checkedMenuIds = ref<number[]>([])

const formData = reactive({
  id: 0, roleName: '', roleCode: '', sort: 0, status: 1, remark: ''
})

onMounted(async () => {
  await loadData()
  const { data } = await getMenuTree()
  menuTree.value = data.data
})

async function loadData() {
  loading.value = true
  try {
    const { data } = await getRoleList()
    tableData.value = data.data
  } finally { loading.value = false }
}

function handleAdd() {
  isEdit.value = false
  Object.assign(formData, { id: 0, roleName: '', roleCode: '', sort: 0, status: 1, remark: '' })
  dialogVisible.value = true
}

function handleEdit(record: RoleItem) {
  isEdit.value = true
  Object.assign(formData, record)
  dialogVisible.value = true
}

async function handleMenuPerm(record: RoleItem) {
  currentRoleId.value = record.id
  const { data } = await getMenuIdsByRole(record.id)
  checkedMenuIds.value = data.data
  menuDialogVisible.value = true
}

async function handleSubmit() {
  try {
    if (isEdit.value) await updateRole(formData)
    else await createRole(formData)
    Message.success(isEdit.value ? '修改成功' : '创建成功')
    dialogVisible.value = false
    loadData()
  } catch {}
}

async function handleSubmitMenus() {
  await setRoleMenus(currentRoleId.value, checkedMenuIds.value)
  Message.success('菜单分配成功')
  menuDialogVisible.value = false
}

async function handleDelete(id: number) {
  await deleteRole(id)
  Message.success('删除成功')
  loadData()
}
</script>
