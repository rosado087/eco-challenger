import { inject, Injectable } from '@angular/core'
import { HttpClient, HttpHeaders } from '@angular/common/http'
import { Observable } from 'rxjs'

@Injectable({
    providedIn: 'root'
})
export class NetApiService {
    private http = inject(HttpClient)
    private baseUrl = '/api'

    get<T>(controller: string, action: string): Observable<T> {
        return this.http.get<T>(this.#buildUrl(controller, action))
    }

    // eslint-disable-next-line  @typescript-eslint/no-explicit-any
    post<T>(controller: string, action: string, data: any): Observable<T> {
        return this.http.post<T>(this.#buildUrl(controller, action), data, {
            headers: new HttpHeaders({
                'Content-Type': 'application/json'
            })
        })
    }

    #buildUrl(controller: string, action: string) {
        return `${this.baseUrl}/${controller.toLowerCase()}/${action.toLocaleLowerCase()}`
    }
}
