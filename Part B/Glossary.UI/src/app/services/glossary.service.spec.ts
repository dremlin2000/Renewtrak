import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { GlossaryService } from './glossary.service';

describe('GlossaryService', () => {
  let service: GlossaryService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
    });
    service = TestBed.inject(GlossaryService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
