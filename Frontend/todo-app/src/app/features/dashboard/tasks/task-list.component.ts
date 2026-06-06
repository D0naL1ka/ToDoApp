import {
  Component, OnInit, OnDestroy,
  ChangeDetectionStrategy, ChangeDetectorRef
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { TaskService } from '../../../core/services/task.service';
import { CategoryService } from '../../../core/services/category.service';
import { Task, TaskFilter, PagedResult } from '../../../shared/models/task.model';
import { Category } from '../../../shared/models/category.model';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';

export interface SortOption {
  label: string;
  value: string;
}

const PAGE_TITLES: Record<string, string> = {
  tasks:     'Всі задачі',
  myday:     'Мій день',
  important: 'Важливо',
  planned:   'Заплановано',
  completed: 'Завершені'
};

const NON_PAGED_ROUTES = new Set(['myday', 'important', 'planned']);

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule, FormsModule, PaginationComponent],
  templateUrl: './task-list.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TaskListComponent implements OnInit, OnDestroy {
  tasks: Task[] = [];
  categories: Category[] = [];
  selectedTask: Task | null = null;
  isLoading = false;
  newTaskTitle = '';
  currentListId: number | null = null;
  currentRoute = '';
  pageTitle = 'Всі задачі';
  pagedResult: PagedResult<Task> | null = null;
  currentSort = 'default';

  filter: TaskFilter = { page: 1, pageSize: 10, search: '' };

  readonly sortOptions: SortOption[] = [
    { label: 'За замовчуванням', value: 'default' },
    { label: 'Найближчі дати',   value: 'dueDateAsc' },
    { label: 'Найдальші дати',   value: 'dueDateDesc' },
    { label: 'Спочатку нові',    value: 'createdAtDesc' }
  ];

  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly taskService: TaskService,
    private readonly categoryService: CategoryService,
    private readonly route: ActivatedRoute,
    private readonly cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.route.params
      .pipe(takeUntil(this.destroy$))
      .subscribe(params => {
        this.currentListId = params['id'] ? +params['id'] : null;
        this.currentRoute = this.currentListId
          ? 'list'
          : (this.route.snapshot.url[0]?.path ?? 'tasks');
        this.pageTitle = PAGE_TITLES[this.currentRoute] ?? 'Список задач';
        this.filter.page = 1;
        this.loadTasks();
      });

    this.categoryService.getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe(cats => {
        this.categories = cats ?? [];
        this.cdr.markForCheck();
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  get currentSortLabel(): string {
    return this.sortOptions.find(o => o.value === this.currentSort)?.label
      ?? 'Сортування';
  }

  get currentCategoryLabel(): string {
    if (!this.filter.categoryId) return 'Всі категорії';
    return this.categories.find(c => c.id === this.filter.categoryId)?.name
      ?? 'Всі категорії';
  }

  loadTasks(): void {
    this.isLoading = true;
    this.cdr.markForCheck();

    this.currentListId
      ? this.loadListTasks(this.currentListId)
      : this.loadSystemTasks();
  }

  private loadListTasks(listId: number): void {
    this.taskService.getByTaskList(listId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: tasks => {
          const raw = Array.isArray(tasks) ? tasks : [];
          this.tasks = this.applySorting(this.applyLocalFilters(raw));
          this.pagedResult = null;
          this.isLoading = false;
          this.cdr.markForCheck();
        },
        error: () => {
          this.isLoading = false;
          this.cdr.markForCheck();
        }
      });
  }

  private loadSystemTasks(): void {
    const loaders: Record<string, () => any> = {
      myday:     () => this.taskService.getMyDay(),
      important: () => this.taskService.getImportant(),
      planned:   () => this.taskService.getPlanned(),
      tasks:     () => this.taskService.getAll({
        ...this.filter, isCompleted: false, isUnassigned: true
      }),
      completed: () => this.taskService.getAll({
        ...this.filter, isCompleted: true
      })
    };

    const loader = loaders[this.currentRoute];
    if (!loader) return;

    loader().pipe(takeUntil(this.destroy$)).subscribe({
      next: (result: any) => {
        const isPaged = !!result?.items;
        const raw: Task[] = isPaged
          ? result.items
          : (Array.isArray(result) ? result : []);

        const filtered = NON_PAGED_ROUTES.has(this.currentRoute)
          ? this.applyLocalFilters(raw)
          : raw;

        this.tasks = this.applySorting(filtered);
        this.pagedResult = isPaged ? result : null;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  private applyLocalFilters(tasks: Task[]): Task[] {
    return tasks.filter(task => {
      if (this.currentRoute !== 'completed' && task.isCompleted) return false;

      const matchesSearch = !this.filter.search ||
        task.title.toLowerCase().includes(
          this.filter.search.toLowerCase().trim()
        );

      const matchesCategory = !this.filter.categoryId ||
        task.categories?.some(c => c.id === this.filter.categoryId);

      return matchesSearch && matchesCategory;
    });
  }

  private applySorting(tasks: Task[]): Task[] {
    if (this.currentSort === 'default') return tasks;

    return [...tasks].sort((a, b) => {
      if (this.currentSort === 'dueDateAsc' || this.currentSort === 'dueDateDesc') {
        if (!a.dueDate && !b.dueDate) return 0;
        if (!a.dueDate) return 1;
        if (!b.dueDate) return -1;

        const diff = new Date(a.dueDate).getTime() - new Date(b.dueDate).getTime();
        return this.currentSort === 'dueDateAsc' ? diff : -diff;
      }

      if (this.currentSort === 'createdAtDesc') {
        return new Date(b.createdAt ?? 0).getTime() -
               new Date(a.createdAt ?? 0).getTime();
      }

      return 0;
    });
  }

  selectCategory(categoryId: number | null | undefined): void {
    this.filter.categoryId = categoryId ?? undefined;
    this.filter.page = 1;
    this.loadTasks();
  }

  selectSort(sortValue: string): void {
    this.currentSort = sortValue;
    this.loadTasks();
  }

  onSearch(): void {
    this.filter.page = 1;
    this.loadTasks();
  }

  onPageChange(page: number): void {
    this.filter.page = page;
    this.loadTasks();
  }

  addTask(): void {
    if (!this.newTaskTitle.trim()) return;

    this.taskService.create({
      title: this.newTaskTitle.trim(),
      taskListId: this.currentListId
    }).pipe(takeUntil(this.destroy$)).subscribe(() => {
      this.newTaskTitle = '';
      this.loadTasks();
      this.taskService.notifyTaskChange();
    });
  }

  toggleComplete(task: Task, event: Event): void {
    event.stopPropagation();
    const categoryIds = task.categories?.map(c => c.id) ?? [];
    this.taskService.update(task.id, {
      ...task, isCompleted: !task.isCompleted, categoryIds
    }).pipe(takeUntil(this.destroy$)).subscribe(() => {
      this.loadTasks();
      this.taskService.notifyTaskChange();
    });
  }

  toggleImportant(task: Task, event: Event): void {
    event.stopPropagation();
    const categoryIds = task.categories?.map(c => c.id) ?? [];
    this.taskService.update(task.id, {
      ...task, isImportant: !task.isImportant, categoryIds
    }).pipe(takeUntil(this.destroy$)).subscribe(() => {
      this.loadTasks();
      this.taskService.notifyTaskChange();
    });
  }

  openDetail(task: Task): void {
    this.selectedTask = { ...task };
    this.cdr.markForCheck();
  }

  onTaskUpdated(updated: Task): void {
    this.selectedTask = updated;
    this.loadTasks();
    this.taskService.notifyTaskChange();
  }

  onCategoryChanged(): void {
    this.loadTasks();
    this.categoryService.getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe(cats => {
        this.categories = cats ?? [];
        this.cdr.markForCheck();
      });
  }

  onTaskDeleted(id: number): void {
    if (this.tasks.length === 1 && this.filter.page > 1) this.filter.page--;
    this.selectedTask = null;
    this.loadTasks();
    this.taskService.notifyTaskChange();
  }

  closeDetail(): void {
    this.selectedTask = null;
    this.cdr.markForCheck();
  }

  formatDate(date: string | null): string {
    if (!date) return '';
    return new Date(date).toLocaleDateString('uk-UA', {
      day: 'numeric', month: 'short'
    });
  }

  isPastDue(date: string | null): boolean {
    if (!date) return false;
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    return new Date(date) < today;
  }
}