import type {
  AuthResponse,
  IdeaDto,
  LoginRequest,
  PagedResult,
  RegisterRequest,
  User,
} from '../types/index';

const API_BASE = (import.meta.env.VITE_API_URL as string) ?? 'http://localhost:8080/api';

function getAccessToken(): string | null {
  return localStorage.getItem('access_token');
}
function getRefreshToken(): string | null {
  return localStorage.getItem('refresh_token');
}
function saveTokens(access: string, refresh: string): void {
  localStorage.setItem('access_token', access);
  localStorage.setItem('refresh_token', refresh);
}
function clearTokens(): void {
  localStorage.removeItem('access_token');
  localStorage.removeItem('refresh_token');
}

let isRefreshing = false;

async function tryRefreshTokens(): Promise<boolean> {
  const refreshToken = getRefreshToken();
  if (!refreshToken) return false;
  try {
    const res = await fetch(`${API_BASE}/auth/refresh`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken }),
    });
    if (!res.ok) {
      clearTokens();
      return false;
    }
    const data: AuthResponse = await res.json();
    saveTokens(data.accessToken, data.refreshToken);
    return true;
  } catch {
    clearTokens();
    return false;
  }
}

async function request<T>(path: string, init: RequestInit = {}): Promise<T> {
  const token = getAccessToken();
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...(init.headers as Record<string, string>),
  };
  if (token) headers['Authorization'] = `Bearer ${token}`;

  const res = await fetch(`${API_BASE}${path}`, { ...init, headers });

  if (res.status === 401 && !isRefreshing) {
    isRefreshing = true;
    const refreshed = await tryRefreshTokens();
    isRefreshing = false;
    if (refreshed) return request<T>(path, init);
    clearTokens();
    throw { title: 'Session expired. Please sign in again.', status: 401 };
  }

  if (res.status === 204) return undefined as T;

  if (!res.ok) {
    const err = await res.json().catch(() => ({ title: 'Request failed' }));
    throw err;
  }

  return res.json() as Promise<T>;
}

// ── Auth ─────────────────────────────────────────────────────────────────────

export async function login(body: LoginRequest): Promise<AuthResponse> {
  const data = await request<AuthResponse>('/auth/login', {
    method: 'POST',
    body: JSON.stringify(body),
  });
  saveTokens(data.accessToken, data.refreshToken);
  return data;
}

export async function register(body: RegisterRequest): Promise<AuthResponse> {
  const data = await request<AuthResponse>('/auth/register', {
    method: 'POST',
    body: JSON.stringify(body),
  });
  saveTokens(data.accessToken, data.refreshToken);
  return data;
}

export async function logout(): Promise<void> {
  const refreshToken = getRefreshToken();
  try {
    if (refreshToken) {
      await request<void>('/auth/logout', {
        method: 'POST',
        body: JSON.stringify({ refreshToken }),
      });
    }
  } finally {
    clearTokens();
  }
}

export async function getMe(): Promise<User> {
  return request<User>('/auth/me');
}

// ── Ideas ─────────────────────────────────────────────────────────────────────

export async function getIdeas(page: number, pageSize: number): Promise<PagedResult<IdeaDto>> {
  return request<PagedResult<IdeaDto>>(`/ideas?page=${page}&pageSize=${pageSize}`);
}

export async function getIdea(id: string): Promise<IdeaDto> {
  return request<IdeaDto>(`/ideas/${id}`);
}

export async function createIdea(title: string, description: string): Promise<IdeaDto> {
  return request<IdeaDto>('/ideas', {
    method: 'POST',
    body: JSON.stringify({ title, description }),
  });
}

export async function updateIdea(id: string, title: string, description: string): Promise<IdeaDto> {
  return request<IdeaDto>(`/ideas/${id}`, {
    method: 'PUT',
    body: JSON.stringify({ title, description }),
  });
}

export async function deleteIdea(id: string): Promise<void> {
  return request<void>(`/ideas/${id}`, { method: 'DELETE' });
}

export async function rateIdea(id: string, value: number): Promise<IdeaDto> {
  return request<IdeaDto>(`/ideas/${id}/rating`, {
    method: 'PUT',
    body: JSON.stringify({ value }),
  });
}

export { clearTokens, getAccessToken };
