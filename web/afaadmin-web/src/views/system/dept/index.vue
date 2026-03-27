<template>
  <div>
    <a-card style="margin-bottom: 16px">
      <a-space>
        <a-button v-permission="'sys:dept:add'" type="primary" status="success" @click="handleAdd(0)">
          <icon-plus /> 新增部门
        </a-button>
      </a-space>
    </a-card>

    <a-card>
      <a-table :data="tableData" :loading="loading" :pagination="false" row-key="id" default-expand-all-rows>
        <template #columns>
          <a-table-column title="部门名称" data-index="deptName" />
          <a-table-column title="负责人" data-index="leader" />
          <a-table-column title="联系电话" data-index="phone" />
          <a-table-column title="排序" data-index="sort" :width="80" />
          <a-table-column title="状态" :width="80">
            <template #cell="{ record }">
              <a-tag :color="record.status === 1 ? 'green' : 'red'">
                {{ record.status === 1 ? '正常' : '停用' }}
              </a-tag>
            </template>
          </a-table-column>
          <a-table-column title="操作" :width="200">
            <template #cell="{ record }">
              <a-space>
                <a-button v-permission="'sys:dept:add'" type="text" size="small" @click="handleAdd(record.id)">添加</a-button>
                <a-button v-permission="'sys:dept:edit'" type="text" size="small" @click="handleEdit(record)">编辑</a-button>
                <a-popconfirm content="确定删除？" @ok="handleDelete(record.id)">
                  <a-button v-permission="'sys:dept:delete'" type="text" size="small" status="danger">删除</a-button>
                </a-popconfirm>
              </a-space>
            </template>
          </a-table-column>
        </template>
      </a-table>
    </a-card>

    <a-modal v-model:visible="dialogVisible" :title="isEdit ? '编辑部门' : '新增部门'" @ok="handleSubmit">
      <a-form :model="formData" layout="vertical">
        <a-form-item label="上级部门">
          <a-tree-select
            v-model="formData.parentId"
            :data="[{ id: 0, deptName: '顶级部门', children: tableData }]"
            :field-names="{ key: 'id', title: 'deptName', children: 'children' }"
          />
        </a-form-item>
        <a-form-item label="部门名称" required><a-input v-model="formData.deptName" /></a-form-item>
        <a-row :gutter="16">
          <a-col :span="12"><a-form-item label="负责人"><a-input v-model="formData.leader" /></a-form-item></a-col>
          <a-col :span="12"><a-form-item label="联系电话"><a-input v-model="formData.phone" /></a-form-item></a-col>
        </a-row>
        <a-row :gutter="16">
          <a-col :span="12"><a-form-item label="排序"><a-input-number v-model="formData.sort" /></a-form-item></a-col>
          <a-col :span="12">
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
import { getDeptTree, createDept, updateDept, deleteDept, type DeptItem } from '@/api/dept'

const loading = ref(false)
const tableData = ref<DeptItem[]>([])
const dialogVisible = ref(false)
const isEdit = ref(false)

const formData = reactive({
  id: 0, parentId: 0, deptName: '', sort: 0, leader: '', phone: '', status: 1
})

onMounted(() => loadData())

async function loadData() {
  loading.value = true
  try {
    const { data } = await getDeptTree()
    tableData.value = data.data
  } finally { loading.value = false }
}

function handleAdd(parentId: number) {
  isEdit.value = false
  Object.assign(formData, { id: 0, parentId, deptName: '', sort: 0, leader: '', phone: '', status: 1 })
  dialogVisible.value = true
}

function handleEdit(record: DeptItem) {
  isEdit.value = true
  Object.assign(formData, record)
  dialogVisible.value = true
}

async function handleSubmit() {
  try {
    if (isEdit.value) await updateDept(formData)
    else await createDept(formData)
    Message.success(isEdit.value ? '修改成功' : '创建成功')
    dialogVisible.value = false
    loadData()
  } catch {}
}

async function handleDelete(id: number) {
  await deleteDept(id)
  Message.success('删除成功')
  loadData()
}
</script>
