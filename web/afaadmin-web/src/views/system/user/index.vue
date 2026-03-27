<template>
  <div>
    <!-- 搜索栏 -->
    <a-card style="margin-bottom: 16px">
      <a-row :gutter="16">
        <a-col :span="5">
          <a-input v-model="query.userName" placeholder="用户名" allow-clear @clear="handleSearch" />
        </a-col>
        <a-col :span="5">
          <a-select v-model="query.status" placeholder="状态" allow-clear @clear="handleSearch">
            <a-option :value="1">正常</a-option>
            <a-option :value="0">停用</a-option>
          </a-select>
        </a-col>
        <a-col :span="5">
          <a-tree-select
            v-model="query.deptId"
            :data="deptTree"
            :field-names="{ key: 'id', title: 'deptName', children: 'children' }"
            placeholder="所属部门"
            allow-clear
            @clear="handleSearch"
          />
        </a-col>
        <a-col :span="9">
          <a-space>
            <a-button type="primary" @click="handleSearch"><icon-search /> 搜索</a-button>
            <a-button @click="handleReset"><icon-refresh /> 重置</a-button>
            <a-button v-permission="'sys:user:add'" type="primary" status="success" @click="handleAdd">
              <icon-plus /> 新增
            </a-button>
          </a-space>
        </a-col>
      </a-row>
    </a-card>

    <!-- 表格 -->
    <a-card>
      <a-table :data="tableData" :loading="loading" :pagination="pagination" @page-change="onPageChange">
        <template #columns>
          <a-table-column title="用户名" data-index="userName" />
          <a-table-column title="昵称" data-index="nickName" />
          <a-table-column title="部门" data-index="deptName" />
          <a-table-column title="手机号" data-index="phone" />
          <a-table-column title="状态" data-index="status">
            <template #cell="{ record }">
              <a-tag :color="record.status === 1 ? 'green' : 'red'">
                {{ record.status === 1 ? '正常' : '停用' }}
              </a-tag>
            </template>
          </a-table-column>
          <a-table-column title="创建时间" data-index="createTime" />
          <a-table-column title="操作" :width="240">
            <template #cell="{ record }">
              <a-space>
                <a-button v-permission="'sys:user:edit'" type="text" size="small" @click="handleEdit(record)">编辑</a-button>
                <a-button v-permission="'sys:user:resetpwd'" type="text" size="small" @click="handleResetPwd(record)">重置密码</a-button>
                <a-popconfirm content="确定删除？" @ok="handleDelete(record.id)">
                  <a-button v-permission="'sys:user:delete'" type="text" size="small" status="danger">删除</a-button>
                </a-popconfirm>
              </a-space>
            </template>
          </a-table-column>
        </template>
      </a-table>
    </a-card>

    <!-- 新增/编辑弹窗 -->
    <a-modal v-model:visible="dialogVisible" :title="isEdit ? '编辑用户' : '新增用户'" :width="560" @ok="handleSubmit">
      <a-form :model="formData" layout="vertical">
        <a-form-item v-if="!isEdit" label="用户名" required>
          <a-input v-model="formData.userName" />
        </a-form-item>
        <a-form-item v-if="!isEdit" label="密码" required>
          <a-input-password v-model="formData.password" />
        </a-form-item>
        <a-form-item label="昵称">
          <a-input v-model="formData.nickName" />
        </a-form-item>
        <a-form-item label="所属部门">
          <a-tree-select
            v-model="formData.deptId"
            :data="deptTree"
            :field-names="{ key: 'id', title: 'deptName', children: 'children' }"
            placeholder="选择部门"
            allow-clear
          />
        </a-form-item>
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="手机号"><a-input v-model="formData.phone" /></a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="邮箱"><a-input v-model="formData.email" /></a-form-item>
          </a-col>
        </a-row>
        <a-form-item label="角色">
          <a-select v-model="formData.roleIds" multiple placeholder="选择角色">
            <a-option v-for="r in roleList" :key="r.id" :value="r.id">{{ r.roleName }}</a-option>
          </a-select>
        </a-form-item>
        <a-form-item label="状态">
          <a-radio-group v-model="formData.status">
            <a-radio :value="1">正常</a-radio>
            <a-radio :value="0">停用</a-radio>
          </a-radio-group>
        </a-form-item>
        <a-form-item label="备注"><a-textarea v-model="formData.remark" /></a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { Message } from '@arco-design/web-vue'
import { IconSearch, IconRefresh, IconPlus } from '@arco-design/web-vue/es/icon'
import { getUserPage, createUser, updateUser, deleteUser, resetPassword, type UserItem } from '@/api/user'
import { getRoleList, type RoleItem } from '@/api/role'
import { getDeptTree, type DeptItem } from '@/api/dept'

const loading = ref(false)
const tableData = ref<UserItem[]>([])
const roleList = ref<RoleItem[]>([])
const deptTree = ref<DeptItem[]>([])

const query = reactive({ userName: '', status: undefined as number | undefined, deptId: undefined as number | undefined, pageIndex: 1, pageSize: 10 })
const pagination = reactive({ total: 0, current: 1, pageSize: 10 })

const dialogVisible = ref(false)
const isEdit = ref(false)
const formData = reactive({
  id: 0, userName: '', password: '', nickName: '', deptId: undefined as number | undefined,
  phone: '', email: '', status: 1, remark: '', roleIds: [] as number[]
})

onMounted(async () => {
  await loadData()
  const [roles, depts] = await Promise.all([getRoleList(), getDeptTree()])
  roleList.value = roles.data.data
  deptTree.value = depts.data.data
})

async function loadData() {
  loading.value = true
  try {
    const { data } = await getUserPage({ ...query })
    tableData.value = data.data.items
    pagination.total = data.data.totalCount
    pagination.current = query.pageIndex
  } finally { loading.value = false }
}

function handleSearch() { query.pageIndex = 1; loadData() }
function handleReset() { Object.assign(query, { userName: '', status: undefined, deptId: undefined, pageIndex: 1 }); loadData() }
function onPageChange(page: number) { query.pageIndex = page; loadData() }

function handleAdd() {
  isEdit.value = false
  Object.assign(formData, { id: 0, userName: '', password: '', nickName: '', deptId: undefined, phone: '', email: '', status: 1, remark: '', roleIds: [] })
  dialogVisible.value = true
}

function handleEdit(record: UserItem) {
  isEdit.value = true
  Object.assign(formData, { ...record, password: '' })
  dialogVisible.value = true
}

async function handleSubmit() {
  try {
    if (isEdit.value) {
      await updateUser(formData)
    } else {
      await createUser(formData)
    }
    Message.success(isEdit.value ? '修改成功' : '创建成功')
    dialogVisible.value = false
    loadData()
  } catch {}
}

async function handleDelete(id: number) {
  await deleteUser(id)
  Message.success('删除成功')
  loadData()
}

async function handleResetPwd(record: UserItem) {
  await resetPassword({ userId: record.id, newPassword: '123456' })
  Message.success('密码已重置为 123456')
}
</script>
