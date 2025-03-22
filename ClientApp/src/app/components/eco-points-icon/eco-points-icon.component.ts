import { Component, input } from '@angular/core';

@Component({
  selector: 'app-eco-points-icon',
  imports: [],
  templateUrl: './eco-points-icon.component.html',
  styleUrl: './eco-points-icon.component.css'
})
export class EcoPointsIconComponent {
  width = input<string>('18px')
}
