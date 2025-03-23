import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TagFormModalComponent } from './tag-form-modal.component';

describe('TagFormModalComponent', () => {
  let component: TagFormModalComponent;
  let fixture: ComponentFixture<TagFormModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TagFormModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TagFormModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
