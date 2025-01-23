import { Component, ElementRef, inject, input, OnInit } from '@angular/core'
import { PopupButton } from '../../models/popup-button'
import { ButtonComponent } from '../button/button.component'

@Component({
    selector: 'app-popup',
    imports: [ButtonComponent],
    templateUrl: './popup.component.html',
    styleUrl: './popup.component.css'
})
export class PopupComponent implements OnInit {
    _elementRef = inject(ElementRef)
    title = input.required<string>()
    message = input.required<string>()
    buttons = input<PopupButton[]>()

    getButtons() {
        if (this.buttons && this.buttons()) return this.buttons()

        return [
            {
                type: 'ok',
                text: 'Okay'
            }
        ] as PopupButton[]
    }

    /*
     * This function is pretty nasty, fetching DOM elements like this is
     * not a good practice, however, there was no other way of doing this
     * given we need to use the included JS function from DaisyUI's package.
     *
     * viewChild cannot find the element in DOM for some reason.
     */
    ngOnInit(): void {
        const elem =
            this._elementRef.nativeElement.querySelector(`#popup-modal-ref`)

        if (!elem || !elem.showModal)
            console.log('Could not find PopupComponent in the DOM')

        elem.showModal()
    }
}
