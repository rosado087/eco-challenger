import { Routes } from '@angular/router';
import { authGuard } from '../guards/auth-guard.guard';

export const routes: Routes = [
    {
        path: '',
        pathMatch: 'full',
        loadComponent: () => {
            return import('./pages/home/home.component').then(
                (m) => m.HomeComponent
            );
        },
        canActivate: [authGuard]
    },
    {
        path: 'forgot-password',
        loadComponent: () => {
            return import(
                './pages/forgot-password/forgot-password.component'
            ).then((m) => m.ForgotPasswordComponent);
        }
    },
    {
        path: 'reset-password/:token',
        loadComponent: () => {
            return import(
                './pages/reset-password/reset-password.component'
            ).then((m) => m.ResetPasswordComponent);
        }
    },
    {
        path: 'add-username',
        loadComponent: () => {
            return import(
                './pages/add-username/add-username.component'
            ).then((m) => m.AddUsernameComponent);
        }
    },
    {
        path: 'register',
        loadComponent: () => {
            return import(
                './pages/register/register.component'
            ).then((m) => m.RegisterComponent);
        }
    },
    {
        path: 'login',
        loadComponent: () => {
            return import(
                './pages/login/login.component'
            ).then((m) => m.LoginComponent);
        }
    },
    {
        path: 'user-profile',
        loadComponent: () => {
            return import(
                './pages/user-profile/user-profile.component'
            ).then((m) => m.UserProfileComponent);
        }
    }
];
