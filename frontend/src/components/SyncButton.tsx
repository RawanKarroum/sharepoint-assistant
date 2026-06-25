import { useState } from 'react'
import { syncDocuments, ApiError } from '../lib/api'
import type { SyncResult } from '../types/api'

export function SyncButton() {
  const [loading, setLoading] = useState(false)
  const [result, setResult] = useState<SyncResult | null>(null)
  const [error, setError] = useState<string | null>(null)

  const handleSync = async () => {
    setLoading(true)
    setError(null)
    setResult(null)

    try {
      const syncResult = await syncDocuments()
      setResult(syncResult)
    } catch (err) {
      const message =
        err instanceof ApiError
          ? err.message
          : 'Failed to sync documents. Please try again.'
      setError(message)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="space-y-2">
      <button
        type="button"
        onClick={handleSync}
        disabled={loading}
        className="w-full rounded-lg border border-slate-300 bg-white px-4 py-2.5 text-sm font-medium text-slate-700 transition-colors hover:bg-slate-50 focus:outline-none focus:ring-2 focus:ring-indigo-400 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-60"
      >
        {loading ? 'Syncing documents...' : 'Sync Documents'}
      </button>

      {result && (
        <div
          className="rounded-lg border border-emerald-200 bg-emerald-50 px-3 py-2 text-xs text-emerald-800"
          role="status"
        >
          Synced {result.documentsProcessed} document
          {result.documentsProcessed === 1 ? '' : 's'}, {result.chunksCreated}{' '}
          chunk{result.chunksCreated === 1 ? '' : 's'}.
        </div>
      )}

      {error && (
        <div
          className="rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-xs text-red-800"
          role="alert"
        >
          {error}
        </div>
      )}
    </div>
  )
}
