import type { ChatRequest, ChatResponse, SyncResult } from '../types/api'

const LOCAL_DEV_API_BASE = 'http://localhost:5042'

const API_BASE =
  import.meta.env.VITE_API_BASE_URL?.trim() ||
  (import.meta.env.DEV ? LOCAL_DEV_API_BASE : '')

export class ApiError extends Error {
  status?: number

  constructor(message: string, status?: number) {
    super(message)
    this.name = 'ApiError'
    this.status = status
  }
}

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    let message = `Request failed with status ${response.status}`
    try {
      const body = (await response.json()) as { error?: string }
      if (body.error) message = body.error
    } catch {
      // ignore JSON parse errors
    }
    throw new ApiError(message, response.status)
  }
  return response.json() as Promise<T>
}

export async function askQuestion(
  question: string,
  conversationId?: string,
): Promise<ChatResponse> {
  const payload: ChatRequest = { question, conversationId }

  const response = await fetch(`${API_BASE}/Chat`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  })

  return handleResponse<ChatResponse>(response)
}

export async function syncDocuments(): Promise<SyncResult> {
  const response = await fetch(`${API_BASE}/Documents/sync`, {
    method: 'POST',
  })

  return handleResponse<SyncResult>(response)
}
