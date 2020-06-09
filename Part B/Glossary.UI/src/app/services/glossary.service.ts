import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/index';
import { GlossaryTerm } from '../models/GlossaryTerm';

@Injectable({
  providedIn: 'root',
})
export class GlossaryService {
  constructor(private http: HttpClient) {}
  baseUrl: string = 'https://localhost:5001/api/glossary/';

  getAll(): Observable<Array<GlossaryTerm>> {
    return this.http.get<Array<GlossaryTerm>>(this.baseUrl);
  }

  getById(id: string): Observable<GlossaryTerm> {
    return this.http.get<GlossaryTerm>(`${this.baseUrl}${id}`);
  }

  create(term: GlossaryTerm): Observable<GlossaryTerm> {
    return this.http.post<GlossaryTerm>(this.baseUrl, term);
  }

  update(term: GlossaryTerm): Observable<GlossaryTerm> {
    return this.http.put<GlossaryTerm>(this.baseUrl, term);
  }

  delete(id: string): Observable<any> {
    return this.http.delete<any>(`${this.baseUrl}${id}`);
  }
}
