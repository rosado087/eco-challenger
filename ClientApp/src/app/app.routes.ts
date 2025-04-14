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
        path: 'user-profile/:id',
        loadComponent: () => {
            return import('./pages/user-profile/user-profile.component').then(
                (m) => m.UserProfileComponent
            )
        },
        canActivate: [authGuard]
    },
    {
        path: 'profile',
        loadComponent: () => {
            return import('./pages/user-profile/user-profile.component').then(
                (m) => m.UserProfileComponent
            )
        },
        canActivate: [authGuard]
    },
    {
        path: 'challenges/:id',
        loadComponent: () => {
            return import('./pages/challenges/challenges.component').then(
                (m) => m.ChallengesComponent
            )
        },
        canActivate: [authGuard]
    },
    {
        path: 'store',
        loadComponent: () => {
            return import('./pages/store/store.component').then(
                (m) => m.StoreComponent
            )
        },
        canActivate: [authGuard]
    },
    {
        path: 'admin',
        canActivate: [authGuard],
        children: [
            {
                path: '',
                pathMatch: 'full',
                redirectTo: '404' // Redirect /admin to 404 not found for now
            },
            {
                path: 'tags',
                loadComponent: () => {
                    return import(
                        './pages/admin/tags-admin/tags-admin.component'
                    ).then((m) => m.TagsAdminComponent)
                },
                canActivate: [authGuard],
                children: [
                    {
                        path: 'create',
                        loadComponent: () => {
                            return import(
                                './pages/admin/tags-admin/tags-admin.component'
                            ).then((m) => m.TagsAdminComponent)
                        },
                        canActivate: [authGuard]
                    },
                    {
                        path: 'edit/:id',
                        loadComponent: () => {
                            return import(
                                './pages/admin/tags-admin/tags-admin.component'
                            ).then((m) => m.TagsAdminComponent)
                        },
                        canActivate: [authGuard]
                    }
                ]
            },
            {
                path: 'challenges',
                loadComponent: () => {
                    return import(
                        './pages/admin/challenges-admin/challenges-admin.component'
                    ).then((m) => m.ChallengesAdminComponent)
                },
                canActivate: [authGuard],
                children: [
                    {
                        path: 'create',
                        loadComponent: () => {
                            return import(
                                './pages/admin/challenges-admin/challenges-admin.component'
                            ).then((m) => m.ChallengesAdminComponent)
                        },
                        canActivate: [authGuard]
                    },
                    {
                        path: 'edit/:id',
                        loadComponent: () => {
                            return import(
                                './pages/admin/challenges-admin/challenges-admin.component'
                            ).then((m) => m.ChallengesAdminComponent)
                        },
                        canActivate: [authGuard]
                    }
                ]
            },
            {
                path: 'users',
                loadComponent: () => {
                    return import(
                        './pages/admin/users-admin/users-admin.component'
                    ).then((m) => m.UsersAdminComponent)
                },
                canActivate: [authGuard]
            },
            {
                path: 'stats',
                loadComponent: () => {
                    return import(
                        './pages/admin/stats-admin/stats-admin.component'
                    ).then((m) => m.StatsAdminComponent)
                },
                canActivate: [authGuard]
            },
            {
                path: '**',
                redirectTo: '/404' // Send the other unknown URLS to the 404
            }
        ]
    },
    { path: '404', component: NotFoundComponent },
    { path: '**', redirectTo: '/404', pathMatch: 'full' }
]
