import { Injectable } from '@angular/core'
import { BehaviorSubject } from 'rxjs'
import { AuthUserInfo } from '../models/auth-user-info'

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private isLoggedInSubject = new BehaviorSubject<boolean>(this.hasUser())
    isLoggedIn$ = this.isLoggedInSubject.asObservable()
    private usernameSubject = new BehaviorSubject<string | null>(
        this.getUsername()
    )
    username$ = this.usernameSubject.asObservable()

    private hasUser(): boolean {
        return sessionStorage.getItem('loggedInUser') !== null
    }

    private getUsername(): string | null {
        const user = sessionStorage.getItem('loggedInUser')
        return user ? JSON.parse(user).Username : null
    }

    get getUserEmail(): string {
        const user = sessionStorage.getItem('loggedInUser')
        return user ? JSON.parse(user).Email : ''
    }

    get isLoggedIn(): boolean {
        return this.isLoggedInSubject.getValue()
    }

    login(userInfo: AuthUserInfo, token: string) {
        sessionStorage.setItem('loggedInUser', JSON.stringify(userInfo))
        sessionStorage.setItem('authToken', token)
        this.isLoggedInSubject.next(true)
        this.usernameSubject.next(userInfo.username)
    }

    logout() {
        sessionStorage.removeItem('loggedInUser')
        sessionStorage.removeItem('authToken')
        this.isLoggedInSubject.next(false)
        this.usernameSubject.next(null)
    }
}
