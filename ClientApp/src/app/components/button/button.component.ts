import { Component, input, output } from '@angular/core'

@Component({
    selector: 'app-button',
    imports: [],
    templateUrl: './button.component.html',
    styleUrl: './button.component.css'
})
export class ButtonComponent {
    customClass = input<string>()
    text = input.required<string>()
    btnClick = output()

    getBtnClasses() {
        return this.customClass() || 'btn btn-primary'
    }
}
