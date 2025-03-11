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

    const isUnprotected = unprotectedRoutes.some((route) =>
        state.url.startsWith(route)
    )
    const isLoggedIn = authService.isLoggedIn()

    if (isUnprotected) {
        // Allow navigating to unprotected routes if not logged in
        if (!isLoggedIn) return true

        // Otherwise send to home
        router.navigate(['/'])
        return false
    }

    // If its protected and user is not logged it
    // force him to login page
    if (!isLoggedIn) {
        router.navigate(['/login'])
        return false
    }

    return true
}
