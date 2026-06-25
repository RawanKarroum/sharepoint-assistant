import { useCallback, useEffect, useRef, useState } from 'react'
import { askQuestion, ApiError } from '../lib/api'
import type { ChatMessage } from '../types/api'
import { ChatInput } from './ChatInput'
import { ErrorBanner } from './ErrorBanner'
import { LoadingDots } from './LoadingDots'
import { MessageBubble } from './MessageBubble'

function createId() {
  return crypto.randomUUID()
}

export function ChatWindow() {
  const [messages, setMessages] = useState<ChatMessage[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [conversationId, setConversationId] = useState<string | undefined>()
  const bottomRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: 'smooth' })
  }, [messages, loading])

  const handleSubmit = useCallback(
    async (question: string) => {
      setError(null)
      setMessages((prev) => [
        ...prev,
        { id: createId(), role: 'user', content: question },
      ])
      setLoading(true)

      try {
        const response = await askQuestion(question, conversationId)

        if (response.conversationId) {
          setConversationId(response.conversationId)
        }

        setMessages((prev) => [
          ...prev,
          {
            id: createId(),
            role: 'assistant',
            content: response.answer,
            sources: response.sources,
          },
        ])
      } catch (err) {
        const message =
          err instanceof ApiError
            ? err.message
            : 'Something went wrong. Please try again.'
        setError(message)
      } finally {
        setLoading(false)
      }
    },
    [conversationId],
  )

  return (
    <div className="flex h-full flex-col">
      <div className="border-b border-slate-200 bg-white px-6 py-4">
        <h2 className="text-lg font-semibold text-slate-900">Chat</h2>
        <p className="text-sm text-slate-500">
          Ask questions about documents synced from OneDrive.
        </p>
      </div>

      <div
        className="flex-1 overflow-y-auto px-4 py-6 md:px-6"
        aria-live="polite"
        aria-label="Chat messages"
      >
        {messages.length === 0 && !loading && (
          <div className="flex h-full items-center justify-center">
            <div className="max-w-md text-center">
              <p className="text-base font-medium text-slate-700">
                Welcome to RAG Document Assistant
              </p>
              <p className="mt-2 text-sm text-slate-500">
                Ask a question below. Answers are generated from your indexed
                document chunks and include source links.
              </p>
            </div>
          </div>
        )}

        <div className="mx-auto max-w-3xl space-y-4">
          {messages.map((message) => (
            <MessageBubble key={message.id} message={message} />
          ))}

          {loading && (
            <div className="flex justify-start">
              <div className="rounded-2xl border border-slate-200 bg-white px-4 py-3 shadow-sm">
                <LoadingDots />
              </div>
            </div>
          )}

          {error && (
            <ErrorBanner message={error} onDismiss={() => setError(null)} />
          )}

          <div ref={bottomRef} />
        </div>
      </div>

      <div className="mx-auto w-full max-w-3xl">
        <ChatInput onSubmit={handleSubmit} disabled={loading} />
      </div>
    </div>
  )
}
