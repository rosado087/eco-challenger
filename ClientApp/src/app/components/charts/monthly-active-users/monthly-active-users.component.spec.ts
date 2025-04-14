import { ComponentFixture, TestBed } from '@angular/core/testing'

import { MonthlyActiveUsersComponent } from './monthly-active-users.component'

describe('MonthlyActiveUsersComponent', () => {
    let component: MonthlyActiveUsersComponent
    let fixture: ComponentFixture<MonthlyActiveUsersComponent>

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [MonthlyActiveUsersComponent]
        }).compileComponents()

        fixture = TestBed.createComponent(MonthlyActiveUsersComponent)
        component = fixture.componentInstance
        fixture.detectChanges()
    })

    it('should create', () => {
        expect(component).toBeTruthy()
    })
})
