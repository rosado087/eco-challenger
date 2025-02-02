/* eslint-disable @typescript-eslint/no-explicit-any */
import { inject, Injectable } from '@angular/core'
import { HttpClient, HttpHeaders } from '@angular/common/http'
import { Observable } from 'rxjs'

@Injectable({
    providedIn: 'root'
})
export class NetApiService {
    private http = inject(HttpClient)
    private baseUrl = '/api'

    get<T>(
        controller: string,
        action: string,
        ...routeParams: string[]
    ): Observable<T> {
        return this.http.get<T>(this.#buildUrl(controller, action, routeParams))
    }

    // eslint-disable-next-line  @typescript-eslint/no-explicit-any
    post<T>(
        controller: string,
        action: string,
        data: any,
        ...routeParams: string[]
    ): Observable<T> {
        return this.http.post<T>(
            this.#buildUrl(controller, action, routeParams),
            data,
            {
                headers: new HttpHeaders({
                    'Content-Type': 'application/json'
                })
            }
        )
    }

    #buildUrl(controller: string, action: string, routeParams: string[]) {
        let route = `${this.baseUrl}/${controller.toLowerCase()}/${action.toLocaleLowerCase()}`

        routeParams?.forEach((r) => {
            route += `/${r}`
        })

        return route
    }
}
