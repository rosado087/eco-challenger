import { Injectable } from '@angular/core'
import { BehaviorSubject } from 'rxjs'
import { AuthUserInfo } from '../models/auth-user-info'

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private isLoggedInSubject = new BehaviorSubject<boolean>(this.hasUser());
    isLoggedIn$ = this.isLoggedInSubject.asObservable();

    private usernameSubject = new BehaviorSubject<string | null>(this.getUsername());
    username$ = this.usernameSubject.asObservable();

    private emailSubject = new BehaviorSubject<string | null>(this.getUserEmail());
    email$ = this.emailSubject.asObservable();

    private hasUser(): boolean {
        return sessionStorage.getItem('loggedInUser') !== null;
    }

    private getUsername(): string | null {
        const user = sessionStorage.getItem('loggedInUser');
        if (!user) return null;

        const parsedUser = JSON.parse(user);
        return parsedUser.username || parsedUser.Username || null;
    }

    private getUserEmail(): string | null {
        const user = sessionStorage.getItem('loggedInUser');
        if (!user) return null;

        const parsedUser = JSON.parse(user);
        return parsedUser.email || parsedUser.Email || null;
    }

    get isLoggedIn(): boolean {
        return this.isLoggedInSubject.getValue();
    }

    login(userInfo: AuthUserInfo, token: string) {
        sessionStorage.setItem('loggedInUser', JSON.stringify(userInfo));
        sessionStorage.setItem('authToken', token);

        this.isLoggedInSubject.next(true);
        this.usernameSubject.next(userInfo.username || userInfo.username);
        this.emailSubject.next(userInfo.email || userInfo.email);
    }

    logout() {
        sessionStorage.removeItem('loggedInUser');
        sessionStorage.removeItem('authToken');

        this.isLoggedInSubject.next(false);
        this.usernameSubject.next(null);
        this.emailSubject.next(null);
    }
}
