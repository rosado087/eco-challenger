import { Component } from '@angular/core';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroAtSymbol, heroUserCircle, heroKey } from '@ng-icons/heroicons/outline';
import { TextInputComponent } from '../../components/text-input/text-input.component';

@Component({
  selector: 'app-forgot-password',
  imports: [TextInputComponent, NgIcon],
  providers: [provideIcons({ heroAtSymbol, heroUserCircle, heroKey })],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.css'
})
export class ForgotPasswordComponent {

}
