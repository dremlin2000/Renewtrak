import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { of } from 'rxjs';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { TermCreateUpdateComponent } from './term-create-update.component';
import { GlossaryService } from '../../services/glossary.service';

describe('TermCreateUpdateComponent', () => {
  let component: TermCreateUpdateComponent;
  let fixture: ComponentFixture<TermCreateUpdateComponent>;
  let glossaryService: GlossaryService;
  const testData = { id: '1', term: 't1', definition: 'def1' };

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [TermCreateUpdateComponent],
      imports: [
        BrowserAnimationsModule,
        HttpClientTestingModule,
        RouterTestingModule,
        ReactiveFormsModule,
        MatSnackBarModule,
      ],
      providers: [GlossaryService],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TermCreateUpdateComponent);
    component = fixture.componentInstance;
    glossaryService = TestBed.get(GlossaryService);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should create a new term', () => {
    //Arrange
    let spy = spyOn(glossaryService, 'create').and.returnValue(of(testData));

    //Act
    component.ngOnInit();
    component.onSubmit();

    spy.calls.mostRecent().returnValue.subscribe((data) => {
      //Assert
      expect(testData).toEqual(data);
    });
  });

  it('When submit the valid form Then form is valid', async(() => {
    //Arrange
    spyOn(glossaryService, 'create').and.returnValue(of(testData));

    //Act
    component.ngOnInit();
    component.formGroup.setValue(testData);
    component.onSubmit();

    //Assert
    expect(component.formGroup.valid).toBeTruthy();
    expect(glossaryService.create).toHaveBeenCalled();
  }));
});
