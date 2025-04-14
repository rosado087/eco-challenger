import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TopCompletedChallengesComponent } from './top-completed-challenges.component';

describe('TopCompletedChallengesComponent', () => {
  let component: TopCompletedChallengesComponent;
  let fixture: ComponentFixture<TopCompletedChallengesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TopCompletedChallengesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TopCompletedChallengesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
