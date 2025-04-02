import { Component, input, output } from '@angular/core'
import { EcoPointsIconComponent } from '../eco-points-icon/eco-points-icon.component'

@Component({
    selector: 'app-store-tag',
    templateUrl: './store-tag.component.html',
    styleUrl: './store-tag.component.css',
    imports: [EcoPointsIconComponent]
})
export class StoreTagComponent {
    bought = input<boolean>(false)
    buyClick = output()
    price = input.required<number>()
    id = input.required<number>()
}
