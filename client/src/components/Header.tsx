import { useRef, useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

/** Chevron-down SVG */
function ChevronIcon({ open }: { open: boolean }) {
  return (
    <svg
      width="16"
      height="16"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
      style={{ transition: 'transform 0.15s', transform: open ? 'rotate(180deg)' : 'none' }}
    >
      <polyline points="6 9 12 15 18 9" />
    </svg>
  );
}

/** Home icon SVG */
function HomeIcon() {
  return (
    <svg
      width="20"
      height="20"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
    >
      <path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z" />
      <polyline points="9 22 9 12 15 12 15 22" />
    </svg>
  );
}

/** Generates a deterministic background colour from a string. */
function avatarColor(name: string): string {
  const palette = [
    '#EF4444', '#F97316', '#EAB308', '#22C55E',
    '#3B82F6', '#8B5CF6', '#EC4899', '#14B8A6',
  ];
  let hash = 0;
  for (let i = 0; i < name.length; i++) hash = name.charCodeAt(i) + ((hash << 5) - hash);
  return palette[Math.abs(hash) % palette.length];
}

export function Header() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [open, setOpen] = useState(false);
  const ref = useRef<HTMLDivElement>(null);

  useEffect(() => {
    function onOutside(e: MouseEvent) {
      if (ref.current && !ref.current.contains(e.target as Node)) setOpen(false);
    }
    document.addEventListener('mousedown', onOutside);
    return () => document.removeEventListener('mousedown', onOutside);
  }, []);

  async function handleLogout() {
    setOpen(false);
    try {
      await logout();
    } catch (err) {
      console.error('Logout failed:', err);
    } finally {
      navigate('/login');
    }
  }

  const initials = user
    ? user.username.slice(0, 2).toUpperCase()
    : '?';

  return (
    <header className="w-full border-b border-[#D9D9D9] bg-white">
      <div className="mx-auto flex max-w-5xl items-center justify-between gap-3 px-6 py-4">
        {/* Home button */}
        <button
          onClick={() => navigate('/')}
          aria-label="Home"
          className="flex items-center justify-center rounded-full border border-[#D9D9D9] bg-[#F5F5F5] p-3 text-[#1E1E1E] hover:bg-[#EBEBEB] transition-colors"
        >
          <HomeIcon />
        </button>

        {/* Right section: greeting + avatar + menu */}
        <div className="flex items-center gap-3">
          {/* Greeting */}
          {user && (
            <span className="text-sm font-medium text-[#1E1E1E]">
              Hello, {user.username}!
            </span>
          )}

          {/* Avatar circle */}
          {user && (
            <div
              className="flex h-8 w-8 items-center justify-center rounded-full text-xs font-bold text-white select-none"
              style={{ backgroundColor: avatarColor(user.username) }}
            >
              {initials}
            </div>
          )}

          {/* Chevron trigger + dropdown */}
          <div className="relative" ref={ref}>
            <button
              onClick={() => setOpen((v) => !v)}
              className="flex items-center gap-1 rounded-full border border-[#D9D9D9] bg-[#F5F5F5] px-3 py-2 text-sm text-[#1E1E1E] hover:bg-[#EBEBEB] transition-colors"
            >
              {!user && <span>Account</span>}
              <ChevronIcon open={open} />
            </button>

            {open && (
              <div className="absolute right-0 top-full z-50 mt-2 w-40 overflow-hidden rounded-lg border border-[#D9D9D9] bg-white shadow-md">
                {user ? (
                  <button
                    onClick={handleLogout}
                    className="w-full px-4 py-2.5 text-left text-sm text-[#1E1E1E] hover:bg-[#F5F5F5] transition-colors"
                  >
                    Log out
                  </button>
                ) : (
                  <>
                    <button
                      onClick={() => { navigate('/login'); setOpen(false); }}
                      className="w-full px-4 py-2.5 text-left text-sm text-[#1E1E1E] hover:bg-[#F5F5F5] transition-colors border-b border-[#D9D9D9]"
                    >
                      Sign in
                    </button>
                    <button
                      onClick={() => { navigate('/register'); setOpen(false); }}
                      className="w-full px-4 py-2.5 text-left text-sm text-[#1E1E1E] hover:bg-[#F5F5F5] transition-colors"
                    >
                      Register
                    </button>
                  </>
                )}
              </div>
            )}
          </div>
        </div>
      </div>
    </header>
  );
}
