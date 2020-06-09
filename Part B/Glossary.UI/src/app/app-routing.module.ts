import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TermListComponent } from './components/term-list/term-list.component';
import { TermCreateUpdateComponent } from './components/term-create-update/term-create-update.component';

const routes: Routes = [
  { path: '', component: TermListComponent },
  { path: 'term-create-update', component: TermCreateUpdateComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
