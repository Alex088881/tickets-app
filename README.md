# 🚆 Tickets App - Поиск ж/д маршрутов

Веб-приложение для поиска железнодорожных маршрутов с использованием Yandex Rasp API.

## 🛠 Технологии

- **Backend**: ASP.NET Core 9.0, Entity Framework Core, SQLite
- **Frontend**: React, Vite, Axios
- **Infrastructure**: Docker, Docker Compose, Nginx
- **API**: Yandex Rasp API v3.0

## 🚀 Быстрый старт

### Предварительные требования
- Docker
- Docker Compose
- API ключ от [Yandex Rasp](https://yandex.ru/dev/rasp/)

### Запуск

1. Клонируйте репозиторий:
```bash
git clone <this-repo-url>
cd tickets-app

2. Создайте файл .env:
YANDEX_API_KEY=вставляйте_сюда_свой_ключ

3. Запустите приложение:
docker-compose up --build

4. Откройте в браузере: http://localhost

Структура проекта:

tickets-app/
├── backend/          # .NET API
├── frontend/         # React приложение
├── docker-compose.yml
└── README.md

API Endpoints:


GET /api/system/status - статус системы

GET /api/stations/search?name=... - поиск станций

POST /api/stations/load - загрузка станций

POST /api/route - поиск маршрутов


Docker

Backend: http://localhost:8080

Frontend: http://localhost

Database: SQLite (volumes)

