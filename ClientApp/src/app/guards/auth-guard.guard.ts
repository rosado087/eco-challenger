import {
    CanActivateFn,
    Router,
    ActivatedRouteSnapshot,
    RouterStateSnapshot
} from '@angular/router'
import { inject } from '@angular/core'
import { AuthService } from '../services/auth/auth.service'

export const authGuard: CanActivateFn = (
    _: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
) => {
    const router: Router = inject(Router)
    const authService: AuthService = inject(AuthService)
    const unprotectedRoutes = [
        '/login',
        '/register',
        '/forgot-password',
        '/reset-password'
    ]

    // Bypass guard for external URLs such as Google Auth
    if (state.url.startsWith('http') || state.url.startsWith('//')) return true

    const isRouteUnprotected = unprotectedRoutes.some((route) =>
        state.url.startsWith(route)
    )
    const isUserLoggedIn = authService.isLoggedIn()
    const isUserAdmin = authService.getUserInfo().isAdmin

    if (isRouteUnprotected) {
        // Allow navigating to unprotected routes if not logged in
        if (!isUserLoggedIn) return true

        // Otherwise send to home
        router.navigate(['/'])
        return false
    }

    // If its protected and user is not logged it
    // force him to login page
    if (!isUserLoggedIn) {
        router.navigate(['/login'])
        return false
    }

    // Make sure he can only access admin routes if isAdmin true
    if (state.url.startsWith('/admin') && !isUserAdmin) {
        // Navigate to 404 instead of homepage so the common user
        // doesn't even know this is an existing route
        router.navigate(['/404'])
        return false
    }

    return true
}
