import { Component, input, output } from '@angular/core'
import { TagStyle } from '../../models/tag'

@Component({
    selector: 'app-tag',
    imports: [],
    templateUrl: './tag.component.html',
    styleUrl: './tag.component.css'
})
export class TagComponent {
    backgroundColor = input.required<string>()
    textColor = input.required<string>()
    type = input<TagStyle | null>('normal')
    text = input<string | null>()
    icon = input<string | null>()
    iconRawData = input<File | null>()
    badgeClick = output()

    getComponentClasses(): string[] {
        const classes = ['badge', 'scale-120', 'static']

        switch (this.type()) {
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

    getRawImageURL(): string | null {
        if (!this.iconRawData()) return null
        return URL.createObjectURL(this.iconRawData() as File)
    }
}
