# Online Library

Full-stack web application for uploading, browsing, and reading books online. Users can upload PDF books with cover images, leave comments, like books, and manage their profiles.

**Live Demo:** https://online-library-tau.vercel.app

## Features

- JWT authentication with `user` and `admin` roles
- Upload PDF books with optional cover images
- Browse, search, like, and unlike books
- Read books online with an embedded PDF viewer
- Download book files
- Comment on books and delete your own comments
- View user profiles with uploaded and liked books
- Local static file support for seeded content
- Cloudinary-backed uploads for production

## Tech Stack

### Backend

- ASP.NET Core Web API (.NET 9)
- Entity Framework Core
- PostgreSQL
- JWT authentication
- BCrypt.Net-Next
- CloudinaryDotNet
- Swashbuckle / OpenAPI

### Frontend

- Angular 21
- TypeScript
- RxJS
- Tailwind CSS v4
- ng2-pdf-viewer
- jwt-decode

### Deployment

- Railway for backend and PostgreSQL
- Vercel for frontend
- Cloudinary for file storage

## Project Structure

```text
Online-Library/
|-- API/
|   |-- Controllers/        Auth, Books, Users, Comments controllers
|   |-- Data/               DbContext, seeding, seed files
|   |-- DTOs/               Request and response models
|   |-- Middleware/         Exception handling and custom exceptions
|   |-- Migrations/         EF Core migrations
|   |-- Models/             Database entities
|   |-- Resources/          Local books and cover images
|   |-- Services/           Cloudinary service
|   `-- Program.cs          App configuration
|-- client/
|   `-- src/app/
|       |-- components/     Header, Toasts
|       |-- guards/         Auth guard
|       |-- interceptors/   Auth and error interceptors
|       |-- pages/          Login, Register, Book List, Book Detail, Add Book, Profile
|       |-- services/       Auth, Book, User, Toast services
|       `-- environments/   Environment configuration
`-- README.md
```

## API Endpoints

### Auth

| Method | Endpoint | Access | Description |
| --- | --- | --- | --- |
| POST | `/api/auth/register` | Public | Register a new user |
| POST | `/api/auth/login` | Public | Login and receive a JWT |

### Books

| Method | Endpoint | Access | Description |
| --- | --- | --- | --- |
| GET | `/api/books` | Public | List books, supports `?search=` |
| GET | `/api/books/{id}` | Public | Get book details |
| POST | `/api/books` | Auth | Create a book using `multipart/form-data` |
| PUT | `/api/books/{id}` | Auth | Update a book using `multipart/form-data` |
| DELETE | `/api/books/{id}` | Auth | Delete a book |
| GET | `/api/books/{id}/download` | Auth | Download the PDF file |
| GET | `/api/books/{id}/file` | Public | Stream the PDF for online preview |
| GET | `/api/books/user/{userId}` | Public | Get books uploaded by a user |
| POST | `/api/books/{id}/like` | Auth | Toggle like / unlike |

### Comments

| Method | Endpoint | Access | Description |
| --- | --- | --- | --- |
| GET | `/api/comments` | Public | Get all comments |
| GET | `/api/comments/book/{bookId}` | Public | Get comments for a book |
| GET | `/api/comments/{id}` | Public | Get a single comment |
| POST | `/api/comments` | Auth | Create a comment |
| DELETE | `/api/comments/{id}` | Auth | Delete a comment |

### Users

| Method | Endpoint | Access | Description |
| --- | --- | --- | --- |
| GET | `/api/users` | Auth | Get all users |
| GET | `/api/users/{id}` | Auth | Get a user profile with uploaded and liked books |
| GET | `/api/users/me` | Auth | Get the current user |
| PUT | `/api/users/{id}` | Auth | Update username |
| DELETE | `/api/users/{id}` | Auth | Delete account |

## Local Development

### Prerequisites

- .NET 9 SDK
- Node.js 20+
- PostgreSQL
- Cloudinary account

### Backend

1. Go to the backend folder:

```bash
cd API
```

2. Create `appsettings.json` based on `appsettings.example.json`.

3. Provide at least these settings:

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
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:4200"
    ]
  }
}
```

4. Apply migrations and run the API:

```bash
dotnet ef database update
dotnet run
```

The API runs at `http://localhost:5164`.

Swagger is available at `http://localhost:5164/swagger`.

### Frontend

1. Go to the frontend folder:

```bash
cd client
```

2. Install dependencies and run the app:

```bash
npm install
npm start
```

The frontend runs at `http://localhost:4200`.

## Deployment Notes

- The frontend is configured to point to the Railway API in `client/src/environments/environment.prod.ts`.
- The backend allows requests from localhost and `*.vercel.app` origins.
- File uploads use Cloudinary in production.
- Static local resources are served from `API/Resources`.
- Demo seeding runs only in Development, not on Railway.

## Environment Variables

### Backend

- `ConnectionStrings__DefaultConnection`
- `Jwt__Key`
- `Jwt__Issuer`
- `Jwt__Audience`
- `Cloudinary__CloudName`
- `Cloudinary__ApiKey`
- `Cloudinary__ApiSecret`

### Optional CORS

- `Cors__AllowedOrigins__0`
- `Cors__AllowedOrigins__1`

## Notes

- `default.jpg` is used when no cover image is selected.
- Seed data is loaded only in development.
- Uploading books currently depends on valid Cloudinary credentials.
