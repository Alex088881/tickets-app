import { useState, useEffect } from "react";
import axios from "axios";
import "./App.css";
import * as XLSX from "xlsx";
import { saveAs } from "file-saver";

export default function App() {
  const [defaultStation, setDefaultStation] = useState("");
  const [from, setFrom] = useState("");
  const [to, setTo] = useState("");
  const [date, setDate] = useState("");
  const [note, setNote] = useState("");
  const [noteBack, setNoteBack] = useState("");
  const [routes, setRoutes] = useState([]); 

  const [fromSuggestions, setFromSuggestions] = useState([]);
  const [toSuggestions, setToSuggestions] = useState([]);
  const [frequent, setFrequent] = useState([]);

  const API = "/api";

  useEffect(() => {
    axios.get(`${API}/stations/default`)
      .then(res => {
        if (res?.data?.title) {
          setDefaultStation(res.data.title);
          setFrom(res.data.title);
        }
      })
      .catch(() => {});

    axios.get(`${API}/stations/frequently`)
      .then(res => {
        if (Array.isArray(res.data)) {
          const titles = res.data.map(station => typeof station === 'string' ? station : station.title);
          setFrequent(titles);
        } else if (res.data?.titles) {
          setFrequent(res.data.titles);
        }
      })
      .catch(() => {});
  }, []);

  function parseTimeAndStation(text) {
    if (!text && text !== "") return { time: "-", station: "-" };

    const s = String(text).trim();
    const timeRegex = /(^|\s)([0-2]?\d[:.][0-5]\d)(?=\s|,|$)/;
    const match = s.match(timeRegex);
    if (match) {
      const time = match[2];
      const station = s.replace(match[0], " ").replace(/[,]/g, "").trim() || "-";
      return { time, station };
    }

    const onlyTime = s.match(/^([0-2]?\d[:.][0-5]\d)$/);
    if (onlyTime) {
      return { time: onlyTime[1], station: "-" };
    }

    return { time: "-", station: s || "-" };
  }

  function formatDateShort(dateIsoOrInput) {
    try {
      const d = new Date(dateIsoOrInput);
      if (isNaN(d)) return "-";
      const dd = String(d.getDate()).padStart(2, "0");
      const mm = String(d.getMonth() + 1).padStart(2, "0");
      const yy = String(d.getFullYear()).slice(-2);
      return `${dd}.${mm}.${yy}`;
    } catch {
      return "-";
    }
  }


  const makeId = () => `${Date.now().toString(36)}-${Math.floor(Math.random() * 1e9).toString(36)}`;

  const findRoute = async () => {
    if (!from || !to || !date) {
      alert("Заполните поля станции и дату!");
      return;
    }

    try {
      const body = { fromStation: from, toStation: to, date: null };
      const res = await axios.post(`${API}/route`, body);

      const formattedDate = new Date(date).toLocaleDateString("ru-RU", {
        day: "2-digit",
        month: "2-digit",
        year: "2-digit",
      });

      const rawFrom = res.data?.routeFromResponses || [];
      const rawBack = res.data?.routeBackResponses || [];

      const routesThere = rawFrom.map(r => {
        const parsedDep = parseTimeAndStation(r.departure ?? "");
        const parsedArr = parseTimeAndStation(r.arrival ?? "");
        return {
          id: makeId(),
          date: formattedDate,
          arrivalTime: parsedArr.time,
          arrival: to,
          departureTime: parsedDep.time,
          departure: from,
          number: r.number ?? "-",
          note: "",
          noteBack: "",
        };
      });

      const routesBack = rawBack.map(r => {
        const parsedDep = parseTimeAndStation(r.departure ?? "");
        const parsedArr = parseTimeAndStation(r.arrival ?? "");
        return {
          id: makeId(),
          date: formattedDate,
          arrivalTime: parsedArr.time,
          arrival: from,
          departureTime: parsedDep.time,
          departure: to,
          number: r.number ?? "-",
          note: "",
          noteBack: "",
        };
      });

      setRoutes(prev => [...prev, ...routesThere, ...routesBack]);
      setFromSuggestions([]);
      setToSuggestions([]);
    } catch (err) {
      console.error(err);
      alert("Маршрут не найден или ошибка сервера");
    }
  };

  const handleFromInput = async (value) => {
    setFrom(value);
    if (value.length < 2) {
      setFromSuggestions([]);
      return;
    }
    try {
      const res = await axios.get(`${API}/stations/search?name=${value}`);
      const titles = Array.isArray(res.data)
        ? res.data.map(st => typeof st === 'string' ? st : st.title)
        : (res.data?.titles ?? []);
      setFromSuggestions(titles.slice(0, 6));
    } catch {}
  };

  const handleToInput = async (value) => {
    setTo(value);
    if (value.length < 2) {
      setToSuggestions([]);
      return;
    }
    try {
      const res = await axios.get(`${API}/stations/search?name=${value}`);
      const titles = Array.isArray(res.data)
        ? res.data.map(st => typeof st === 'string' ? st : st.title)
        : (res.data?.titles ?? []);
      setToSuggestions(titles.slice(0, 6));
    } catch {}
  };

  const saveDefault = async () => {
    if (!defaultStation) return;
    try {
      await axios.post(`${API}/stations/default?name=${defaultStation}`);
      alert("Станция по умолчанию сохранена");
    } catch {
      alert("Ошибка при сохранении");
    }
  };

  const updateRouteField = (index, field, value) => {
  setRoutes((prev) =>
    prev.map((r, i) =>
      i === index ? { ...r, [field]: value } : r
    )
  );
};

const exportToExcel = () => {
  if (routes.length === 0) {
    alert("Нет данных для экспорта");
    return;
  }

  
  const exportData = routes.map(({ date, arrivalTime, arrival, departureTime, departure, number }) => ({
    "Дата": date,
    "Время прибытия": arrivalTime,
    "Станция прибытия": arrival,
    "Время отправления": departureTime,
    "Станция отправления": departure,
    "Номер поезда": number,
  }));

  const ws = XLSX.utils.json_to_sheet(exportData);
  const wb = XLSX.utils.book_new();
  XLSX.utils.book_append_sheet(wb, ws, "Маршруты");

  const wbout = XLSX.write(wb, { bookType: "xlsx", type: "array" });
  saveAs(new Blob([wbout], { type: "application/octet-stream" }), "routes.xlsx");
};

  return (
    <div className="container">
      <h1>🚆 Поиск маршрутов</h1>

      <div className="block">
        <p><b>Станция по умолчанию:</b> {defaultStation || "не установлена"}</p>
        <div className="row">
          <input
            value={defaultStation}
            onChange={(e) => setDefaultStation(e.target.value)}
            placeholder="Введите станцию..."
          />
          <button onClick={saveDefault}>Сохранить</button>
        </div>
      </div>

      <div className="block">
        <div className="row">
          <div className="input-wrap">
            <input
              value={from}
              onChange={(e) => handleFromInput(e.target.value)}
              placeholder="Откуда"
            />
            {fromSuggestions.length > 0 && (
              <div className="suggestions">
                {fromSuggestions.map((s, i) => (
                  <div
                    key={i}
                    onClick={() => { setFrom(s); setFromSuggestions([]); }}
                  >
                    {s}
                  </div>
                ))}
              </div>
            )}
          </div>

          <div className="input-wrap">
            <input
              value={to}
              onChange={(e) => handleToInput(e.target.value)}
              placeholder="Куда"
            />
            {toSuggestions.length > 0 && (
              <div className="suggestions">
                {toSuggestions.map((s, i) => (
                  <div
                    key={i}
                    onClick={() => { setTo(s); setToSuggestions([]); }}
                  >
                    {s}
                  </div>
                ))}
              </div>
            )}
          </div>

          <input
            type="date"
            value={date}
            onChange={(e) => setDate(e.target.value)}
          />

          <button onClick={findRoute}>Найти</button>
        </div>

        <div className="frequent">
          <p>Часто используемые станции:</p>
          {frequent.map((s, i) => (
            <button key={i} onClick={() => setTo(s)}>{s}</button>
          ))}
        </div>
      </div>

      {routes.length > 0 && (
        <div className="results">
          <h2>Список поездок</h2>
          <div className="table-wrap">
            <table className="routes-table">
              <thead>
                <tr>
                  <th>Дата</th>
                  <th>Время прибытия</th>
                  <th>Станция прибытия</th>
                  <th>Заметка</th>
                  <th>Дата</th>
                  <th>Время отправления</th>
                  <th>Станция отправления</th>
                  <th>Заметка</th>
                  <th>Номер поезда</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {routes.map((r, i) => {
                  
                  const pairIndex = Math.floor(i / 2);
                  const isEvenPair = pairIndex % 2 === 0;
                  const rowClass = isEvenPair ? "even-row" : "odd-row";

                  return (
                    <tr key={r.id ?? i} className={rowClass}>
                      <td>
  <input
    value={r.date}
    onChange={(e) => updateRouteField(i, "date", e.target.value)}
  />
</td>
<td>
  <input
    value={r.arrivalTime}
    onChange={(e) => updateRouteField(i, "arrivalTime", e.target.value)}
  />
</td>
<td>
  <input
    value={r.arrival}
    onChange={(e) => updateRouteField(i, "arrival", e.target.value)}
  />
</td>
                      <td>
                        <input value={r.note} onChange={(e) => updateRouteField(i, "note", e.target.value)}>
                        </input>
                      </td>
                      <td>{r.date}</td>
                      <td>
  <input
    value={r.departureTime}
    onChange={(e) => updateRouteField(i, "departureTime", e.target.value)}
  />
</td>
<td>
  <input
    value={r.departure}
    onChange={(e) => updateRouteField(i, "departure", e.target.value)}
  />
</td>
                      <td>
                        <input value={r.noteBack} onChange={(e) => updateRouteField(i, "noteBack", e.target.value)}>
                        </input>
                      </td>
                      <td>
  <input
    value={r.number}
    onChange={(e) => updateRouteField(i, "number", e.target.value)}
  />
</td>
                      <td>
                        <button
                          className="delete-btn"
                          onClick={() =>
                            setRoutes(prev => prev.filter(item => (item.id ?? null) !== (r.id ?? null)))
                          }
                        >
                          Удалить
                        </button>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
          <button className="export-btn" onClick={exportToExcel}>📤 Экспорт в Excel</button>
        </div>
      )}
    </div>
  );
}
