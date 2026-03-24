import { ReactNode } from 'react';
import { renderHook, waitFor } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import { AuthProvider } from '../contexts/AuthContext';
import { useAuth } from './useAuth';

vi.mock('../services/api', () => ({
  getAccessToken: vi.fn(() => null),
  getMe: vi.fn(),
  login: vi.fn(),
  register: vi.fn(),
  logout: vi.fn(),
  clearTokens: vi.fn(),
}));

describe('useAuth', () => {
  it('throws when used outside provider', () => {
    expect(() => renderHook(() => useAuth())).toThrowError(
      'useAuth must be used inside <AuthProvider>',
    );
  });

  it('returns context value when wrapped in provider', async () => {
    function Wrapper({ children }: { children: ReactNode }) {
      return <AuthProvider>{children}</AuthProvider>;
    }

    const { result } = renderHook(() => useAuth(), { wrapper: Wrapper });

    await waitFor(() => {
      expect(result.current.isLoading).toBe(false);
    });

    expect(result.current.user).toBeNull();
    expect(typeof result.current.login).toBe('function');
    expect(typeof result.current.register).toBe('function');
    expect(typeof result.current.logout).toBe('function');
  });
});
