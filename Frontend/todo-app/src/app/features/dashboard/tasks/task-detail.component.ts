import {
    Component, EventEmitter, Input,
    OnChanges, OnDestroy, OnInit,
    Output, SimpleChanges,
    ChangeDetectionStrategy, ChangeDetectorRef
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { TaskService } from '../../../core/services/task.service';
import { TaskListService } from '../../../core/services/task-list.service';
import { CategoryService } from '../../../core/services/category.service';
import { Task, UpdateTaskDto, RepeatInterval } from '../../../shared/models/task.model';
import { Category } from '../../../shared/models/category.model';
import { TaskList } from '../../../shared/models/task-list.model';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';

const PASTEL_COLORS = [
    '#f2df75', '#9af7cd', '#f9a6ad',
    '#9bc6f5', '#cdb9f2', '#f3cea5'
];

const randomPastelColor = (): string =>
    PASTEL_COLORS[Math.floor(Math.random() * PASTEL_COLORS.length)];

export interface RepeatOption {
    label: string;
    value: RepeatInterval;
}

@Component({
    selector: 'app-task-detail',
    standalone: true,
    imports: [CommonModule, FormsModule, ConfirmDialogComponent],
    templateUrl: './task-detail.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class TaskDetailComponent implements OnInit, OnChanges, OnDestroy {
    @Input() task!: Task;
    @Input() categories: Category[] = [];
    @Output() taskUpdated = new EventEmitter<Task>();
    @Output() taskDeleted = new EventEmitter<number>();
    @Output() closed = new EventEmitter<void>();
    @Output() categoryChanged = new EventEmitter<void>();

    taskLists: TaskList[] = [];
    newCategoryName = '';
    editingCategoryId: number | null = null;
    editingCategoryName = '';
    isLoadingLists = true;
    showDeleteTaskDialog = false;
    showDeleteCategoryDialog = false;
    categoryToDelete: Category | null = null;

    readonly repeatOptions: RepeatOption[] = [
        { label: 'Без повтору', value: 'None' },
        { label: 'Щодня', value: 'Daily' },
        { label: 'Щотижня', value: 'Weekly' },
        { label: 'Щомісяця', value: 'Monthly' }
    ];

    private readonly destroy$ = new Subject<void>();

    constructor(
        private readonly taskService: TaskService,
        private readonly taskListService: TaskListService,
        private readonly categoryService: CategoryService,
        private readonly cdr: ChangeDetectorRef
    ) { }

    ngOnInit(): void {
        this.loadTaskLists();
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes['task'] && !changes['task'].firstChange) {
            this.taskLists.length === 0
                ? this.loadTaskLists()
                : this.cdr.markForCheck();
        }
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }

    get repeatLabel(): string {
        return this.repeatOptions.find(o => o.value === this.task.repeatInterval)?.label
            ?? 'Без повтору';
    }

    get taskListLabel(): string {
        return this.taskLists.find(l => l.id === this.task.taskListId)?.name
            ?? 'Без списку';
    }

    getTodayDate(): string {
        return new Date().toISOString().split('T')[0];
    }

    getTodayDateTime(): string {
        const now = new Date();
        return now.toISOString().slice(0, 16);
    }

    selectRepeatInterval(interval: RepeatInterval): void {
        this.update({ repeatInterval: interval });
    }

    onDateChange(field: 'dueDate' | 'reminderDate', value: string): void {
        if (!value) {
            this.update({ [field]: null });
            return;
        }
        const formatted = field === 'dueDate'
            ? `${value}T00:00:00`
            : `${value}:00`;
        this.update({ [field]: formatted });
    }

    private loadTaskLists(): void {
        this.isLoadingLists = true;
        this.taskListService.getAll()
            .pipe(takeUntil(this.destroy$))
            .subscribe({
                next: lists => {
                    this.taskLists = lists ?? [];
                    this.isLoadingLists = false;
                    this.cdr.markForCheck();
                },
                error: () => {
                    this.isLoadingLists = false;
                    this.cdr.markForCheck();
                }
            });
    }

    private buildUpdateDto(changes: Partial<UpdateTaskDto>): UpdateTaskDto {
        const categoryIds = (this.task.categories ?? [])
            .filter(Boolean)
            .map(c => c.id);

        return {
            title: this.task.title,
            description: this.task.description,
            isCompleted: this.task.isCompleted,
            isImportant: this.task.isImportant,
            isMyDay: this.task.isMyDay,
            dueDate: this.task.dueDate,
            reminderDate: this.task.reminderDate,
            repeatInterval: this.task.repeatInterval,
            taskListId: this.task.taskListId,
            categoryIds,
            ...changes
        };
    }

    update(changes: Partial<UpdateTaskDto>): void {
        this.taskService.update(this.task.id, this.buildUpdateDto(changes))
            .pipe(takeUntil(this.destroy$))
            .subscribe(updated => {
                this.task = updated;
                this.taskUpdated.emit(updated);
                this.cdr.markForCheck();
            });
    }

    hasCategory(category: Category): boolean {
        return this.task.categories?.some(c => c?.id === category.id) ?? false;
    }

    toggleCategory(category: Category, event?: Event): void {
        event?.stopPropagation();

        const currentCategories = this.task.categories ?? [];
        const isSelected = this.hasCategory(category);

        const updatedCategories = isSelected
            ? currentCategories.filter(c => c.id !== category.id)
            : [...currentCategories, category];

        this.task = { ...this.task, categories: updatedCategories };
        this.update({ categoryIds: updatedCategories.map(c => c.id) });
    }

    addNewCategory(event?: Event): void {
        event?.preventDefault();
        event?.stopPropagation();

        if (!this.newCategoryName.trim()) return;

        this.categoryService.create({
            name: this.newCategoryName.trim(),
            color: randomPastelColor()
        }).pipe(takeUntil(this.destroy$)).subscribe(newCat => {
            this.categories = [newCat, ...this.categories];
            this.newCategoryName = '';
            this.toggleCategory(newCat);
            this.categoryChanged.emit();
            this.cdr.markForCheck();
        });
    }

    startEditCategory(cat: Category, event: Event): void {
        event.stopPropagation();
        this.editingCategoryId = cat.id;
        this.editingCategoryName = cat.name;
    }

    saveCategoryEdit(cat: Category, event: Event): void {
        event.stopPropagation();
        if (!this.editingCategoryName.trim()) return;

        this.categoryService.update(cat.id, {
            name: this.editingCategoryName.trim(),
            color: cat.color
        }).pipe(takeUntil(this.destroy$)).subscribe(updated => {
            this.categories = this.categories.map(c => c.id === cat.id ? updated : c);
            this.task = {
                ...this.task,
                categories: this.task.categories?.map(c => c.id === cat.id ? updated : c) ?? []
            };
            this.editingCategoryId = null;
            this.taskUpdated.emit(this.task);
            this.categoryChanged.emit();
            this.cdr.markForCheck();
        });
    }

    openDeleteTaskDialog(): void {
        this.showDeleteTaskDialog = true;
    }

    onDeleteTaskConfirmed(): void {
        this.showDeleteTaskDialog = false;
        this.taskService.delete(this.task.id)
            .pipe(takeUntil(this.destroy$))
            .subscribe(() => this.taskDeleted.emit(this.task.id));
    }

    openDeleteCategoryDialog(cat: Category, event: Event): void {
        event.stopPropagation();
        this.categoryToDelete = cat;
        this.showDeleteCategoryDialog = true;
    }

    onDeleteCategoryConfirmed(): void {
        if (!this.categoryToDelete) return;

        const catId = this.categoryToDelete.id;
        this.showDeleteCategoryDialog = false;
        this.categoryToDelete = null;

        this.categoryService.delete(catId)
            .pipe(takeUntil(this.destroy$))
            .subscribe(() => {
                this.categories = this.categories.filter(c => c.id !== catId);
                const updatedCategories = (this.task.categories ?? [])
                    .filter(c => c.id !== catId);
                this.task = { ...this.task, categories: updatedCategories };
                this.taskUpdated.emit(this.task);
                this.categoryChanged.emit();
                this.cdr.markForCheck();
            });
    }
}