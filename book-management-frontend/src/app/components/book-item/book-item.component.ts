import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Book } from '../../models/book.model';

@Component({
  selector: 'app-book-item',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './book-item.component.html',
  styleUrl: './book-item.component.css',
})
export class BookItemComponent {
  @Input() book!: Book;
  @Input() viewMode: 'grid' | 'list' = 'grid';
  @Output() onCardClick = new EventEmitter<void>();
}
