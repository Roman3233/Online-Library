# 📚 Online Library

A full-stack web application for uploading, browsing, and reading books online. Users can upload PDF books with cover images, leave comments, like books, and manage their profiles.

**Live Demo:** [online-library-tau.vercel.app](https://online-library-tau.vercel.app)

---

## Features

- **Authentication** — JWT-based registration and login with role support (`user` / `admin`)
- **Books** — upload PDF books with cover images, browse, search, read online, and download
- **Likes** — like / unlike books, view liked books on profile
- **Comments** — add and delete comments on books
- **Profile** — view uploaded and liked books, edit username, delete account
- **File storage** — Cloudinary for production, local fallback for development
- **PDF viewer** — inline PDF preview via `ng2-pdf-viewer`
- **Search** — search books by title, author, or description

---

## Tech Stack

### Backend
- ASP.NET Core Web API (.NET 9)
- Entity Framework Core + PostgreSQL
- JWT Authentication
- Cloudinary (file storage)
- BCrypt.Net (password hashing)
- Swashbuckle / OpenAPI

### Frontend
- Angular 21
- TypeScript + RxJS
- Tailwind CSS v4
- `ng2-pdf-viewer`
- `jwt-decode`

### Infrastructure
- **Railway** — backend + PostgreSQL
- **Vercel** — frontend

---

## Project Structure

```
Online-Library/
├── API/
│   ├── Controllers/        # Auth, Books, Users, Comments
│   ├── Data/               # AppDbContext, DbInitializer, seed files
│   ├── DTOs/               # Request/response objects
│   ├── Middleware/         # Exception handling
│   ├── Models/             # Database entities
│   ├── Migrations/         # EF Core migrations
│   ├── Resources/          # Local book files and covers
│   ├── Services/           # CloudinaryService
│   └── Program.cs          # App configuration
└── client/
    └── src/app/
        ├── components/     # Header, Toasts
        ├── pages/          # Login, Register, BookList, BookDetail, AddBook, Profile
        ├── services/       # Auth, Book, User, Toast
        ├── guards/         # AuthGuard
        ├── interceptors/   # Auth, Error
        └── environments/   # environment.ts, environment.prod.ts
```

---

## API Endpoints

### Auth
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| POST | `/api/auth/register` | Public | Register a new user |
| POST | `/api/auth/login` | Public | Login, returns JWT |

### Books
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| GET | `/api/books` | Public | List all books, supports `?search=` |
| GET | `/api/books/{id}` | Public | Book details |
| POST | `/api/books` | Auth | Create book (`multipart/form-data`) |
| PUT | `/api/books/{id}` | Auth | Update book (`multipart/form-data`) |
| DELETE | `/api/books/{id}` | Auth | Delete book |
| GET | `/api/books/{id}/download` | Auth | Download PDF |
| GET | `/api/books/{id}/file` | Public | PDF streaming / preview |
| GET | `/api/books/user/{userId}` | Public | Books by user |
| POST | `/api/books/{id}/like` | Auth | Toggle like |

### Comments
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| GET | `/api/comments/book/{bookId}` | Public | Comments for a book |
| POST | `/api/comments` | Auth | Add comment |
| DELETE | `/api/comments/{id}` | Auth | Delete comment |

### Users
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| GET | `/api/users/{id}` | Auth | User profile with books and liked books |
| PUT | `/api/users/{id}` | Auth | Update username |
| DELETE | `/api/users/{id}` | Auth | Delete account |

---

## Running Locally

### Prerequisites
- .NET 9 SDK
- Node.js 20+
- PostgreSQL
- Cloudinary account (optional, local file storage works without it)

### Backend

```bash
cd API
```

Create `appsettings.json` (use `appsettings.example.json` as a template):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=OnlineLibraryDb;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "Key": "your-secret-key",
    "Issuer": "https://localhost:5001",
    "Audience": "https://localhost:5001"
  },
  "Cloudinary": {
    "CloudName": "",
    "ApiKey": "",
    "ApiSecret": ""
  }
}
```

```bash
dotnet ef database update
dotnet run
```

API will be available at `http://localhost:5164`. Swagger UI at `http://localhost:5164/swagger`.

### Frontend

```bash
cd client
npm install
ng serve
```

App will be available at `http://localhost:4200`.

---

## Deployment

- **Backend** — deployed on [Railway](https://railway.app) with PostgreSQL add-on
- **Frontend** — deployed on [Vercel](https://vercel.com)
- **Files** — stored on [Cloudinary](https://cloudinary.com)

Environment variables are managed through Railway and Vercel dashboards.
