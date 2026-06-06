import {
  Component, OnInit, OnDestroy,
  ChangeDetectionStrategy, ChangeDetectorRef, Output, EventEmitter
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil, forkJoin } from 'rxjs';
import { TaskListService } from '../../../core/services/task-list.service';
import { TaskService } from '../../../core/services/task.service';
import { TaskList } from '../../models/task-list.model';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';

interface SystemCount {
  myDay: number;
  important: number;
  planned: number;
  all: number;
  completed: number;
}

const EMPTY_COUNTS: SystemCount = {
  myDay: 0, important: 0,
  planned: 0, all: 0, completed: 0
};

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, FormsModule, ConfirmDialogComponent],
  templateUrl: './sidebar.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SidebarComponent implements OnInit, OnDestroy {
  // Додаємо Output подію для зв'язку з Dashboard
  @Output() listSelected = new EventEmitter<void>();
  @Output() toggleMenu = new EventEmitter<void>(); 

  isCollapsed = false;
  taskLists: TaskList[] = [];
  newListName = '';
  isAddingList = false;
  editingListId: number | null = null;
  editingListName = '';
  counts: SystemCount = { ...EMPTY_COUNTS };
  showDeleteListDialog = false;
  listToDelete: TaskList | null = null;

  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly taskListService: TaskListService,
    private readonly taskService: TaskService,
    private readonly router: Router,
    private readonly cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadLists();
    this.loadCounts();

    this.taskService.taskChanged$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.loadLists();
        this.loadCounts();
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadCounts(): void {
    forkJoin({
      myDay:     this.taskService.getMyDay(),
      important: this.taskService.getImportant(),
      planned:   this.taskService.getPlanned(),
      all:       this.taskService.getAll({
        page: 1, pageSize: 1,
        isCompleted: false, isUnassigned: true
      }),
      completed: this.taskService.getAll({
        page: 1, pageSize: 1, isCompleted: true
      })
    }).pipe(takeUntil(this.destroy$)).subscribe({
      next: results => {
        this.counts = {
          myDay:     results.myDay.filter((t: any) => !t.isCompleted).length,
          important: results.important.filter((t: any) => !t.isCompleted).length,
          planned:   results.planned.filter((t: any) => !t.isCompleted).length,
          all:       results.all.totalCount,
          completed: results.completed.totalCount
        };
        this.cdr.markForCheck();
      },
      error: () => {
        this.counts = { ...EMPTY_COUNTS };
        this.cdr.markForCheck();
      }
    });
  }

  loadLists(): void {
    this.taskListService.getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: lists => {
          this.taskLists = lists ?? [];
          this.cdr.markForCheck();
        },
        error: () => {
          this.taskLists = [];
          this.cdr.markForCheck();
        }
      });
  }

  toggleSidebar(): void {
    this.isCollapsed = !this.isCollapsed;
    this.toggleMenu.emit();
  }

  addList(): void {
    if (!this.newListName.trim()) return;

    this.taskListService.create({ name: this.newListName.trim() })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: list => {
          this.taskLists = [...this.taskLists, list];
          this.newListName = '';
          this.isAddingList = false;
          this.router.navigate(['/list', list.id]);
          this.listSelected.emit();
          this.cdr.markForCheck();
        }
      });
  }

  startEdit(list: TaskList): void {
    this.editingListId = list.id;
    this.editingListName = list.name;
  }

  saveEdit(list: TaskList): void {
    if (!this.editingListName.trim()) return;

    this.taskListService.update(list.id, { name: this.editingListName.trim() })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: updated => {
          this.taskLists = this.taskLists.map(l => l.id === list.id ? updated : l);
          this.editingListId = null;
          this.cdr.markForCheck();
        }
      });
  }

  openDeleteListDialog(list: TaskList, event?: Event): void {
    event?.stopPropagation();
    this.listToDelete = list;
    this.showDeleteListDialog = true;
  }

  onDeleteListConfirmed(): void {
    if (!this.listToDelete) return;

    const listId = this.listToDelete.id;
    this.showDeleteListDialog = false;
    this.listToDelete = null;

    this.taskListService.delete(listId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.taskLists = this.taskLists.filter(l => l.id !== listId);
          this.router.navigate(['/tasks']);
          this.listSelected.emit();
          this.cdr.markForCheck();
        }
      });
  }
}