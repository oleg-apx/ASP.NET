import { useEffect, useState } from 'react';

type WeatherForecast = {
  date: string;
  temperatureC: number;
  summary: string | null;
};

const base = import.meta.env.VITE_API_BASE ?? '';

/**
 * Кросс-доменный запрос: фронт (localhost:5174) → API (localhost:5299), CORS на бэкенде.
 */
export default function App() {
  const [data, setData] = useState<WeatherForecast[] | null>(null);
  const [err, setErr] = useState<string | null>(null);

  useEffect(() => {
    const url = `${base.replace(/\/$/, '')}/WeatherForecast`;
    fetch(url)
      .then((r) => {
        if (!r.ok) throw new Error(`${r.status} ${r.statusText}`);
        return r.json();
      })
      .then((rows: WeatherForecast[]) => setData(rows))
      .catch((e: Error) => setErr(e.message));
  }, []);

  return (
    <>
      <h1>Погода с отдельного API</h1>
      <p>
        Фронт: <code>http://localhost:5174</code> · API:{' '}
        <code>{base || '(задайте VITE_API_BASE)'}</code>
      </p>
      {err && <p className="error">Ошибка: {err}</p>}
      {!err && data === null && <p>Загрузка…</p>}
      {data && (
        <table>
          <thead>
            <tr>
              <th>Дата</th>
              <th>°C</th>
              <th>Описание</th>
            </tr>
          </thead>
          <tbody>
            {data.map((row, i) => (
              <tr key={i}>
                <td>{row.date}</td>
                <td>{row.temperatureC}</td>
                <td>{row.summary}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </>
  );
}
