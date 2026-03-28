import { post, get, del } from './request'
import { getToken } from '@/utils/auth'

export interface ChatRequest {
  sessionId?: string
  message: string
}

export interface ChatStreamEvent {
  event: 'start' | 'content' | 'end' | 'error'
  content?: string
  sessionId?: string
}

export interface ChatSessionInfo {
  sessionId: string
  title: string
  messageCount: number
}

export interface ChatHistoryItem {
  role: string
  content: string
  time: string
}

/**
 * 非流式聊天（移动端使用，SSE 在小程序端兼容性差）
 */
export function chat(data: ChatRequest) {
  return post<{ sessionId: string; message: string; dataType: string }>('/ai/chat', data)
}

/**
 * 获取会话列表
 */
export function getSessions() {
  return get<ChatSessionInfo[]>('/ai/sessions')
}

/**
 * 获取历史
 */
export function getHistory(sessionId: string) {
  return get<ChatHistoryItem[]>(`/ai/session/${sessionId}/history`)
}

/**
 * 删除会话
 */
export function deleteSession(sessionId: string) {
  return del(`/ai/session/${sessionId}`)
}

/**
 * AI 状态
 */
export function getAIStatus() {
  return get<{ enabled: boolean; provider: string; model: string }>('/ai/status')
}
