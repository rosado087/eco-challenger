import { Component, inject, OnInit } from '@angular/core';
import { ButtonComponent } from '../../components/button/button.component';
import { NetApiService } from '../../services/net-api.service';
import { Router } from '@angular/router';
import { PopupLoaderService } from '../../services/popup-loader.service';
declare var google: any;

export interface Result {
  success: any
}

@Component({
  selector: 'app-home',
  imports: [ButtonComponent],
  providers: [PopupLoaderService],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})


export class HomeComponent implements OnInit {
    
  netApi = inject(NetApiService)
  popupLoader = inject(PopupLoaderService)
  router = inject(Router)
  title = 'ClientApp'


  ngOnInit(): void {

    let value;

    this.netApi.get<Result>('User', 'GetGoogleId').subscribe({

      next: (data) => {
        
        google.accounts.id.initialize({
          client_id: data.success,
          callback: (resp: any) => this.handleLoginGoogle(resp)
        });

        google.accounts.id.renderButton(document.getElementById("google-btn"), {
          theme: 'filled_green',
          size: 'large',
          shape: 'rectangle'
        })
      },
      error: () => {
        this.popupLoader.showPopup(
          'Whops',
          'Isto é um problema.'
          
        )
      }
    });

    
  }

  handleLoginGoogle(response: any) {
    if (response) {
      const info = JSON.parse(atob(response.credential.split(".")[1]));
      sessionStorage.setItem("loggedInUser", JSON.stringify(info));

      this.netApi.post<Result>('User', 'AuthenticateGoogle', [info.sub,info.email]).subscribe({
        next: (data) => {
          if (data.success) this.router.navigate(['main-page']);
          else {
            this.router.navigate(['add-username']);
          }
        },
        error: () => {
          this.popupLoader.showPopup(
            'Whops',
            'Houve um problema ao autenticar com conta Google.'

          )
        }
      })
      
    }
  }
}
