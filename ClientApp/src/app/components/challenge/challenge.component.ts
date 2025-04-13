import { Component, input, output } from '@angular/core'
import { CommonModule } from '@angular/common'
import { ChallengeType } from '../../models/challenge-model'
import { EcoPointsIconComponent } from '../eco-points-icon/eco-points-icon.component'

@Component({
    selector: 'app-challenge',
    imports: [CommonModule, EcoPointsIconComponent],
    templateUrl: './challenge.component.html',
    styleUrl: './challenge.component.css',
    standalone: true
})
export class ChallengeComponent {
    title = input.required<string>()
    description = input.required<string>()
    type = input<ChallengeType | null>('Daily')
    icon = input<string | null>()
    points = input.required<number>()
    iconRawData = input<File | null>()
    badgeClick = output()

    getRawImageURL(): string | null {
        if (!this.iconRawData()) return null
        return URL.createObjectURL(this.iconRawData() as File)
    }
}
