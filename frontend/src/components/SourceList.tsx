import type { SourceLink } from '../types/api'
import { SourceCard } from './SourceCard'

interface SourceListProps {
  sources: SourceLink[]
}

export function SourceList({ sources }: SourceListProps) {
  if (sources.length === 0) return null

  return (
    <div className="mt-3 border-t border-slate-200 pt-3">
      <p className="mb-2 text-xs font-semibold uppercase tracking-wide text-slate-500">
        Sources ({sources.length})
      </p>
      <div className="space-y-2">
        {sources.map((source) => (
          <SourceCard key={source.documentId} source={source} />
        ))}
      </div>
    </div>
  )
}
