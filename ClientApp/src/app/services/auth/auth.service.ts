import { inject, Injectable } from '@angular/core'
import { AuthUserInfo } from '../../models/auth-user-info'
import { CookieService } from 'ngx-cookie-service'

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private cookieService = inject(CookieService)

    private mapUserCookies(user: AuthUserInfo) {
        this.cookieService.set('auth_id', user.id.toString())
        this.cookieService.set('auth_username', user.username)
        this.cookieService.set('auth_email', user.email)
        this.cookieService.set('auth_isAdmin', user.isAdmin.toString())
    }

    login(userInfo: AuthUserInfo, token: string) {
        this.mapUserCookies(userInfo) // Save user data

        this.cookieService.set("auth_token", token) // Save JWT token
    }

    logout() {
        this.cookieService.deleteAll() // Clear all cookies
    }

    isLoggedIn() {
        return !!this.cookieService.get("auth_token")
    }

    getUserInfo() {
        const user: AuthUserInfo = {
            id: parseInt(this.cookieService.get('auth_id')),
            username: this.cookieService.get('auth_username'),
            email: this.cookieService.get('auth_email'),
            isAdmin: this.cookieService.get('auth_isAdmin').toLowerCase() == 'true'
        }

        return user;
    }

    getAuthToken() {
        return this.cookieService.get("auth_token")
    }
}
