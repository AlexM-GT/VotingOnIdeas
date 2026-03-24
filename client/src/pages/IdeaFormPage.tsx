import { useEffect, useState, type FormEvent } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import * as api from '../services/api';

interface IdeaFormPageProps {
  mode: 'create' | 'edit';
}

export function IdeaFormPage({ mode }: IdeaFormPageProps) {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [loading, setLoading] = useState(mode === 'edit');
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (mode === 'edit' && id) {
      api
        .getIdea(id)
        .then((idea) => {
          setTitle(idea.title);
          setDescription(idea.description);
        })
        .catch((e: { title?: string }) => setError(e?.title ?? 'Failed to load'))
        .finally(() => setLoading(false));
    }
  }, [mode, id]);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    setSaving(true);
    try {
      if (mode === 'create') {
        const idea = await api.createIdea(title, description);
        navigate(`/ideas/${idea.id}`);
      } else if (id) {
        const idea = await api.updateIdea(id, title, description);
        navigate(`/ideas/${idea.id}`);
      }
    } catch (err: unknown) {
      const e = err as { title?: string; detail?: string; errors?: Record<string, string[]> };
      const firstError =
        e?.errors ? Object.values(e.errors).flat()[0] : (e?.detail ?? e?.title ?? 'Save failed');
      setError(firstError ?? 'Save failed');
    } finally {
      setSaving(false);
    }
  }

  if (loading) return <div className="flex justify-center py-16 text-sm text-[#999]">Loading…</div>;

  return (
    <div className="mx-auto w-full max-w-xl px-6 py-8">
      <button
        onClick={() => navigate(-1)}
        className="mb-6 flex items-center gap-1 text-sm text-[#888] hover:text-[#1E1E1E] transition-colors"
      >
        ← Back
      </button>

      <div className="rounded-lg border border-[#D9D9D9] bg-white p-8">
        <h1 className="mb-6 text-xl font-semibold text-[#1E1E1E]">
          {mode === 'create' ? 'New idea' : 'Edit idea'}
        </h1>

        {error && (
          <p className="mb-4 rounded-lg border border-red-200 bg-red-50 px-4 py-2.5 text-sm text-red-700">
            {error}
          </p>
        )}

        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <label className="flex flex-col gap-1 text-sm font-medium text-[#1E1E1E]">
            Title
            <input
              type="text"
              required
              minLength={3}
              maxLength={200}
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              placeholder="Give your idea a concise title"
              className="rounded-lg border border-[#D9D9D9] px-3 py-2 text-sm outline-none focus:border-[#1E1E1E] transition-colors"
            />
          </label>

          <label className="flex flex-col gap-1 text-sm font-medium text-[#1E1E1E]">
            Description
            <textarea
              required
              minLength={10}
              maxLength={2000}
              rows={5}
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="Describe your idea in detail…"
              className="rounded-lg border border-[#D9D9D9] px-3 py-2 text-sm outline-none focus:border-[#1E1E1E] transition-colors resize-none"
            />
          </label>

          <div className="flex gap-3 mt-1">
            <button
              type="submit"
              disabled={saving}
              className="flex-1 rounded-full border border-[#1E1E1E] bg-[#1E1E1E] py-2.5 text-sm font-medium text-white hover:bg-[#333] disabled:opacity-50 transition-colors"
            >
              {saving ? 'Saving…' : mode === 'create' ? 'Create idea' : 'Save changes'}
            </button>
            <button
              type="button"
              onClick={() => navigate(-1)}
              className="flex-1 rounded-full border border-[#D9D9D9] bg-[#F5F5F5] py-2.5 text-sm font-medium text-[#1E1E1E] hover:bg-[#EBEBEB] transition-colors"
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
