import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { StarRating } from './StarRating';
import type { IdeaDto } from '../types/index';

/* ── Icon SVGs matching Figma nodes ─────────────────────────────────────────
   View  (1-490) = Search icon
   Delete(1-485) = Trash 2 icon
   ──────────────────────────────────────────────────────────────────────────── */

function SearchIcon() {
  return (
    <svg width="20" height="20" viewBox="0 0 24 24" fill="none"
      stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <circle cx="11" cy="11" r="8" />
      <line x1="21" y1="21" x2="16.65" y2="16.65" />
    </svg>
  );
}

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

/** Neutral icon button: #F5F5F5 bg, #D9D9D9 border, fully rounded, 12px pad */
function IconBtn({
  onClick,
  label,
  children,
  disabled,
}: {
  onClick: () => void;
  label: string;
  children: React.ReactNode;
  disabled?: boolean;
}) {
  return (
    <button
      type="button"
      onClick={onClick}
      disabled={disabled}
      aria-label={label}
      className="flex items-center justify-center rounded-full border border-[#D9D9D9] bg-[#F5F5F5] p-3 text-[#1E1E1E] transition-colors hover:bg-[#EBEBEB] disabled:opacity-40"
    >
      {children}
    </button>
  );
}

interface IdeaCardProps {
  idea: IdeaDto;
  userRating?: number;
  onRate: (id: string, value: number) => Promise<void>;
  onDelete: (id: string) => Promise<void>;
}

export function IdeaCard({ idea, userRating, onRate, onDelete }: IdeaCardProps) {
  const navigate = useNavigate();
  const { user } = useAuth();
  const isOwner = user?.id === idea.userId;
  const isAdmin = user?.role === 'admin';
  const canDelete = isOwner || isAdmin;
  const [deleting, setDeleting] = useState(false);

  async function handleDelete() {
    if (!confirm('Delete this idea?')) return;
    setDeleting(true);
    try {
      await onDelete(idea.id);
    } finally {
      setDeleting(false);
    }
  }

  return (
    /*
     * Card: Figma 1-469 inner card
     * bg-white, 1px solid #D9D9D9, border-radius 8px
     */
    <article className="flex flex-col overflow-hidden rounded-lg border border-[#D9D9D9] bg-white p-5">
        {/* Text */}
        <div>
          <h3 className="text-base font-semibold text-[#1E1E1E] leading-snug">{idea.title}</h3>
          <p className="mt-1.5 text-sm text-[#666] leading-relaxed line-clamp-2">{idea.description}</p>
          <p className="mt-1 text-xs text-[#BDBDBD]">by {idea.username}</p>
        </div>

        {/* Actions row */}
        <div className="flex items-center gap-2 flex-wrap">
          {/* Stars — interactive for logged-in users */}
          <StarRating
            value={idea.averageRating}
            userValue={userRating}
            onChange={user ? (v) => onRate(idea.id, v) : undefined}
          />

          {/* Vote count */}
          <span className="text-xs text-[#999]">
            {idea.voteCount} {idea.voteCount === 1 ? 'vote' : 'votes'}
            {idea.averageRating !== null && ` · Average: ${idea.averageRating.toFixed(1)}`}
          </span>

          <div className="ml-auto flex items-center gap-2">
            {/* View button – Search icon (Figma 1-490) */}
            <IconBtn onClick={() => navigate(`/ideas/${idea.id}`)} label="View">
              <SearchIcon />
            </IconBtn>

            {/* Delete button – Trash icon (Figma 1-485), owner or admin */}
            {canDelete && (
              <IconBtn onClick={handleDelete} label="Delete" disabled={deleting}>
                <TrashIcon />
              </IconBtn>
            )}
          </div>
        </div>
    </article>
  );
}
