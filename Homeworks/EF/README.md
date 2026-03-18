# Otus.PromoCodeFactory

Проект для домашних заданий и демо по курсу `C# ASP.NET Core Разработчик` от `Отус`.
Cистема `Promocode Factory` для выдачи промокодов партнеров для клиентов по группам предпочтений.

Данный проект является стартовой точкой для домашнего задания по Entity Framework.

## PostgreSQL (Docker Compose)

```bash
docker compose up -d
```

Строка подключения по умолчанию в `appsettings.json` совпадает с контейнером (`localhost:5432`, БД `promocodefactory`, пользователь `postgres` / пароль `postgres`).

## Миграции

```bash
cd src
dotnet ef database update --project PromoCodeFactory.DataAccess --startup-project PromoCodeFactory.WebHost
```

## CI

Сборка и применение миграций к PostgreSQL настраиваются в [`.github/workflows/ci.yml`](.github/workflows/ci.yml) (GitHub Actions).