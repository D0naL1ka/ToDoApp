# TodoApp

A task management app built with an Angular frontend and an ASP.NET Core backend.

## Live Demo
<p align="center">
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
<img width="1901" height="864" alt="image" src="https://github.com/user-attachments/assets/236af7ff-bbd6-48e1-b390-9ec811a031e3" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
<img width="1881" height="867" alt="image" src="https://github.com/user-attachments/assets/fedb5d0e-9277-464f-885b-2396b62c4555" />
<img width="300" height="550" alt="image" src="https://github.com/user-attachments/assets/ba31c344-5b57-447e-bde2-b7b5f896de4b" />
<img width="300" height="550" alt="image" src="https://github.com/user-attachments/assets/01395ebd-d0c7-492f-bbc6-9668f639994c" />
<img width="300" height="550" alt="image" src="https://github.com/user-attachments/assets/bb51e785-f00f-4cb2-bea8-4c9948cfdb35" />

</p>

https://splendorous-dango-8372dd.netlify.app

## Tech Stack

### Frontend
- Angular 17+ (Standalone Components)
- Bootstrap 5
- Material Symbols Outlined (icons)
- TypeScript

### Backend
- ASP.NET Core 8
- Entity Framework Core (Code First)
- SQL Server
- JWT authentication
- AutoMapper
- FluentValidation

### Infrastructure
- Frontend: Netlify
- Backend: MonsterASP.net
- Database:

**Production:** MS SQL Server (cloud-hosted databaseasp.net / MonsterASP)

**Development:** MS SQL Server (local development database)
## Features

### Authentication
Users can register by providing their first name, last name, email, and password. Authentication is handled via email and password login, with a JWT token stored in localStorage and automatically attached to every request. All routes are protected by an AuthGuard that redirects unauthenticated users to the login page.

### Side Menu
The sidebar provides quick access to five system sections: My Day shows tasks the user has explicitly added to their day, Important displays tasks marked with a star, Scheduled lists all tasks that have a due date, All Tasks shows every active task, and Completed contains finished tasks. Each section displays a counter with the current task count.

### Lists
Users can create custom lists to organize their tasks, giving each list a name. Lists can be renamed or deleted at any time. Deleting a list also permanently removes all tasks within it — the user is shown a confirmation dialog before the action is carried out.

### Tasks
Each task is created with a title and can be enriched with additional details. Users can mark a task as completed using a round checkbox, or flag it as important using the star icon. Tasks can be added to My Day, assigned a due date (which turns red with an "Overdue" label when the date has passed), and given a reminder with a specific date and time. A recurrence option allows tasks to repeat daily, weekly, or monthly. Tasks can be moved between lists, assigned one or more color-coded category labels, and given a free-text note.

### Search and Filtering
The task list supports real-time search by title, filtering by category, and sorting by default order, nearest due date, farthest due date, or newest first. Filters can be combined freely. Results are paginated with 10 tasks per page, and the page resets to 1 whenever a filter or search term changes.

### Categories
Five system categories are available out of the box (red, orange, yellow, green, and blue). Users can also create their own categories with a custom name and color. Custom categories can be edited or deleted at any time. Active categories on a task are shown as colored pill badges, and selecting a category in the filter dropdown narrows the task list accordingly.

### Responsive Design
The application is fully usable on mobile devices including iOS and Android. On small screens the sidebar slides in as a fixed overlay with a semi-transparent backdrop, and task details open as a full-screen panel. The search bar and filter dropdowns stack vertically to fit narrow viewports.

## Backend Architecture

```
TodoApp/
├── TodoApp.API/            — Controllers, middleware, extensions
├── TodoApp.Application/    — DTOs, services, interfaces, validators, mappings
├── TodoApp.Domain/         — Entities, repository interfaces
└── TodoApp.Infrastructure/ — DbContext, repositories, configurations, services
```

### Layers
- **Domain** — entities (`User`, `TaskItem`, `TaskList`, `Category`, `TaskCategory`, `RefreshToken`) and repository interfaces
- **Application** — business logic, DTOs, AutoMapper profiles, FluentValidation validators
- **Infrastructure** — EF Core DbContext, Fluent API configurations, repositories, `PasswordHasher`, `JwtService`
- **API** — controllers (`Auth`, `Tasks`, `TaskLists`, `Categories`), JWT middleware, Swagger, CORS


## Frontend Architecture

```
src/app/
├── core/
│   ├── services/       — AuthService, TaskService, TaskListService, CategoryService
│   ├── interceptors/   — JwtInterceptor
│   └── guards/         — AuthGuard
├── features/
│   ├── auth/           — Login, Register
│   └── dashboard/
│       └── tasks/      — TaskListComponent, TaskDetailComponent
└── shared/
    ├── components/     — Navbar, Sidebar, Pagination, ConfirmDialog
    └── models/         — Task, Category, TaskList, Auth models
```


## Running Locally

### Backend

```bash
# Clone the repository
git clone <repo-url>
cd TodoApp/Backend/TodoApp

# Configure appsettings.json
# Specify the connection string and JWT secret

# Apply migrations
Update-Database -StartupProject TodoApp.API

# Run
dotnet run --project TodoApp.API
```

### Frontend

```bash
cd TodoApp/Frontend/todo-app

# Install dependencies
npm install

# Run in dev mode
ng serve

# Build for production
ng build --configuration production
```

## Environment variables

Copy `appsettings.Example.json` to `appsettings.json` and fill in:

```json
{
  “ConnectionStrings”: {
    “DefaultConnection”: “Server=YOUR_SERVER;Database=TodoAppDb;...”
  },
  “JwtSettings”: {
    “SecretKey”: “YOUR_SECRET_KEY_MIN_32_CHARACTERS”,
    “Issuer”: “TodoApp”,
    “Audience”: “TodoAppUsers”,
    “ExpirationMinutes”: 60
  }
}
```
## API Endpoints

| Method | URL | Description |
|------|-----|------|
| POST | `/api/auth/register` | Register |
| POST | `/api/auth/login` | Login |
| GET | `/api/tasks` | All Tasks (paginated) |
| GET | `/api/tasks/myday` | My Day Tasks |
| GET | `/api/tasks/important` | Important Tasks |
| GET | `/api/tasks/planned` | Planned Tasks |
| POST | `/api/tasks` | Create Task |
| PUT | `/api/tasks/{id}` | Update Task |
| DELETE | `/api/tasks/{id}` | Delete Task |
| GET | `/api/tasklists` | All Lists |
| POST | `/api/tasklists` | Create List |
| PUT | `/api/tasklists/{id}` | Update List |
| DELETE | `/api/tasklists/{id}` | Delete List |
| GET | `/api/tasklists/{id}/tasks` | List Tasks |
| GET | `/api/categories` | All Categories |
| POST | `/api/categories` | Create Category |
| PUT | `/api/categories/{id}` | Update Category |
| DELETE | `/api/categories/{id}` | Delete Category |

## Contact & Socials
* **GitHub:** [D0naL1ka](https://github.com/D0naL1ka)
* **LinkedIn:** [www.linkedin.com/in/alina-suchok](https://www.linkedin.com/in/alina-suchok/)
* **Email:** alina.suchok00@gmail.com
