import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { BookService } from '../../services/book.service';
import { Book } from '../../models/book.model';
import { BookFormComponent } from '../book-form/book-form.component';

@Component({
  selector: 'app-book-detail',
  standalone: true,
  imports: [CommonModule, BookFormComponent],
  templateUrl: './book-detail.component.html',
  styleUrl: './book-detail.component.css',
})
export class BookDetailComponent implements OnInit {
  book: Book | undefined;
  loading = true;
  error = '';
  showEditForm = false;
  imageError = false;

  toast: { message: string; type: 'success' | 'error'; visible: boolean } = {
    message: '',
    type: 'success',
    visible: false,
  };
  private toastTimer: ReturnType<typeof setTimeout> | null = null;

  showToast(message: string, type: 'success' | 'error' = 'success'): void {
    if (this.toastTimer) clearTimeout(this.toastTimer);
    this.toast = { message, type, visible: true };
    this.cdr.detectChanges();
    this.toastTimer = setTimeout(() => {
      this.toast.visible = false;
      this.cdr.detectChanges();
    }, 3500);
  }

  dismissToast(): void {
    if (this.toastTimer) clearTimeout(this.toastTimer);
    this.toast.visible = false;
  }

  onImageError(): void {
    this.imageError = true;
  }

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bookService: BookService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id')); // Debug ID
    console.log('BookDetailComponent initialized with ID:', id);

    if (id) {
      this.loadBook(id);
    } else {
      console.error('Invalid ID or route failed to resolve');
      this.error = 'Invalid book ID';
      this.loading = false;
    }
  }

  loadBook(id: number): void {
    console.log('Initiating fetch for book:', id);
    this.bookService.getBookById(id).subscribe({
      next: (book) => {
        console.log('Book successfully loaded:', book);
        this.book = book;
        this.loading = false;
        this.cdr.detectChanges(); // Ensure UI updates
      },
      error: (err) => {
        console.error('Error fetching book details:', err);
        this.error = 'Failed to load book details. Please try again.';
        this.loading = false;
        this.cdr.detectChanges(); // Ensure UI updates
      },
    });
  }

  getFormattedDate(): string {
    if (!this.book) return '';
    const date = new Date(this.book.publicationDate);
    return date.toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' });
  }

  onBack(): void {
    this.router.navigate(['/']);
  }

  onEditClick(): void {
    this.showEditForm = true;
  }

  onDeleteClick(): void {
    if (this.book && this.book.id && confirm('Are you sure you want to delete this book?')) {
      this.bookService.deleteBook(this.book.id).subscribe({
        next: () => {
          this.showToast('Book deleted successfully!');
          setTimeout(() => this.router.navigate(['/']), 1200);
        },
        error: (err) => {
          console.error('Error deleting book:', err);
          this.showToast('Failed to delete book', 'error');
        },
      });
    }
  }

  onFormSubmit(updatedBook: Book): void {
    if (this.book?.id) {
      this.bookService.updateBook(this.book.id, updatedBook).subscribe({
        next: (book) => {
          this.book = book;
          this.showEditForm = false;
          this.cdr.detectChanges();
          this.showToast('Book updated successfully!');
        },
        error: (err) => {
          console.error('Error updating book:', err);
          this.showToast('Failed to update book. Please try again.', 'error');
        },
      });
    }
  }

  onFormCancel(): void {
    this.showEditForm = false;
  }
}
