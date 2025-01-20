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
import { NetApiService } from '../../services/net-api.service'
import { SuccessModel } from '../../models/success-model'

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
    netApi = inject(NetApiService)
    popupLoader = inject(PopupLoaderService)

    sendEmail() {
        this.netApi.get<SuccessModel>('RecoverPassword', 'SendRecoveryEmail').subscribe({
            next: () => {
                // Always send this, even if the account doesn't exist
                this.popupLoader.showPopup(
                    'Recuperação de Palavra-Passe',
                    'Foi feito o envio de um email com instruções de recuperação de password!'
                )
            },
            error: () => this.popupLoader.showPopup(
                'Whops :(',
                'Ocorreu um erro desconhecido ao enviar o email.'
            )
        })


        
    }
}
