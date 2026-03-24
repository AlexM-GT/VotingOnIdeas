import { render, screen } from '@testing-library/react';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { ProtectedRoute } from './ProtectedRoute';

const mockUseAuth = vi.fn();

vi.mock('../hooks/useAuth', () => ({
  useAuth: () => mockUseAuth(),
}));

describe('ProtectedRoute', () => {
  beforeEach(() => {
    mockUseAuth.mockReset();
  });

  it('shows loading state while auth is loading', () => {
    mockUseAuth.mockReturnValue({ user: null, isLoading: true });

    render(
      <MemoryRouter initialEntries={['/protected']}>
        <Routes>
          <Route
            path="/protected"
            element={<ProtectedRoute><div>Private content</div></ProtectedRoute>}
          />
        </Routes>
      </MemoryRouter>,
    );

    expect(screen.getByText('Loading…')).toBeInTheDocument();
  });

  it('redirects unauthenticated users to login', () => {
    mockUseAuth.mockReturnValue({ user: null, isLoading: false });

    render(
      <MemoryRouter initialEntries={['/protected']}>
        <Routes>
          <Route
            path="/protected"
            element={<ProtectedRoute><div>Private content</div></ProtectedRoute>}
          />
          <Route path="/login" element={<div>Login page</div>} />
        </Routes>
      </MemoryRouter>,
    );

    expect(screen.getByText('Login page')).toBeInTheDocument();
  });

  it('renders children for authenticated users', () => {
    mockUseAuth.mockReturnValue({
      user: { id: 'u1', username: 'alice', email: 'alice@example.com', role: 'user' },
      isLoading: false,
    });

    render(
      <MemoryRouter initialEntries={['/protected']}>
        <Routes>
          <Route
            path="/protected"
            element={<ProtectedRoute><div>Private content</div></ProtectedRoute>}
          />
        </Routes>
      </MemoryRouter>,
    );

    expect(screen.getByText('Private content')).toBeInTheDocument();
  });
});
