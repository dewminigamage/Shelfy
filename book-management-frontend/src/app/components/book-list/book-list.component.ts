import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BookService, BookFilter } from '../../services/book.service';
import { Book } from '../../models/book.model';
import { BookFormComponent } from '../book-form/book-form.component';
import { BookItemComponent } from '../book-item/book-item.component';
import { ThemeService } from '../../services/theme.service';

@Component({
  selector: 'app-book-list',
  standalone: true,
  imports: [CommonModule, FormsModule, BookFormComponent, BookItemComponent],
  templateUrl: './book-list.component.html',
  styleUrl: './book-list.component.css',
})
export class BookListComponent implements OnInit {
  books: Book[] = [];
  loading = true;
  error = '';
  showForm = false;
  editingBookId: number | null = null;

  toast: { message: string; type: 'success' | 'error'; visible: boolean } = {
    message: '',
    type: 'success',
    visible: false,
  };
  private toastTimer: ReturnType<typeof setTimeout> | null = null;

  showToast(message: string, type: 'success' | 'error' = 'success'): void {
    if (this.toastTimer) clearTimeout(this.toastTimer);
    this.toast = { message, type, visible: true };
    this.cdr.markForCheck();
    this.toastTimer = setTimeout(() => {
      this.toast.visible = false;
      this.cdr.markForCheck();
    }, 3500);
  }

  dismissToast(): void {
    if (this.toastTimer) clearTimeout(this.toastTimer);
    this.toast.visible = false;
  }

  // Filter and search properties
  searchQuery = '';
  // yearFilter = 'all'; // Removed as requested
  minDate: string | null = null;
  maxDate: string | null = null;
  sortBy: 'title' | 'author' | 'date' = 'title';
  sortOrder: 'asc' | 'desc' = 'asc';
  showAdvancedFilters = false;

  // View Mode
  viewMode: 'grid' | 'list' = 'grid';

  // Pagination
  currentPage = 1;
  itemsPerPage = 8; // Adjust as needed
  totalPages = 1;
  paginatedBooks: Book[] = [];

  constructor(
    private bookService: BookService,
    private cdr: ChangeDetectorRef,
    private router: Router,
    public theme: ThemeService,
  ) {}

  ngOnInit(): void {
    console.log('BookListComponent initialized');
    this.loadBooks();
  }

  loadBooks(): void {
    this.loading = true;
    this.error = '';
    console.log('Loading books...');

    const filters: BookFilter = {
      searchQuery: this.searchQuery,
      // yearFilter: this.yearFilter,
      minDate: this.minDate || undefined,
      maxDate: this.maxDate || undefined,
      sortBy: this.sortBy,
      sortOrder: this.sortOrder,
    };

    this.bookService.getAllBooks(filters).subscribe({
      next: (data) => {
        console.log('Books loaded:', data);
        this.books = data;
        this.updatePagination();
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Error loading books:', err);
        this.error = 'Failed to load books. Make sure the backend server is running.';
        this.loading = false;
        this.cdr.markForCheck();
      },
    });
  }

  updatePagination(): void {
    this.totalPages = Math.ceil(this.books.length / this.itemsPerPage);
    if (this.totalPages === 0) this.totalPages = 1;
    // ensure current page is valid
    if (this.currentPage > this.totalPages) this.currentPage = this.totalPages;

    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    const endIndex = startIndex + this.itemsPerPage;
    this.paginatedBooks = this.books.slice(startIndex, endIndex);
  }

  onPageChange(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.updatePagination();
      // Optional: Scroll to top of list
      // window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }

  setViewMode(mode: 'grid' | 'list'): void {
    this.viewMode = mode;
  }

  onFilterChange(): void {
    this.currentPage = 1; // Reset to first page on filter change
    this.loadBooks();
  }

  onClearFilters(): void {
    this.searchQuery = '';
    // this.yearFilter = 'all';
    this.minDate = null;
    this.maxDate = null;
    this.sortBy = 'title';
    this.sortOrder = 'asc';
    this.currentPage = 1;
    this.loadBooks();
  }

  onAddClick(): void {
    this.showForm = true;
    this.editingBookId = null;
  }

  onBookSelect(book: Book): void {
    if (book.id) {
      this.router.navigate(['/book', book.id]);
    }
  }

  onEdit(bookId: number): void {
    this.editingBookId = bookId;
    this.showForm = true;
  }

  onDelete(bookId: number): void {
    if (confirm('Are you sure you want to delete this book?')) {
      this.bookService.deleteBook(bookId).subscribe({
        next: () => {
          this.books = this.books.filter((b) => b.id !== bookId);
          this.updatePagination();
          this.cdr.markForCheck();
          this.showToast('Book deleted successfully!');
        },
        error: (err) => {
          this.error = 'Failed to delete book';
          console.error('Error deleting book:', err);
          this.cdr.markForCheck();
        },
      });
    }
  }

  onFormClose(): void {
    this.showForm = false;
    this.editingBookId = null;
  }

  onFormSubmit(book: Book): void {
    if (this.editingBookId !== null) {
      this.bookService.updateBook(this.editingBookId, book).subscribe({
        next: (updatedBook) => {
          const index = this.books.findIndex((b) => b.id === updatedBook.id);
          if (index > -1) {
            this.books[index] = updatedBook;
          }
          this.updatePagination(); // Added this
          this.showForm = false;
          this.editingBookId = null;
          this.cdr.markForCheck();
        },
        error: (err) => {
          this.error = 'Failed to update book';
          console.error('Error updating book:', err);
          this.cdr.markForCheck();
        },
      });
    } else {
      this.bookService.addBook(book).subscribe({
        next: (newBook) => {
          console.log('Book added:', newBook);
          this.books = [...this.books, newBook];
          this.updatePagination(); // Added this
          this.showForm = false;
          this.cdr.markForCheck();
          this.showToast('Book added successfully!');
        },
        error: (err) => {
          this.error = 'Failed to add book';
          console.error('Error adding book:', err);
          this.cdr.markForCheck();
        },
      });
    }
  }

  getBookForEdit(): Book | undefined {
    if (this.editingBookId === null) {
      return undefined;
    }
    return this.books.find((b) => b.id === this.editingBookId);
  }
}
