import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login.component')
        .then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register/register.component')
        .then(m => m.RegisterComponent)
  },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/dashboard/dashboard.component')
        .then(m => m.DashboardComponent),
    children: [
      { path: '', redirectTo: 'tasks', pathMatch: 'full' },
      {
        path: 'tasks',
        loadComponent: () =>
          import('./features/dashboard/tasks/task-list.component')
            .then(m => m.TaskListComponent)
      },
      {
        path: 'myday',
        loadComponent: () =>
          import('./features/dashboard/tasks/task-list.component')
            .then(m => m.TaskListComponent)
      },
      {
        path: 'important',
        loadComponent: () =>
          import('./features/dashboard/tasks/task-list.component')
            .then(m => m.TaskListComponent)
      },
      {
        path: 'planned',
        loadComponent: () =>
          import('./features/dashboard/tasks/task-list.component')
            .then(m => m.TaskListComponent)
      },
      {
        path: 'list/:id',
        loadComponent: () =>
          import('./features/dashboard/tasks/task-list.component')
            .then(m => m.TaskListComponent)
      }
    ]
  },
  { path: '**', redirectTo: '' }
];