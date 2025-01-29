import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddUsernameComponent } from './add-username.component';

describe('AddUsernameComponent', () => {
  let component: AddUsernameComponent;
  let fixture: ComponentFixture<AddUsernameComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddUsernameComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddUsernameComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
