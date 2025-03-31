/* eslint-disable @typescript-eslint/no-explicit-any */

import { inject, Injectable } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Observable } from 'rxjs'
import { environment } from '../../../environments/environment'

@Injectable({
    providedIn: 'root'
})
export class NetApiService {
    private http = inject(HttpClient)
    private baseUrl = environment.apiUrl

    get<T>(
        controller: string,
        action: string,
        queryParams?: Record<string, string>,
        ...routeParams: string[]
    ): Observable<T> {
        return this.http.get<T>(
            this.#buildUrl(controller, action, routeParams, queryParams)
        )
    }

    post<T>(
        controller: string,
        action: string,
        data: any,
        ...routeParams: string[]
    ): Observable<T> {
        return this.http.post<T>(
            this.#buildUrl(controller, action, routeParams),
            data
        )
    }

    #buildUrl(
        controller: string,
        action: string,
        routeParams: string[],
        queryParams?: Record<string, string>
    ) {
        let route = `${this.baseUrl}/${controller.toLowerCase()}/${action.toLocaleLowerCase()}`

        // Add route params
        routeParams?.forEach((r) => {
            route += `/${r}`
        })

        if (queryParams) {
            let strParams = ''
            Object.keys(queryParams).forEach((key) => {
                strParams += `&${key}=${queryParams[key]}`
            })

            // We dont want the first &
            route += `?${strParams.substring(1)}`
        }

        return route
    }
}
