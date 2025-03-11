import { Component, inject } from '@angular/core'
import { LogoComponent } from '../logo/logo.component'
import { Router, RouterLink } from '@angular/router'
import { AuthService } from '../../services/auth/auth.service'
import { PopupLoaderService } from '../../services/popup-loader/popup-loader.service'

@Component({
    selector: 'app-header',
    standalone: true,
    imports: [LogoComponent, RouterLink],
    templateUrl: './header.component.html',
    styleUrl: './header.component.css',
    providers: [PopupLoaderService]
})

export class HeaderComponent {
    popupLoader = inject(PopupLoaderService)
    authService = inject(AuthService)
    router = inject(Router)

    logout() {
        this.popupLoader.showPopup(
            'Logout',
            'Tem a certeza que pretende fazer logout?',
            [
                {
                    type: 'yes',
                    text: 'Sim',
                    callback: () => {
                        this.authService.logout()
                        this.router.navigate(['/login'])
                    }
                },
                {
                    type: 'cancel',
                    text: 'Cancelar'
                }
            ]
        )
    }
}
