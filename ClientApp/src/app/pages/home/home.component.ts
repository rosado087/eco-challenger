import { Component, inject, OnInit } from '@angular/core';
import { ButtonComponent } from '../../components/button/button.component';
import { NetApiService } from '../../services/net-api.service';
import { Test } from '../../models/test.type';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { PopupLoaderService } from '../../services/popup-loader.service';
declare var google: any;

export interface Result {
  success: boolean
}

@Component({
  selector: 'app-home',
  imports: [ButtonComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})


export class HomeComponent implements OnInit {
    
  netApi = inject(NetApiService)
  popupLoader = inject(PopupLoaderService)
  title = 'ClientApp'

  constructor(private router: Router) {
    
  }

  ngOnInit(): void {

    let value;

    this.netApi.get<String>('user', 'id').subscribe({

      next: (data) => {

        value = data;

      },
      error: () => {
        this.popupLoader.showPopup(
          'Whops',
          'Isto é um problema.'
          
        )
      }
    });

    google.accounts.id.initialize({
      client_id: value,
      callback: (resp: any) => this.handleLoginGoogle(resp)
    });

    google.accounts.id.renderButton(document.getElementById("google-btn"), {
      theme: 'filled_green',
      size: 'large',
      shape:'rectangle'
    })
  }

  handleLoginGoogle(response: any) {
    if (response) {
      const info = JSON.parse(atob(response.credential.split(".")[1]));
      sessionStorage.setItem("loggedInUser", JSON.stringify(info));
      
      this.netApi.post<Result>('user', 'authenticate', [info.sub,info.email]).subscribe({
        next: (data) => {
          console.log(data.success);
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
