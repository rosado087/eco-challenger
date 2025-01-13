import { Component, inject } from '@angular/core'
import { NetApiService } from '../../services/net-api.service'
import { Test } from '../../models/test.type'

@Component({
    selector: 'app-button',
    imports: [],
    templateUrl: './button.component.html',
    styleUrl: './button.component.css'
})
export class ButtonComponent {
    netApi = inject(NetApiService)

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
