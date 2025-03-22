import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StoreTagComponent } from './store-tag.component';

describe('StoreTagComponent', () => {
  let component: StoreTagComponent;
  let fixture: ComponentFixture<StoreTagComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StoreTagComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StoreTagComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
