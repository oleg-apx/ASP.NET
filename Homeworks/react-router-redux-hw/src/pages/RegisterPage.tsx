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

function RegisterForm() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const [displayName, setDisplayName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirm, setConfirm] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    setError('');
    if (password !== confirm) {
      setError('Пароли не совпадают');
      return;
    }
    if (!email.trim() || !displayName.trim()) return;
    dispatch(
      login({
        email: email.trim(),
        displayName: displayName.trim(),
      }),
    );
    navigate('/', { replace: true });
  };

  return (
    <Form onSubmit={handleSubmit}>
      {error && (
        <div className="alert alert-danger py-2 small" role="alert">
          {error}
        </div>
      )}
      <Form.Group className="mb-3" controlId="regName">
        <Form.Label>Имя</Form.Label>
        <Form.Control
          type="text"
          placeholder="Иван"
          value={displayName}
          onChange={(ev) => setDisplayName(ev.target.value)}
          required
          autoComplete="name"
        />
      </Form.Group>
      <Form.Group className="mb-3" controlId="regEmail">
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
      <Form.Group className="mb-3" controlId="regPassword">
        <Form.Label>Пароль</Form.Label>
        <Form.Control
          type="password"
          value={password}
          onChange={(ev) => setPassword(ev.target.value)}
          autoComplete="new-password"
        />
      </Form.Group>
      <Form.Group className="mb-3" controlId="regConfirm">
        <Form.Label>Подтверждение пароля</Form.Label>
        <Form.Control
          type="password"
          value={confirm}
          onChange={(ev) => setConfirm(ev.target.value)}
          autoComplete="new-password"
        />
      </Form.Group>
      <Button variant="success" type="submit" className="w-100">
        Создать аккаунт
      </Button>
    </Form>
  );
}

const RegisterWithLayout = withAuthPageLayout(RegisterForm, {
  title: 'Регистрация',
  footer: (
    <>
      Уже есть аккаунт?{' '}
      <AuthFooterLink to="/login">Войти</AuthFooterLink>
    </>
  ),
});

export const RegisterPage = withRedirectIfAuthenticated(RegisterWithLayout);
