export interface User {
  id: string;
  username: string;
  email: string;
  role: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  user: User;
}

export interface IdeaDto {
  id: string;
  title: string;
  description: string;
  userId: string;
  username: string;
  createdAt: string;
  averageRating: number | null;
  voteCount: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

export interface ApiError {
  type?: string;
  title: string;
  status?: number;
  detail?: string;
  errors?: Record<string, string[]>;
}
