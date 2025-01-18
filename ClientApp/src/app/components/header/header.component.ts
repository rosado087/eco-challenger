import { Component } from '@angular/core'
import { LogoComponent } from '../logo/logo.component'
import { RouterLink } from '@angular/router'

@Component({
    selector: 'app-header',
    imports: [LogoComponent, RouterLink],
    templateUrl: './header.component.html',
    styleUrl: './header.component.css'
})
export class HeaderComponent {}
