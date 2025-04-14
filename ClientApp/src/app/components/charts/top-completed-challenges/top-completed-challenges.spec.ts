import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TopPurchasedTagsComponent } from './top-completed-challenges.component';

describe('TopPurchasedTagsComponent', () => {
  let component: TopPurchasedTagsComponent;
  let fixture: ComponentFixture<TopPurchasedTagsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TopPurchasedTagsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TopPurchasedTagsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
