import { Component, OnInit, inject } from '@angular/core'
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
