import type { ComponentType, ReactNode } from 'react';
import { Card, Col, Container, Row } from 'react-bootstrap';
import { Link } from 'react-router-dom';

export type AuthPageLayoutOptions = {
  /** Заголовок карточки (общий для Login / Register) */
  title: string;
  /** Текст под формой + ссылка (убирает дублирование разметки) */
  footer: ReactNode;
};

/**
 * HOC: единая обёртка для страниц входа и регистрации
 * (карточка, отступы, заголовок — без копипаста в двух компонентах).
 */
export function withAuthPageLayout<P extends object>(
  Wrapped: ComponentType<P>,
  options: AuthPageLayoutOptions,
): ComponentType<P> {
  function AuthPageLayoutComponent(props: P) {
    return (
      <Container className="py-5">
        <Row className="justify-content-center">
          <Col xs={12} md={8} lg={5}>
            <Card className="shadow-sm">
              <Card.Body className="p-4">
                <Card.Title as="h1" className="h4 mb-4 text-center">
                  {options.title}
                </Card.Title>
                <Wrapped {...props} />
                <div className="text-center mt-3 small text-muted">
                  {options.footer}
                </div>
              </Card.Body>
            </Card>
          </Col>
        </Row>
      </Container>
    );
  }

  AuthPageLayoutComponent.displayName = `withAuthPageLayout(${Wrapped.displayName ?? Wrapped.name ?? 'Component'})`;
  return AuthPageLayoutComponent;
}

/** Переиспользуемый футер со ссылкой */
export function AuthFooterLink(props: { to: string; children: ReactNode }) {
  return (
    <>
      <Link to={props.to}>{props.children}</Link>
    </>
  );
}
