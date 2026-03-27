<template>
  <div>
    <a-row :gutter="16">
      <!-- 左侧：字典类型 -->
      <a-col :span="10">
        <a-card title="字典类型">
          <template #extra>
            <a-button v-permission="'sys:dict:add'" type="text" size="small" @click="handleAddType">
              <icon-plus /> 新增
            </a-button>
          </template>
          <a-table
            :data="typeList"
            :loading="typeLoading"
            :pagination="false"
            :row-selection="{ type: 'radio', selectedRowKeys: selectedTypeId ? [selectedTypeId] : [] }"
            @row-click="onTypeSelect"
          >
            <template #columns>
              <a-table-column title="字典名称" data-index="dictName" />
              <a-table-column title="字典编码" data-index="dictCode" />
              <a-table-column title="操作" :width="120">
                <template #cell="{ record }">
                  <a-space>
                    <a-button v-permission="'sys:dict:edit'" type="text" size="mini" @click.stop="handleEditType(record)">编辑</a-button>
                    <a-popconfirm content="确定删除？" @ok="handleDeleteType(record.id)">
                      <a-button v-permission="'sys:dict:delete'" type="text" size="mini" status="danger" @click.stop>删除</a-button>
                    </a-popconfirm>
                  </a-space>
                </template>
              </a-table-column>
            </template>
          </a-table>
        </a-card>
      </a-col>

      <!-- 右侧：字典数据 -->
      <a-col :span="14">
        <a-card :title="'字典数据' + (selectedType ? ` - ${selectedType.dictName}` : '')">
          <template #extra>
            <a-button v-permission="'sys:dict:add'" type="text" size="small" :disabled="!selectedTypeId" @click="handleAddData">
              <icon-plus /> 新增
            </a-button>
          </template>
          <a-table :data="dataList" :loading="dataLoading" :pagination="false">
            <template #columns>
              <a-table-column title="标签" data-index="dictLabel" />
              <a-table-column title="键值" data-index="dictValue" />
              <a-table-column title="排序" data-index="sort" :width="70" />
              <a-table-column title="操作" :width="120">
                <template #cell="{ record }">
                  <a-space>
                    <a-button v-permission="'sys:dict:edit'" type="text" size="mini" @click="handleEditData(record)">编辑</a-button>
                    <a-popconfirm content="确定删除？" @ok="handleDeleteData(record.id)">
                      <a-button v-permission="'sys:dict:delete'" type="text" size="mini" status="danger">删除</a-button>
                    </a-popconfirm>
                  </a-space>
                </template>
              </a-table-column>
            </template>
          </a-table>
        </a-card>
      </a-col>
    </a-row>

    <!-- 字典类型弹窗 -->
    <a-modal v-model:visible="typeDialog" :title="isEditType ? '编辑字典类型' : '新增字典类型'" @ok="handleSubmitType">
      <a-form :model="typeForm" layout="vertical">
        <a-form-item label="字典名称" required><a-input v-model="typeForm.dictName" /></a-form-item>
        <a-form-item label="字典编码" required><a-input v-model="typeForm.dictCode" /></a-form-item>
        <a-form-item label="状态">
          <a-radio-group v-model="typeForm.status"><a-radio :value="1">正常</a-radio><a-radio :value="0">停用</a-radio></a-radio-group>
        </a-form-item>
        <a-form-item label="备注"><a-textarea v-model="typeForm.remark" /></a-form-item>
      </a-form>
    </a-modal>

    <!-- 字典数据弹窗 -->
    <a-modal v-model:visible="dataDialog" :title="isEditData ? '编辑字典数据' : '新增字典数据'" @ok="handleSubmitData">
      <a-form :model="dataForm" layout="vertical">
        <a-form-item label="标签" required><a-input v-model="dataForm.dictLabel" /></a-form-item>
        <a-form-item label="键值" required><a-input v-model="dataForm.dictValue" /></a-form-item>
        <a-row :gutter="16">
          <a-col :span="12"><a-form-item label="排序"><a-input-number v-model="dataForm.sort" /></a-form-item></a-col>
          <a-col :span="12">
            <a-form-item label="状态">
              <a-radio-group v-model="dataForm.status"><a-radio :value="1">正常</a-radio><a-radio :value="0">停用</a-radio></a-radio-group>
            </a-form-item>
          </a-col>
        </a-row>
        <a-form-item label="备注"><a-textarea v-model="dataForm.remark" /></a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { Message } from '@arco-design/web-vue'
import { IconPlus } from '@arco-design/web-vue/es/icon'
import {
  getDictTypeList, createDictType, updateDictType, deleteDictType,
  getDictDataByTypeId, createDictData, updateDictData, deleteDictData,
  type DictTypeItem, type DictDataItem
} from '@/api/dict'

const typeLoading = ref(false)
const dataLoading = ref(false)
const typeList = ref<DictTypeItem[]>([])
const dataList = ref<DictDataItem[]>([])
const selectedTypeId = ref<number | null>(null)

const selectedType = computed(() => typeList.value.find(t => t.id === selectedTypeId.value))

// 类型表单
const typeDialog = ref(false)
const isEditType = ref(false)
const typeForm = reactive({ id: 0, dictName: '', dictCode: '', status: 1, remark: '' })

// 数据表单
const dataDialog = ref(false)
const isEditData = ref(false)
const dataForm = reactive({ id: 0, dictTypeId: 0, dictLabel: '', dictValue: '', sort: 0, cssClass: '', status: 1, remark: '' })

onMounted(() => loadTypes())

async function loadTypes() {
  typeLoading.value = true
  try {
    const { data } = await getDictTypeList()
    typeList.value = data.data
  } finally { typeLoading.value = false }
}

async function loadData() {
  if (!selectedTypeId.value) return
  dataLoading.value = true
  try {
    const { data } = await getDictDataByTypeId(selectedTypeId.value)
    dataList.value = data.data
  } finally { dataLoading.value = false }
}

function onTypeSelect(record: DictTypeItem) {
  selectedTypeId.value = record.id
  loadData()
}

// 类型 CRUD
function handleAddType() {
  isEditType.value = false
  Object.assign(typeForm, { id: 0, dictName: '', dictCode: '', status: 1, remark: '' })
  typeDialog.value = true
}
function handleEditType(r: DictTypeItem) {
  isEditType.value = true; Object.assign(typeForm, r); typeDialog.value = true
}
async function handleSubmitType() {
  if (isEditType.value) await updateDictType(typeForm)
  else await createDictType(typeForm)
  Message.success('操作成功'); typeDialog.value = false; loadTypes()
}
async function handleDeleteType(id: number) {
  await deleteDictType(id); Message.success('删除成功')
  if (selectedTypeId.value === id) { selectedTypeId.value = null; dataList.value = [] }
  loadTypes()
}

// 数据 CRUD
function handleAddData() {
  isEditData.value = false
  Object.assign(dataForm, { id: 0, dictTypeId: selectedTypeId.value, dictLabel: '', dictValue: '', sort: 0, cssClass: '', status: 1, remark: '' })
  dataDialog.value = true
}
function handleEditData(r: DictDataItem) {
  isEditData.value = true; Object.assign(dataForm, r); dataDialog.value = true
}
async function handleSubmitData() {
  if (isEditData.value) await updateDictData(dataForm)
  else await createDictData(dataForm)
  Message.success('操作成功'); dataDialog.value = false; loadData()
}
async function handleDeleteData(id: number) {
  await deleteDictData(id); Message.success('删除成功'); loadData()
}
</script>
