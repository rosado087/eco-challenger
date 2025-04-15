import { Component, inject} from '@angular/core'
import { PopupLoaderService } from '../../services/popup-loader/popup-loader.service'
import { ActivatedRoute, Router, RouterLink } from '@angular/router'
import { AuthService } from '../../services/auth/auth.service'

@Component({
    selector: 'app-home',
    imports: [RouterLink],
    providers: [PopupLoaderService],
    templateUrl: './home.component.html',
    styleUrl: './home.component.css'
})
export class HomeComponent {
    router = inject(Router)
    route = inject(ActivatedRoute)
    authService = inject(AuthService)

    ngOnInit(): void {
        this.router.navigate(['/challenges/'+ this.authService.getUserInfo().id])
    }
}
