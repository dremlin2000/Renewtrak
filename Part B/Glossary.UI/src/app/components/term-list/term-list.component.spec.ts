import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of } from 'rxjs';
import { TermListComponent } from './term-list.component';
import { GlossaryService } from '../../services/glossary.service';
import { MatDialogModule } from '@angular/material/dialog';

describe('TermListComponent', () => {
  let component: TermListComponent;
  let fixture: ComponentFixture<TermListComponent>;
  let glossaryService: GlossaryService;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [TermListComponent],
      imports: [HttpClientTestingModule, MatDialogModule],
      providers: [GlossaryService],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TermListComponent);
    component = fixture.componentInstance;
    glossaryService = TestBed.get(GlossaryService);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should retreive glossary term list', () => {
    //Arrange
    const testData = [{ id: '1', term: 't1', definition: 'def1' }];
    let spy = spyOn(glossaryService, 'getAll').and.returnValue(of(testData));

    //Act
    component.ngOnInit();

    spy.calls.mostRecent().returnValue.subscribe((data) => {
      //Assert
      expect(testData).toEqual(data);
    });
  });
});
