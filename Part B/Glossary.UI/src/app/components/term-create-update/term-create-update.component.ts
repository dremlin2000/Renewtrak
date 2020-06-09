import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { map } from 'rxjs/operators';
import * as uuid from 'uuid';
import { GlossaryService } from '../../services/glossary.service';
import { NotificationService } from '../../services/notification.service';
import { GlossaryTerm } from '../../models/GlossaryTerm';

@Component({
  selector: 'app-term-create',
  templateUrl: './term-create-update.component.html',
  styleUrls: ['./term-create-update.component.css'],
})
export class TermCreateUpdateComponent implements OnInit {
  formGroup: FormGroup;
  titleAlert: string = 'This field is required';
  termToEdit: GlossaryTerm;
  isNewTerm: boolean = true;

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    public activatedRoute: ActivatedRoute,
    private glossaryService: GlossaryService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.activatedRoute.paramMap
      .pipe(map(() => window.history.state))
      .subscribe((state) => {
        if (state?.termToEdit) {
          this.isNewTerm = false;
          this.termToEdit = state.termToEdit;
        }
        this.formGroup = this.formBuilder.group({
          id: [{ value: this.termToEdit?.id ?? uuid.v4(), disabled: true }],
          term: [this.termToEdit?.term, Validators.required],
          definition: [this.termToEdit?.definition, Validators.required],
        });
      });
  }

  onSubmit() {
    (this.isNewTerm
      ? this.glossaryService.create(this.formGroup.getRawValue())
      : this.glossaryService.update(this.formGroup.getRawValue())
    ).subscribe((data) => {
      this.notificationService.showSuccess(
        `Term "${data.term}" has been saved successfully to database!`
      );
      this.router.navigate(['']);
    });
  }
}
