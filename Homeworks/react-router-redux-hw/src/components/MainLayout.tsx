import { Container } from 'react-bootstrap';
import { Outlet } from 'react-router-dom';
import { AppNavbar } from './AppNavbar';

export function MainLayout() {
  return (
    <>
      <AppNavbar />
      <Container as="main" className="pb-5">
        <Outlet />
      </Container>
    </>
  );
}
