import { getToken } from '@/utils/auth'

const BASE_URL = import.meta.env.VITE_API_BASE_URL

export interface ChatRequest {
  sessionId?: string
  message: string
}

export interface ChatResponse {
  sessionId: string
  message: string
  structuredData?: any
  dataType: string
}

export interface ChatStreamEvent {
  event: 'start' | 'content' | 'function_call' | 'end' | 'error'
  content?: string
  sessionId?: string
  functionName?: string
  data?: any
}

export interface ChatSessionInfo {
  sessionId: string
  title: string
  createdAt: string
  lastActiveAt: string
  messageCount: number
}

export interface ChatHistoryItem {
  role: 'user' | 'assistant'
  content: string
  time: string
}

/**
 * 流式聊天（SSE）
 */
export function chatStream(
  request: ChatRequest,
  onEvent: (event: ChatStreamEvent) => void,
  onDone?: () => void,
  onError?: (err: string) => void
): AbortController {
  const controller = new AbortController()
  const token = getToken()

  fetch(`${BASE_URL}/ai/chat/stream`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    },
    body: JSON.stringify(request),
    signal: controller.signal
  })
    .then(async (response) => {
      const reader = response.body?.getReader()
      if (!reader) {
        onError?.('无法读取响应流')
        return
      }

      const decoder = new TextDecoder()
      let buffer = ''

      while (true) {
        const { done, value } = await reader.read()
        if (done) break

        buffer += decoder.decode(value, { stream: true })
        const lines = buffer.split('\n')
        buffer = lines.pop() || ''

        for (const line of lines) {
          const trimmed = line.trim()
          if (trimmed.startsWith('data: ')) {
            try {
              const data = JSON.parse(trimmed.slice(6)) as ChatStreamEvent
              onEvent(data)
            } catch {}
          }
        }
      }
      onDone?.()
    })
    .catch((err) => {
      if (err.name !== 'AbortError') {
        onError?.(err.message || '网络异常')
      }
    })

  return controller
}

/**
 * 获取会话列表
 */
export async function getSessions(): Promise<ChatSessionInfo[]> {
  const { default: service } = await import('./request')
  const { data } = await service.get('/ai/sessions')
  return data.data
}

/**
 * 获取会话历史
 */
export async function getSessionHistory(sessionId: string): Promise<ChatHistoryItem[]> {
  const { default: service } = await import('./request')
  const { data } = await service.get(`/ai/session/${sessionId}/history`)
  return data.data
}

/**
 * 删除会话
 */
export async function deleteSession(sessionId: string) {
  const { default: service } = await import('./request')
  return service.delete(`/ai/session/${sessionId}`)
}

/**
 * AI 服务状态
 */
export async function getAIStatus() {
  const { default: service } = await import('./request')
  const { data } = await service.get('/ai/status')
  return data.data
}
