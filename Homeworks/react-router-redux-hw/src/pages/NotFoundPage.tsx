import { Button, Container } from 'react-bootstrap';
import { Link } from 'react-router-dom';

/** Страница 404 — отдельный маршрут catch-all */
export function NotFoundPage() {
  return (
    <Container
      className="d-flex flex-column align-items-center justify-content-center text-center"
      style={{ minHeight: '70vh' }}
    >
      <h1 className="display-1 fw-bold text-primary">404</h1>
      <p className="lead text-muted mb-4">
        Страница не найдена. Проверьте адрес или вернитесь на главную.
      </p>
      <Button as={Link} to="/" variant="primary" size="lg">
        На главную
      </Button>
    </Container>
  );
}
