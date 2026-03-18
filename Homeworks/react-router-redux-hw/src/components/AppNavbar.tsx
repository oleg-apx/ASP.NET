import { Container, Nav, Navbar } from 'react-bootstrap';
import { Link, useNavigate } from 'react-router-dom';
import { logout } from '../store/slices/authSlice';
import { useAppDispatch, useAppSelector } from '../store/hooks';

export function AppNavbar() {
  const { isAuthenticated, user } = useAppSelector((s) => s.auth);
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  const handleLogout = () => {
    dispatch(logout());
    navigate('/');
  };

  return (
    <Navbar bg="dark" variant="dark" expand="md" className="mb-4">
      <Container>
        <Navbar.Brand as={Link} to="/">
          Promo App
        </Navbar.Brand>
        <Navbar.Toggle aria-controls="main-nav" />
        <Navbar.Collapse id="main-nav">
          <Nav className="me-auto">
            <Nav.Link as={Link} to="/">
              Главная
            </Nav.Link>
            {!isAuthenticated && (
              <>
                <Nav.Link as={Link} to="/login">
                  Вход
                </Nav.Link>
                <Nav.Link as={Link} to="/register">
                  Регистрация
                </Nav.Link>
              </>
            )}
          </Nav>
          <Nav>
            {isAuthenticated && user && (
              <>
                <Navbar.Text className="me-3 text-white-50">
                  {user.displayName} ({user.email})
                </Navbar.Text>
                <Nav.Link role="button" onClick={handleLogout}>
                  Выйти
                </Nav.Link>
              </>
            )}
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
}
