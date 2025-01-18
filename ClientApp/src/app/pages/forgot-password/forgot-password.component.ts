import { Component, inject } from '@angular/core'
import { NgIcon, provideIcons } from '@ng-icons/core'
import {
    heroAtSymbol,
    heroUserCircle,
    heroKey
} from '@ng-icons/heroicons/outline'
import { TextInputComponent } from '../../components/text-input/text-input.component'
import { ButtonComponent } from '../../components/button/button.component'
import { PopupLoaderService } from '../../services/popup-loader.service'

@Component({
    selector: 'app-forgot-password',
    imports: [TextInputComponent, NgIcon, ButtonComponent],
    providers: [
        provideIcons({ heroAtSymbol, heroUserCircle, heroKey }),
        PopupLoaderService
    ],
    templateUrl: './forgot-password.component.html',
    styleUrl: './forgot-password.component.css'
})
export class ForgotPasswordComponent {
    popupLoader = inject(PopupLoaderService)

    sendEmail() {
        this.popupLoader.showPopup(
            'Recuperação de Palavra-Passe',
            'Foi feito o envio de um email com instruções de recuperação de password!'
        )
    }
}
