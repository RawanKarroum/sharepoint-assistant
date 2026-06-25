export interface SourceLink {
  documentId: string
  title: string
  url: string
  snippet?: string
}

export interface ChatRequest {
  question: string
  conversationId?: string
}

export interface ChatResponse {
  answer: string
  sources: SourceLink[]
  conversationId?: string
}

export interface SyncResult {
  documentsProcessed: number
  chunksCreated: number
  syncedAt: string
}

export type ChatMessage =
  | { id: string; role: 'user'; content: string }
  | { id: string; role: 'assistant'; content: string; sources: SourceLink[] }
