import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { DialogBoxComponent } from '../dialog-box/dialog-box.component';
import { GlossaryService } from '../../services/glossary.service';
import { GlossaryTerm } from '../../models/GlossaryTerm';

@Component({
  selector: 'term-list',
  templateUrl: './term-list.component.html',
  styleUrls: ['./term-list.component.css'],
})
export class TermListComponent implements OnInit {
  dataSource: Array<GlossaryTerm> = [];
  displayedColumns: string[] = ['id', 'term', 'definition', 'action'];

  constructor(
    private glossaryService: GlossaryService,
    public dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.glossaryService.getAll().subscribe((data) => {
      this.dataSource = data;
    });
  }

  openDialog(action: string, term: GlossaryTerm) {
    const dialogRef = this.dialog.open(DialogBoxComponent, {
      width: '250px',
      data: { action, id: term.id, name: term.term },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result.event == 'Delete') {
        this.deleteTerm(result.data.id);
      }
    });
  }

  deleteTerm(id: string) {
    this.glossaryService.delete(id).subscribe((data) => {
      this.dataSource = this.dataSource.filter((value) => {
        return value.id != id;
      });
    });
  }
}
