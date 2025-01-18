import { Component, inject } from '@angular/core'
import { ButtonComponent } from '../../components/button/button.component';
import { NetApiService } from '../../services/net-api.service';
import { Test } from '../../models/test.type';

@Component({
    selector: 'app-home',
    imports: [ButtonComponent],
    templateUrl: './home.component.html',
    styleUrl: './home.component.css'
})
export class HomeComponent {
    netApi = inject(NetApiService)
    title = 'ClientApp'

    btnClick() {
        this.netApi.get<Test>('test', 'testme').subscribe({
          next: (data) => {
            if (data.success) alert('Sent and success!')
            else alert('there was no success')
          },
          error: () => alert('An error ocurred')
        })
    }
}
