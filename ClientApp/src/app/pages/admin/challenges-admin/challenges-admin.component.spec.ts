import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChallengesAdminComponent } from './challenges-admin.component';

describe('ChallengesAdminComponent', () => {
  let component: ChallengesAdminComponent;
  let fixture: ComponentFixture<ChallengesAdminComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChallengesAdminComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChallengesAdminComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
