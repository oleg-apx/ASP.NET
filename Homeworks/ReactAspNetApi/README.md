# ДЗ: ASP.NET Core + React (один хост и разные хосты)

> Шаблон `dotnet new react` из старых SPA-templates целится в **netcoreapp2.2** (не поддерживается). Здесь — **.NET 9** с тем же подходом: **`Startup.cs`**, **SPA middleware**, **`UseProxyToSpaDevelopmentServer`**.

## Критерии

| Баллы | Условие | Где |
|------|---------|-----|
| **6** | Один хост: Kestrel + React | `ReactSpaSameHost` |
| **2** | Разные хосты: API + Vite | `WeatherApi` + `weather-vite-client` |
| **2** | CORS + кросс-доменный запрос | `WeatherApi/Program.cs` + страница погоды во `weather-vite-client` |

---

## Часть 1 — один хост (6 баллов)

1. Терминал — фронт (Vite, порт **5173**):

   ```bash
   cd ReactSpaSameHost/ClientApp
   npm install
   npm run dev
   ```

2. Второй терминал — бэкенд:

   ```bash
   cd ReactSpaSameHost
   dotnet run
   ```

3. Браузер: **`https://localhost:7147`** (или HTTP из `launchSettings`).  
   - HTML/React проксируются на Vite (`UseProxyToSpaDevelopmentServer`).  
   - **`GET /WeatherForecast`** обрабатывает ASP.NET — таблица погоды на главной.

**Отдельная сборка фронта:** `npm run build` в `ClientApp` → артефакты в `ClientApp/dist`.  
При **`dotnet publish`** цель `PublishRunVite` выполнит `npm install` / `npm run build` и положит файлы в `wwwroot`.

**Продакшен без proxy:** выставьте `ASPNETCORE_ENVIRONMENT=Production`, соберите ClientApp — SPA отдаётся из `dist` (при наличии папки включается `UseSpaStaticFiles`).

---

## Часть 2 — разные хосты + CORS (4 балла)

1. API (порт **5299**):

   ```bash
   cd WeatherApi
   dotnet run
   ```

2. Фронт Vite (порт **5174**, чтобы не конфликтовать с ClientApp из части 1):

   ```bash
   cd weather-vite-client
   npm install
   npm run dev
   ```

3. Откройте **`http://localhost:5174`**: страница запрашивает  
   **`http://localhost:5299/WeatherForecast`** — политика CORS **`ViteFrontend`** разрешает origin **`http://localhost:5174`**.

При смене порта фронта обновите `WithOrigins(...)` в `WeatherApi/Program.cs` и при необходимости `VITE_API_BASE` в `weather-vite-client/.env.development`.

---

## Решение

```text
ReactAspNetApi.sln
ReactSpaSameHost/          — один хост (Startup.cs, SPA)
WeatherApi/                — только Web API
weather-vite-client/       — Vite + React (отдельно)
```
