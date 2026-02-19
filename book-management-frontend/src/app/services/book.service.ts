import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Book } from '../models/book.model';

export interface BookFilter {
  searchQuery?: string;
  yearFilter?: string;
  minDate?: string;
  maxDate?: string;
  sortBy?: 'title' | 'author' | 'date';
  sortOrder?: 'asc' | 'desc';
}

@Injectable({
  providedIn: 'root',
})
export class BookService {
  private apiUrl = 'https://humble-happiness-production.up.railway.app/api/books';

  constructor(private http: HttpClient) {}

  getAllBooks(filters?: BookFilter): Observable<Book[]> {
    let params = new HttpParams();

    if (filters) {
      if (filters.searchQuery) params = params.set('searchQuery', filters.searchQuery);
      if (filters.yearFilter) params = params.set('yearFilter', filters.yearFilter);
      if (filters.minDate) params = params.set('minDate', filters.minDate);
      if (filters.maxDate) params = params.set('maxDate', filters.maxDate);
      if (filters.sortBy) params = params.set('sortBy', filters.sortBy);
      if (filters.sortOrder) params = params.set('sortOrder', filters.sortOrder);
    }

    return this.http.get<Book[]>(this.apiUrl, { params });
  }

  getBookById(id: number): Observable<Book> {
    return this.http.get<Book>(`${this.apiUrl}/${id}`);
  }

  addBook(book: Book): Observable<Book> {
    return this.http.post<Book>(this.apiUrl, book);
  }

  updateBook(id: number, book: Book): Observable<Book> {
    return this.http.put<Book>(`${this.apiUrl}/${id}`, book);
  }

  deleteBook(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
