import { useEffect, useState } from 'react';

type WeatherForecast = {
  date: string;
  temperatureC: number;
  summary: string | null;
};

/** Запрос к тому же хосту, что и страница (через Kestrel + SPA-proxy). */
export default function App() {
  const [data, setData] = useState<WeatherForecast[] | null>(null);
  const [err, setErr] = useState<string | null>(null);

  useEffect(() => {
    fetch('/WeatherForecast')
      .then((r) => {
        if (!r.ok) throw new Error(`${r.status} ${r.statusText}`);
        return r.json();
      })
      .then((rows: WeatherForecast[]) => setData(rows))
      .catch((e: Error) => setErr(e.message));
  }, []);

  return (
    <>
      <h1>Один хост: React + ASP.NET Core</h1>
      <p>
        Откройте приложение через URL бэкенда (например{' '}
        <code>https://localhost:7147</code>), предварительно запустив{' '}
        <code>npm run dev</code> в папке <code>ClientApp</code>.
      </p>
      <h2>Погода (API этого же приложения)</h2>
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
