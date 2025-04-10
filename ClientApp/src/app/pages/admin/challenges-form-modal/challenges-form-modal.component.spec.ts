import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChallengesFormModalComponent } from './challenges-form-modal.component';

describe('ChallengesFormModalComponent', () => {
  let component: ChallengesFormModalComponent;
  let fixture: ComponentFixture<ChallengesFormModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChallengesFormModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChallengesFormModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
