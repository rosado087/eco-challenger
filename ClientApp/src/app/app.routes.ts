import { Routes } from '@angular/router'

export const routes: Routes = [
    {
        path: '',
        pathMatch: 'full',
        loadComponent: () => {
            return import('./pages/home/home.component').then(
                (m) => m.HomeComponent
            )
        }
    },
    {
        path: 'forgot-password',
        loadComponent: () => {
            return import(
                './pages/forgot-password/forgot-password.component'
            ).then((m) => m.ForgotPasswordComponent)
        }
    },
    {
        path: 'reset-password/:token',
        loadComponent: () => {
            return import(
                './pages/reset-password/reset-password.component'
            ).then((m) => m.ResetPasswordComponent)
        }

  },
  {
    path: 'main-page',
    loadComponent: () => {
      return import(
        './pages/main-page/main-page.component'
      ).then((m) => m.MainPageComponent)
    }
  },
  {
    path: 'add-username',
    loadComponent: () => {
      return import(
        './pages/add-username/add-username.component'
      ).then((m) => m.AddUsernameComponent)
    }
  }
]
