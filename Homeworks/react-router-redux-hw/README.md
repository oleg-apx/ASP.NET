# ДЗ: React Router + Redux + Bootstrap + HOC

## Требования задания

| Критерий | Реализация |
|----------|------------|
| React Router | Маршруты `/`, `/login`, `/register`, `*` (404) |
| Redux Toolkit | Слайс `auth`: вход / выход, данные пользователя |
| UI | **Bootstrap 5** + **react-bootstrap** |
| HOC | `withAuthPageLayout` — общая карточка для Login/Register; `withRedirectIfAuthenticated` — редирект с `/login` и `/register`, если уже вошли |

## Запуск

```bash
cd Homeworks/react-router-redux-hw
npm install
npm run dev
```

Сборка: `npm run build`, превью: `npm run preview`.

## Структура

```
src/
  components/     — навбар, layout с Outlet
  hoc/            — HOC (убирают дублирование)
  pages/          — HomePage, LoginPage, RegisterPage, NotFoundPage
  store/          — store, hooks, slices/authSlice
```

Авторизация демонстрационная (без сервера): после «Входа» / «Регистрации» состояние попадает в Redux.
