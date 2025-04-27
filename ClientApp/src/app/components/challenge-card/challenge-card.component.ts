import { Component, input, output } from '@angular/core'
import { CommonModule } from '@angular/common'
import { ChallengeType } from '../../models/challenge-model'

@Component({
    selector: 'app-challenge-card',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './challenge-card.component.html',
    styleUrl: './challenge-card.component.css'
})
export class ChallengeCardComponent {
    title = input.required<string>()
    description = input<string | null>();
    type = input.required<ChallengeType>()
    points = input<number | null>()
    completed = input<boolean>(false)
    progress = input<number | null>()
    maxProgress = input<number | null>()
    showProgress = input<boolean>(false)
    complete = output()
    progressClick = output()
    openModal = output()
}
