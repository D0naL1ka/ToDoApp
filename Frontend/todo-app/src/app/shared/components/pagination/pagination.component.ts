import { Component, EventEmitter, Input, OnChanges, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagination.component.html'
})
export class PaginationComponent implements OnChanges {
  @Input() currentPage = 1;
  @Input() totalPages = 1;
  @Output() pageChange = new EventEmitter<number>();

  pages: number[] = [];

  ngOnChanges(): void {
    this.pages = Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  goTo(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.pageChange.emit(page);
  }
}