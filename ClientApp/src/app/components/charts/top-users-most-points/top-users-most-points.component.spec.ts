import { ComponentFixture, TestBed } from '@angular/core/testing'

import { TopUserMostPointsComponent } from './top-users-most-points.component'

describe('TopUserMostPointsComponent', () => {
    let component: TopUserMostPointsComponent
    let fixture: ComponentFixture<TopUserMostPointsComponent>

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [TopUserMostPointsComponent]
        }).compileComponents()

        fixture = TestBed.createComponent(TopUserMostPointsComponent)
        component = fixture.componentInstance
        fixture.detectChanges()
    })

    it('should create', () => {
        expect(component).toBeTruthy()
    })
})
