import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TaskListService } from '../../../core/services/task-list.service';
import { TaskList } from '../../models/task-list.model';
import { TaskService } from '../../../core/services/task.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, FormsModule],
  templateUrl: './sidebar.component.html',
})
export class SidebarComponent implements OnInit {
  isCollapsed = false;
  taskLists: TaskList[] = [];
  newListName = '';
  isAddingList = false;
  editingListId: number | null = null;
  editingListName = '';

  myDayCount = 0;
  importantCount = 0;
  plannedCount = 0;
  allTasksCount = 0;


  constructor(
    private taskListService: TaskListService,
    private taskService: TaskService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadLists();
    this.loadSystemCounts();
  }

  loadSystemCounts(): void {
    this.taskService.getMyDay().subscribe(tasks => this.myDayCount = tasks.length);
    this.taskService.getImportant().subscribe(tasks => this.importantCount = tasks.length);
    this.taskService.getPlanned().subscribe(tasks => this.plannedCount = tasks.length);
    // Для "Всіх задач" ми можемо просто запросити 1 сторінку і подивитися на загальну кількість
    this.taskService.getAll({ page: 1, pageSize: 1 }).subscribe(res => this.allTasksCount = res.totalCount);
  }

  toggleSidebar(): void {
    this.isCollapsed = !this.isCollapsed;
  }

  loadLists(): void {
    this.taskListService.getAll().subscribe(lists => {
      this.taskLists = lists;
    });
  }

  addList(): void {
    if (!this.newListName.trim()) return;
    this.taskListService.create({ name: this.newListName.trim() }).subscribe(list => {
      this.taskLists.push(list);
      this.newListName = '';
      this.isAddingList = false;
    });
  }

  startEdit(list: TaskList): void {
    this.editingListId = list.id;
    this.editingListName = list.name;
  }

  saveEdit(list: TaskList): void {
    if (!this.editingListName.trim()) return;
    this.taskListService.update(list.id, { name: this.editingListName.trim() }).subscribe(updated => {
      list.name = updated.name;
      this.editingListId = null;
    });
  }

  deleteList(list: TaskList): void {
    if (!confirm(`Видалити список "${list.name}"? Всі задачі також будуть видалені.`)) return;
    this.taskListService.delete(list.id).subscribe(() => {
      this.taskLists = this.taskLists.filter(l => l.id !== list.id);
      this.router.navigate(['/']);
    });
  }
}