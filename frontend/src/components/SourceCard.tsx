import type { SourceLink } from '../types/api'

interface SourceCardProps {
  source: SourceLink
}

export function SourceCard({ source }: SourceCardProps) {
  const hasUrl = Boolean(source.url)

  return (
    <a
      href={hasUrl ? source.url : undefined}
      target={hasUrl ? '_blank' : undefined}
      rel={hasUrl ? 'noreferrer' : undefined}
      className={`block rounded-lg border border-slate-200 bg-white p-3 transition-colors ${
        hasUrl
          ? 'hover:border-indigo-300 hover:bg-indigo-50/50 focus:outline-none focus:ring-2 focus:ring-indigo-400'
          : 'cursor-default'
      }`}
    >
      <div className="flex items-start justify-between gap-2">
        <h4 className="text-sm font-medium text-slate-900">
          {source.title || 'Untitled document'}
        </h4>
        {hasUrl && (
          <svg
            className="mt-0.5 h-4 w-4 shrink-0 text-indigo-500"
            fill="none"
            viewBox="0 0 24 24"
            strokeWidth={1.5}
            stroke="currentColor"
            aria-hidden="true"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              d="M13.5 6H5.25A2.25 2.25 0 003 8.25v10.5A2.25 2.25 0 005.25 21h10.5A2.25 2.25 0 0018 18.75V10.5m-10.5 6L21 3m0 0h-5.25M21 3v5.25"
            />
          </svg>
        )}
      </div>
      {source.snippet && (
        <p className="mt-1.5 line-clamp-3 text-xs leading-relaxed text-slate-600">
          {source.snippet}
        </p>
      )}
    </a>
  )
}
