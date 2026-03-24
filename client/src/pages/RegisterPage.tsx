import { useState, type FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

export function RegisterPage() {
  const { register } = useAuth();
  const navigate = useNavigate();
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      await register(username, email, password);
      navigate('/');
    } catch (err: unknown) {
      const e = err as { title?: string; detail?: string; errors?: Record<string, string[]> };
      const firstError =
        e?.errors ? Object.values(e.errors).flat()[0] : (e?.detail ?? e?.title ?? 'Registration failed');
      setError(firstError ?? 'Registration failed');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="flex min-h-[calc(100vh-65px)] items-center justify-center bg-[#FAFAFA] px-4">
      <div className="w-full max-w-sm rounded-lg border border-[#D9D9D9] bg-white p-8">
        <h1 className="mb-6 text-xl font-semibold text-[#1E1E1E]">Create account</h1>

        {error && (
          <p className="mb-4 rounded-lg border border-red-200 bg-red-50 px-4 py-2.5 text-sm text-red-700">
            {error}
          </p>
        )}

        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <label className="flex flex-col gap-1 text-sm font-medium text-[#1E1E1E]">
            Username
            <input
              type="text"
              required
              minLength={2}
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              placeholder="johndoe"
              className="rounded-lg border border-[#D9D9D9] px-3 py-2 text-sm outline-none focus:border-[#1E1E1E] transition-colors"
            />
          </label>

          <label className="flex flex-col gap-1 text-sm font-medium text-[#1E1E1E]">
            Email
            <input
              type="email"
              required
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="you@example.com"
              className="rounded-lg border border-[#D9D9D9] px-3 py-2 text-sm outline-none focus:border-[#1E1E1E] transition-colors"
            />
          </label>

          <label className="flex flex-col gap-1 text-sm font-medium text-[#1E1E1E]">
            Password
            <input
              type="password"
              required
              minLength={6}
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="min. 6 characters"
              className="rounded-lg border border-[#D9D9D9] px-3 py-2 text-sm outline-none focus:border-[#1E1E1E] transition-colors"
            />
          </label>

          <button
            type="submit"
            disabled={loading}
            className="mt-1 rounded-full border border-[#1E1E1E] bg-[#1E1E1E] py-2.5 text-sm font-medium text-white hover:bg-[#333] disabled:opacity-50 transition-colors"
          >
            {loading ? 'Creating account…' : 'Register'}
          </button>
        </form>

        <p className="mt-5 text-center text-sm text-[#888]">
          Already have an account?{' '}
          <Link to="/login" className="font-medium text-[#1E1E1E] underline underline-offset-2">
            Sign in
          </Link>
        </p>
      </div>
    </div>
  );
}
