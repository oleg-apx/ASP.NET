import type { ComponentType } from 'react';
import { Navigate } from 'react-router-dom';
import { useAppSelector } from '../store/hooks';

/**
 * HOC: если пользователь уже в системе — редирект на главную
 * (общая логика для /login и /register).
 */
export function withRedirectIfAuthenticated<P extends object>(
  Component: ComponentType<P>,
): ComponentType<P> {
  function RedirectGuard(props: P) {
    const isAuthenticated = useAppSelector((s) => s.auth.isAuthenticated);
    if (isAuthenticated) {
      return <Navigate to="/" replace />;
    }
    return <Component {...props} />;
  }

  RedirectGuard.displayName = `withRedirectIfAuthenticated(${Component.displayName ?? Component.name ?? 'Component'})`;
  return RedirectGuard;
}
