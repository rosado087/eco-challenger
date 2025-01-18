import { Component, inject } from '@angular/core';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroAtSymbol, heroUserCircle, heroKey } from '@ng-icons/heroicons/outline';
import { TextInputComponent } from '../../components/text-input/text-input.component';
import { ButtonComponent } from '../../components/button/button.component';
import { PopupLoaderDirective } from '../../directives/popup-loader.directive';

@Component({
  selector: 'app-forgot-password',
  imports: [TextInputComponent, NgIcon, ButtonComponent],
  providers: [provideIcons({ heroAtSymbol, heroUserCircle, heroKey })],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.css',
})
export class ForgotPasswordComponent {
    popupLoader = inject(PopupLoaderDirective)

    sendEmail() {
        this.popupLoader.showPopup('teste')
    }
}
