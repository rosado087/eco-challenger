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
      path: 'login',
      loadComponent: () => {
        return import(
          './pages/login/login.component'
        ).then((m) => m.LoginComponent)
      }
    }
]
