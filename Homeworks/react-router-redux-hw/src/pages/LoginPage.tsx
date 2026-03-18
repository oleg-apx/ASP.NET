import { type FormEvent, useState } from 'react';
import { Button, Form } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import { login } from '../store/slices/authSlice';
import { useAppDispatch } from '../store/hooks';
import {
  AuthFooterLink,
  withAuthPageLayout,
} from '../hoc/withAuthPageLayout';
import { withRedirectIfAuthenticated } from '../hoc/withRedirectIfAuthenticated';

function LoginForm() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    if (!email.trim()) return;
    const displayName = email.split('@')[0]?.trim() || 'Пользователь';
    dispatch(login({ email: email.trim(), displayName }));
    navigate('/', { replace: true });
  };

  return (
    <Form onSubmit={handleSubmit}>
      <Form.Group className="mb-3" controlId="loginEmail">
        <Form.Label>Email</Form.Label>
        <Form.Control
          type="email"
          placeholder="you@example.com"
          value={email}
          onChange={(ev) => setEmail(ev.target.value)}
          required
          autoComplete="email"
        />
      </Form.Group>
      <Form.Group className="mb-3" controlId="loginPassword">
        <Form.Label>Пароль</Form.Label>
        <Form.Control
          type="password"
          placeholder="••••••••"
          value={password}
          onChange={(ev) => setPassword(ev.target.value)}
          autoComplete="current-password"
        />
      </Form.Group>
      <Button variant="primary" type="submit" className="w-100">
        Войти
      </Button>
    </Form>
  );
}

const LoginWithLayout = withAuthPageLayout(LoginForm, {
  title: 'Вход',
  footer: (
    <>
      Нет аккаунта?{' '}
      <AuthFooterLink to="/register">Зарегистрироваться</AuthFooterLink>
    </>
  ),
});

/** Страница входа: HOC layout + редирект, если уже авторизован */
export const LoginPage = withRedirectIfAuthenticated(LoginWithLayout);
