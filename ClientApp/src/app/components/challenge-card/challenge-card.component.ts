import { Component, input, output } from '@angular/core'
import { CommonModule } from '@angular/common'

@Component({
  selector: 'app-challenge-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './challenge-card.component.html',
  styleUrl: './challenge-card.component.css'
})
export class ChallengeCardComponent {
  title = input.required<string>()
  type = input.required<'Daily' | 'Weekly'>()
  points = input<number | null>()
  completed = input<boolean>(false)
  progress = input<number | null>()
  maxProgress = input<number | null>()
  showProgress = input<boolean>(false)
  onComplete = output()
  onProgress = output()
  onOpenModal = output()
}
