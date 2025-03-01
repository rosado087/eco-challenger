import { Component, OnInit, inject } from '@angular/core'
import { LogoComponent } from '../logo/logo.component'
import { Router, RouterLink } from '@angular/router'
import { AuthService } from '../../services/auth.service'
import { PopupLoaderService } from '../../services/popup-loader.service'

@Component({
    selector: 'app-header',
    standalone: true,
    imports: [LogoComponent, RouterLink],
    templateUrl: './header.component.html',
    styleUrl: './header.component.css',
    providers: [PopupLoaderService] // ✅ Ensure service is provided
})
export class HeaderComponent implements OnInit {
    isLoggedIn = false
    username: string | null = null
    popupLoader = inject(PopupLoaderService) // ✅ Inject PopupLoaderService properly

    constructor(
        private authService: AuthService,
        private router: Router
    ) {}

    ngOnInit() {
        this.authService.isLoggedIn$.subscribe(
            (status) => (this.isLoggedIn = status)
        )
        this.authService.username$.subscribe((name) => (this.username = name))
    }

    logout() {
        this.popupLoader.showPopup(
            'Logout',
            'Você saiu da sua conta com sucesso!',
            [
                {
                    type: 'ok',
                    text: 'Okay',
                    callback: () => {
                        this.authService.logout()
                        this.router.navigate(['/login'])
                    }
                }
            ]
        )
    }
}
