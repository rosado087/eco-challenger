import { Component, input } from '@angular/core';
import { PopupButton } from '../../models/popup-button.type';

@Component({
  selector: 'app-popup',
  imports: [],
  templateUrl: './popup.component.html',
  styleUrl: './popup.component.css'
})
export class PopupComponent {
  message = input.required<string>()
  buttons = input<PopupButton[]>([{
    type: 'ok',
    text: 'Okay'
  }])
}
