import { ComponentFixture, TestBed } from '@angular/core/testing'

import { EcoPointsIconComponent } from './eco-points-icon.component'

describe('EcoPointsIconComponent', () => {
    let component: EcoPointsIconComponent
    let fixture: ComponentFixture<EcoPointsIconComponent>

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [EcoPointsIconComponent]
        }).compileComponents()

        fixture = TestBed.createComponent(EcoPointsIconComponent)
        component = fixture.componentInstance
        fixture.detectChanges()
    })

    it('should create', () => {
        expect(component).toBeTruthy()
    })
})
