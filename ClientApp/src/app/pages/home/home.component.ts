import { Component } from '@angular/core'
import { PopupLoaderService } from '../../services/popup-loader.service'

@Component({
    selector: 'app-home',
    //imports: [ButtonComponent],
    providers: [PopupLoaderService],
    templateUrl: './home.component.html',
    styleUrl: './home.component.css'
})
export class HomeComponent {}
