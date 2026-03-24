import { useNavigate } from 'react-router-dom';
import { IdeasList } from '../components/IdeasList';
import { IDEAS_PAGE_SIZE } from '../config/index';

export function IdeasPage() {
  const navigate = useNavigate();
  return (
    <div className="mx-auto w-full max-w-5xl px-6 py-8">
      <IdeasList pageSize={IDEAS_PAGE_SIZE} onAddClick={() => navigate('/ideas/create')} />
    </div>
  );
}
