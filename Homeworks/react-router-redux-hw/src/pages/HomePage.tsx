import { Button, Card } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import { useAppSelector } from '../store/hooks';

export function HomePage() {
  const { isAuthenticated, user } = useAppSelector((s) => s.auth);

  return (
    <Card className="shadow-sm">
      <Card.Body className="p-4 p-md-5">
        <Card.Title as="h1" className="h3 mb-3">
          Главная
        </Card.Title>
        {isAuthenticated && user ? (
          <>
            <p className="lead mb-4">
              Здравствуйте, <strong>{user.displayName}</strong>!
            </p>
            <p className="text-muted mb-0">
              Состояние сессии хранится в <strong>Redux</strong>. После обновления
              страницы данные сбросятся (store в памяти) — для постоянной сессии
              понадобился бы API и, например, redux-persist.
            </p>
          </>
        ) : (
          <>
            <p className="lead mb-4">
              Вы не авторизованы. Войдите или зарегистрируйтесь.
            </p>
            <div className="d-flex flex-wrap gap-2">
              <Button as={Link} to="/login" variant="primary">
                Вход
              </Button>
              <Button as={Link} to="/register" variant="outline-primary">
                Регистрация
              </Button>
            </div>
          </>
        )}
      </Card.Body>
    </Card>
  );
}
