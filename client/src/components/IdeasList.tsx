import { useEffect, useState, useCallback } from 'react';
import * as api from '../services/api';
import { useAuth } from '../hooks/useAuth';
import { IdeaCard } from './IdeaCard';
import type { IdeaDto, PagedResult } from '../types/index';

interface IdeasListProps {
  /** Items per page — defaults to IDEAS_PAGE_SIZE from config. */
  pageSize: number;
  onAddClick: () => void;
}

function ChevronLeft() {
  return (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none"
      stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <polyline points="15 18 9 12 15 6" />
    </svg>
  );
}
function ChevronRight() {
  return (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none"
      stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <polyline points="9 18 15 12 9 6" />
    </svg>
  );
}

export function IdeasList({ pageSize, onAddClick }: IdeasListProps) {
  const { user } = useAuth();
  const [result, setResult] = useState<PagedResult<IdeaDto> | null>(null);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  /** Per-idea user ratings tracked in state after voting. */
  const [userRatings, setUserRatings] = useState<Record<string, number>>({});

  const load = useCallback(
    async (p: number) => {
      setLoading(true);
      setError(null);
      try {
        const data = await api.getIdeas(p, pageSize);
        setResult(data);
        setPage(p);
      } catch (e: unknown) {
        const err = e as { title?: string };
        setError(err?.title ?? 'Failed to load ideas');
      } finally {
        setLoading(false);
      }
    },
    [pageSize],
  );

  useEffect(() => {
    load(1);
  }, [load]);

  async function handleRate(id: string, value: number) {
    if (!user) return;
    try {
      const updated = await api.rateIdea(id, value);
      setUserRatings((prev) => ({ ...prev, [id]: value }));
      setResult((prev) => {
        if (!prev) return prev;
        return {
          ...prev,
          items: prev.items.map((item) => (item.id === id ? updated : item)),
        };
      });
    } catch (e: unknown) {
      const err = e as { title?: string };
      setError(err?.title ?? 'Failed to rate idea');
    }
  }

  async function handleDelete(id: string) {
    try {
      await api.deleteIdea(id);
      // Reload current page (or previous if the page becomes empty).
      const targetPage =
        result && result.items.length === 1 && page > 1 ? page - 1 : page;
      await load(targetPage);
    } catch (e: unknown) {
      const err = e as { title?: string };
      setError(err?.title ?? 'Failed to delete idea');
    }
  }

  const totalPages = result?.totalPages ?? 1;

  return (
    <section className="w-full">
      {/* Section header: heading + add button — matches Figma layout */}
      <div className="flex items-start justify-between mb-5">
        <div>
          <h2 className="text-2xl font-semibold text-[#1E1E1E]">Ideas</h2>
          <p className="text-sm text-[#888] mt-0.5">View them! Like them!</p>
        </div>

        {/* Add Idea button — Figma 1-488: Plus icon, neutral icon button */}
        {user && (
          <button
            type="button"
            onClick={onAddClick}
            aria-label="Add idea"
            className="flex items-center justify-center rounded-full border border-[#D9D9D9] bg-[#F5F5F5] p-3 text-[#1E1E1E] hover:bg-[#EBEBEB] transition-colors"
          >
            {/* Plus icon (Figma 1-419) */}
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none"
              stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <line x1="12" y1="5" x2="12" y2="19" />
              <line x1="5" y1="12" x2="19" y2="12" />
            </svg>
          </button>
        )}
      </div>

      {/* Error banner */}
      {error && (
        <div className="mb-4 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
          {error}
        </div>
      )}

      {/* Cards */}
      {loading ? (
        <div className="flex justify-center py-16 text-sm text-[#999]">Loading…</div>
      ) : !result || result.items.length === 0 ? (
        <div className="flex flex-col items-center justify-center py-16 text-sm text-[#999]">
          <p>No ideas yet.</p>
          {user && (
            <button
              onClick={onAddClick}
              className="mt-3 text-sm text-[#1E1E1E] underline underline-offset-2"
            >
              Add the first one
            </button>
          )}
        </div>
      ) : (
        <div className="flex flex-col gap-4">
          {result.items.map((idea) => (
            <IdeaCard
              key={idea.id}
              idea={idea}
              userRating={userRatings[idea.id]}
              onRate={handleRate}
              onDelete={handleDelete}
            />
          ))}
        </div>
      )}

      {/* Pagination */}
      {!loading && totalPages > 1 && (
        <div className="mt-8 flex items-center justify-center gap-2">
          <button
            disabled={!result?.hasPreviousPage}
            onClick={() => load(page - 1)}
            className="flex items-center justify-center rounded-full border border-[#D9D9D9] bg-[#F5F5F5] p-2 text-[#1E1E1E] hover:bg-[#EBEBEB] disabled:opacity-40 transition-colors"
            aria-label="Previous page"
          >
            <ChevronLeft />
          </button>

          {Array.from({ length: totalPages }, (_, i) => i + 1)
            .filter((p) => p === 1 || p === totalPages || Math.abs(p - page) <= 1)
            .reduce<(number | '…')[]>((acc, p, idx, arr) => {
              if (idx > 0 && (p as number) - (arr[idx - 1] as number) > 1) acc.push('…');
              acc.push(p);
              return acc;
            }, [])
            .map((p, i) =>
              p === '…' ? (
                <span key={`dots-${i}`} className="px-1 text-[#999] select-none">…</span>
              ) : (
                <button
                  key={p}
                  onClick={() => load(p as number)}
                  className={[
                    'h-9 w-9 rounded-full border text-sm font-medium transition-colors',
                    p === page
                      ? 'border-[#1E1E1E] bg-[#1E1E1E] text-white'
                      : 'border-[#D9D9D9] bg-[#F5F5F5] text-[#1E1E1E] hover:bg-[#EBEBEB]',
                  ].join(' ')}
                >
                  {p}
                </button>
              ),
            )}

          <button
            disabled={!result?.hasNextPage}
            onClick={() => load(page + 1)}
            className="flex items-center justify-center rounded-full border border-[#D9D9D9] bg-[#F5F5F5] p-2 text-[#1E1E1E] hover:bg-[#EBEBEB] disabled:opacity-40 transition-colors"
            aria-label="Next page"
          >
            <ChevronRight />
          </button>
        </div>
      )}
    </section>
  );
}
