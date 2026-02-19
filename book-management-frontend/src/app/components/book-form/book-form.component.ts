import { Component, Input, Output, EventEmitter, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Book } from '../../models/book.model';

@Component({
  selector: 'app-book-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './book-form.component.html',
  styleUrl: './book-form.component.css',
})
export class BookFormComponent implements OnInit {
  @Input() book?: Book;
  @Input() existingBooks: Book[] = [];
  @Output() onSubmit = new EventEmitter<Book>();
  @Output() onCancel = new EventEmitter<void>();

  formData: Book = {
    title: '',
    author: '',
    isbn: '',
    imageUrl: '',
    publicationDate: new Date(),
  };

  errors: { [key: string]: string } = {};
  isEditing = false;
  todayDate: string = '';

  // Image Upload
  imagePreview: string | null = null;
  selectedFile: File | null = null;

  constructor(private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    // Set today's date for the date input max attribute
    this.todayDate = this.getTodayDateString();

    if (this.book) {
      this.isEditing = true;
      this.formData = { ...this.book };
      // Normalize publicationDate to 'YYYY-MM-DD' string so the date input binds correctly
      if (this.formData.publicationDate) {
        const d = new Date(this.formData.publicationDate);
        const yyyy = d.getFullYear();
        const mm = String(d.getMonth() + 1).padStart(2, '0');
        const dd = String(d.getDate()).padStart(2, '0');
        (this.formData as any).publicationDate = `${yyyy}-${mm}-${dd}`;
      }
      this.imagePreview = this.book.imageUrl || null;
    }
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
      console.log('File selected:', file.name, 'Size:', file.size);

      // Create a preview
      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreview = reader.result as string;
        this.formData.imageUrl = this.imagePreview; // Store base64 in formData
        console.log('Image converted to Base64 length:', this.formData.imageUrl.length);
        this.cdr.detectChanges(); // Force update view
      };
      reader.readAsDataURL(file);
    }
  }

  getTodayDateString(): string {
    const today = new Date();
    const year = today.getFullYear();
    const month = String(today.getMonth() + 1).padStart(2, '0');
    const day = String(today.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  validateISBN(isbn: string): string | null {
    // Remove hyphens for validation
    const cleanISBN = isbn.replace(/-/g, '');

    // Check if it contains only digits
    if (!/^\d+$/.test(cleanISBN)) {
      return 'ISBN must contain only numbers and optional hyphens';
    }

    // Check length (10 or 13 digits)
    if (cleanISBN.length !== 10 && cleanISBN.length !== 13) {
      return 'ISBN must be exactly 10 or 13 digits';
    }

    return null;
  }

  isISBNUnique(isbn: string): boolean {
    // When editing, allow the same ISBN
    if (this.isEditing && this.book?.isbn === isbn) {
      return true;
    }

    // Check if ISBN already exists in the list
    return !this.existingBooks.some(
      (book) => book.isbn.replace(/-/g, '') === isbn.replace(/-/g, ''),
    );
  }

  validateTitle(title: string): string | null {
    const trimmedTitle = title.trim();

    if (!trimmedTitle) {
      return 'Title is required';
    }

    if (trimmedTitle.length < 2) {
      return 'Title must be at least 2 characters';
    }

    if (trimmedTitle.length > 100) {
      return 'Title cannot exceed 100 characters';
    }

    return null;
  }

  validateAuthor(author: string): string | null {
    const trimmedAuthor = author.trim();

    if (!trimmedAuthor) {
      return 'Author is required';
    }

    if (trimmedAuthor.length < 2) {
      return 'Author must be at least 2 characters';
    }

    if (trimmedAuthor.length > 50) {
      return 'Author cannot exceed 50 characters';
    }

    return null;
  }

  validatePublicationDate(date: any): string | null {
    if (!date) {
      return 'Publication date is required';
    }

    const pubDate = new Date(date);
    const today = new Date();

    // Check if date is in the future
    if (pubDate > today) {
      return 'Publication date cannot be in the future';
    }

    // Check if date is after 1450 (first printed books)
    const minYear = new Date('1450-01-01');
    if (pubDate < minYear) {
      return 'Publication date must be after 1450';
    }

    return null;
  }

  validateForm(): boolean {
    this.errors = {};

    // Validate Title
    const titleError = this.validateTitle(this.formData.title);
    if (titleError) {
      this.errors['title'] = titleError;
    }

    // Validate Author
    const authorError = this.validateAuthor(this.formData.author);
    if (authorError) {
      this.errors['author'] = authorError;
    }

    // Validate ISBN
    if (!this.formData.isbn || this.formData.isbn.trim() === '') {
      this.errors['isbn'] = 'ISBN is required';
    } else {
      const isbnError = this.validateISBN(this.formData.isbn);
      if (isbnError) {
        this.errors['isbn'] = isbnError;
      } else if (!this.isISBNUnique(this.formData.isbn)) {
        this.errors['isbn'] = 'This ISBN already exists';
      }
    }

    // Validate Publication Date
    const dateError = this.validatePublicationDate(this.formData.publicationDate);
    if (dateError) {
      this.errors['date'] = dateError;
    }

    return Object.keys(this.errors).length === 0;
  }

  isValidForm(): boolean {
    return (
      this.formData.title.length >= 2 && this.formData.author.length >= 2 && !!this.formData.isbn
    );
  }

  submit(): void {
    if (this.validateForm()) {
      this.onSubmit.emit(this.formData);
    }
  }

  cancel(): void {
    this.onCancel.emit();
  }
}
