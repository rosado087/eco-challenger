import { Component, input } from '@angular/core'
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms'

@Component({
    selector: 'app-text-input',
    imports: [ReactiveFormsModule, FormsModule],
    templateUrl: './text-input.component.html',
    styleUrl: './text-input.component.css'
})
export class TextInputComponent {
    formControl = input<FormControl>()
    placeholder = input<string>()
    type = input('text')

    // We need this since inputs don't accept undefined
    getFormControl() {
        return this.formControl() || new FormControl()
    }
}
