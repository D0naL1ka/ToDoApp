import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-confirm-dialog',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './confirm-dialog.component.html'
})
export class ConfirmDialogComponent {
    @Input() title = 'Підтвердження';
    @Input() message = 'Ви впевнені?';
    @Input() confirmText = 'Видалити';
    @Input() cancelText = 'Скасувати';
    @Input() isVisible = false;
    @Input() isDanger = true;

    @Output() confirmed = new EventEmitter<void>();
    @Output() cancelled = new EventEmitter<void>();

    confirm(): void {
        this.confirmed.emit();
    }

    cancel(): void {
        this.cancelled.emit();
    }
}