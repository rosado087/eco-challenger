import { TestBed } from '@angular/core/testing';

import { PopupLoaderService } from './popup-loader.service';

describe('PopupLoaderService', () => {
  let service: PopupLoaderService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PopupLoaderService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
