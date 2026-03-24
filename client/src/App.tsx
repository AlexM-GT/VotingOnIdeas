import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { Header } from './components/Header';
import { ProtectedRoute } from './components/ProtectedRoute';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { IdeasPage } from './pages/IdeasPage';
import { IdeaDetailPage } from './pages/IdeaDetailPage';
import { IdeaFormPage } from './pages/IdeaFormPage';

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <div className="flex min-h-screen flex-col bg-[#FAFAFA]">
          <Header />
          <main className="flex-1">
            <Routes>
              <Route path="/" element={<IdeasPage />} />
              <Route path="/login" element={<LoginPage />} />
              <Route path="/register" element={<RegisterPage />} />
              <Route path="/ideas/:id" element={<IdeaDetailPage />} />
              <Route
                path="/ideas/create"
                element={
                  <ProtectedRoute>
                    <IdeaFormPage mode="create" />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/ideas/:id/edit"
                element={
                  <ProtectedRoute>
                    <IdeaFormPage mode="edit" />
                  </ProtectedRoute>
                }
              />
              <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
          </main>
        </div>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
