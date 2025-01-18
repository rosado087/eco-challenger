import { Component, inject, OnInit } from '@angular/core';
import { PopupLoaderService } from '../../services/popup-loader.service';
import { TextInputComponent } from '../../components/text-input/text-input.component';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { ButtonComponent } from '../../components/button/button.component';
import { heroKey } from '@ng-icons/heroicons/outline';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-reset-password',
  imports: [TextInputComponent, NgIcon, ButtonComponent],
  providers: [provideIcons({ heroKey }), PopupLoaderService],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css'
})
export class ResetPasswordComponent implements OnInit {
  popupLoader = inject(PopupLoaderService)
  resetToken: string | null = null

  constructor(private route: ActivatedRoute,
    private router: Router
  ) {}

  resetPassword() {
    this.popupLoader.showPopup('Alteração de Palavra-Passe', 'A palavra passe foi alterada com sucesso!')
  }

  ngOnInit(): void {
    this.resetToken = this.route.snapshot.paramMap.get('token')

    // If token doesn't exist, redirect to homepage
    if(!this.resetToken) this.router.navigate(['/'])
  }
}
