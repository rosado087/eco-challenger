import { Component, input, output } from '@angular/core';
import { TagStyle } from '../../models/tag-style';

@Component({
  selector: 'app-tag',
  imports: [],
  templateUrl: './tag.component.html',
  styleUrl: './tag.component.css'
})

export class TagComponent {
    color = input.required<string>()
    type = input<TagStyle>('normal')
    text = input<string>()
    icon = input<string>()
    badgeClick = output()

    getComponentClasses(): string[] {
        let classes = ['badge', 'scale-120']

        switch(this.type()) {
            case 'normal':
                break
            case 'soft':
                classes.push('badge-soft')
                break
            case 'dash':
                classes.push('badge-dash')
                break
            case 'outline':
                classes.push('badge-outline')

        }

        return classes
    }

    getTextColor() {
        return this.darkenColor(this.color(), 50)
    }

    darkenColor(hex: string, percent: number): string {
        const num = parseInt(hex.replace("#", ""), 16);
        const amt = Math.round(2.55 * percent);
        const R = (num >> 16) - amt;
        const G = ((num >> 8) & 0x00FF) - amt;
        const B = (num & 0x0000FF) - amt;
      
        return "#" + (
          0x1000000 +
          (Math.max(0, R) << 16) +
          (Math.max(0, G) << 8) +
          Math.max(0, B)
        ).toString(16).slice(1);
      }
      
}
