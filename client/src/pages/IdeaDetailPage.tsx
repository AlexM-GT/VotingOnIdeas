import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import * as api from '../services/api';
import { useAuth } from '../hooks/useAuth';
import { StarRating } from '../components/StarRating';
import type { IdeaDto } from '../types/index';

function TrashIcon() {
  return (
    <svg width="20" height="20" viewBox="0 0 24 24" fill="none"
      stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <polyline points="3 6 5 6 21 6" />
      <path d="M19 6l-1 14a2 2 0 01-2 2H8a2 2 0 01-2-2L5 6" />
      <path d="M10 11v6M14 11v6" />
      <path d="M9 6V4a1 1 0 011-1h4a1 1 0 011 1v2" />
    </svg>
  );
}
function EditIcon() {
  return (
    <svg width="20" height="20" viewBox="0 0 24 24" fill="none"
      stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7" />
      <path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4 9.5-9.5z" />
    </svg>
  );
}

export function IdeaDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { user } = useAuth();
  const [idea, setIdea] = useState<IdeaDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [userRating, setUserRating] = useState<number | undefined>();
  const [deleting, setDeleting] = useState(false);

  useEffect(() => {
    if (!id) return;
    api
      .getIdea(id)
      .then(setIdea)
      .catch((e: { title?: string }) => setError(e?.title ?? 'Not found'))
      .finally(() => setLoading(false));
  }, [id]);

  async function handleRate(value: number) {
    if (!id) return;
    try {
      const updated = await api.rateIdea(id, value);
      setIdea(updated);
      setUserRating(value);
    } catch (e: unknown) {
      const err = e as { title?: string };
      setError(err?.title ?? 'Failed to rate');
    }
  }

  async function handleDelete() {
    if (!idea || !confirm('Delete this idea?')) return;
    setDeleting(true);
    try {
      await api.deleteIdea(idea.id);
      navigate('/');
    } catch (e: unknown) {
      const err = e as { title?: string };
      setError(err?.title ?? 'Failed to delete');
      setDeleting(false);
    }
  }

  if (loading) return <div className="flex justify-center py-16 text-sm text-[#999]">Loading…</div>;
  if (error || !idea) {
    return (
      <div className="mx-auto max-w-5xl px-6 py-8">
        <p className="text-red-600">{error ?? 'Idea not found'}</p>
        <button onClick={() => navigate('/')} className="mt-4 text-sm underline text-[#1E1E1E]">
          Back to ideas
        </button>
      </div>
    );
  }

  const isOwner = user?.id === idea.userId;
  const isAdmin = user?.role === 'admin';
  const canManage = isOwner || isAdmin;

  return (
    <div className="mx-auto max-w-5xl px-6 py-8">
      {/* Back */}
      <button
        onClick={() => navigate('/')}
        className="mb-6 flex items-center gap-1 text-sm text-[#888] hover:text-[#1E1E1E] transition-colors"
      >
        ← Back
      </button>

      <div className="rounded-lg border border-[#D9D9D9] bg-white p-6">
        <div className="flex flex-col gap-4">
          {/* Title + owner actions */}
          <div className="flex items-start justify-between gap-4">
            <h1 className="text-2xl font-semibold text-[#1E1E1E]">{idea.title}</h1>
            {canManage && (
              <div className="flex items-center gap-2 shrink-0">
                {isOwner && (
                  <button
                    onClick={() => navigate(`/ideas/${idea.id}/edit`)}
                    aria-label="Edit"
                    className="flex items-center justify-center rounded-full border border-[#D9D9D9] bg-[#F5F5F5] p-3 text-[#1E1E1E] hover:bg-[#EBEBEB] transition-colors"
                  >
                    <EditIcon />
                  </button>
                )}
                <button
                  onClick={handleDelete}
                  disabled={deleting}
                  aria-label="Delete"
                  className="flex items-center justify-center rounded-full border border-[#D9D9D9] bg-[#F5F5F5] p-3 text-[#1E1E1E] hover:bg-[#EBEBEB] disabled:opacity-40 transition-colors"
                >
                  <TrashIcon />
                </button>
              </div>
            )}
          </div>

          <p className="text-sm text-[#888]">by {idea.username}</p>
          <p className="text-sm leading-relaxed text-[#444]">{idea.description}</p>

          {/* Rating */}
          <div className="border-t border-[#D9D9D9] pt-4">
            <p className="mb-2 text-xs font-medium uppercase tracking-wide text-[#888]">
              {user ? 'Your rating' : 'Community rating'}
            </p>
            <div className="flex items-center gap-3">
              <StarRating
                value={idea.averageRating}
                userValue={userRating}
                onChange={user ? handleRate : undefined}
              />
              <span className="text-sm text-[#999]">
                {idea.voteCount} {idea.voteCount === 1 ? 'vote' : 'votes'}
                {idea.averageRating !== null && ` · Average: ${idea.averageRating.toFixed(1)}`}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
