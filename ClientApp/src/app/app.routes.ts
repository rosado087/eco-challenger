import { Routes } from '@angular/router'
import { authGuard } from '../app/guards/auth-guard.guard'
import { NotFoundComponent } from './pages/not-found/not-found.component'

export const routes: Routes = [
    {
        path: '',
        pathMatch: 'full',
        loadComponent: () => {
            return import('./pages/home/home.component').then(
                (m) => m.HomeComponent
            )
        },
        canActivate: [authGuard]
    },
    {
        path: 'forgot-password',
        loadComponent: () => {
            return import(
                './pages/forgot-password/forgot-password.component'
            ).then((m) => m.ForgotPasswordComponent)
        },
        canActivate: [authGuard]
    },
    {
        path: 'reset-password/:token',
        loadComponent: () => {
            return import(
                './pages/reset-password/reset-password.component'
            ).then((m) => m.ResetPasswordComponent)
        },
        canActivate: [authGuard]
    },
    {
        path: 'register',
        loadComponent: () => {
            return import('./pages/register/register.component').then(
                (m) => m.RegisterComponent
            )
        },
        canActivate: [authGuard]
    },
    {
        path: 'login',
        loadComponent: () => {
            return import('./pages/login/login.component').then(
                (m) => m.LoginComponent
            )
        },
        canActivate: [authGuard]
    },
    {
        path: 'user-profile',
        loadComponent: () => {
            return import('./pages/user-profile/user-profile.component').then(
                (m) => m.UserProfileComponent
            )
        },
        canActivate: [authGuard]
    },
    { path: '404', component: NotFoundComponent },
    { path: '**', redirectTo: '/404', pathMatch: 'full' }
]
