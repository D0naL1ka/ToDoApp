import { Category } from './category.model';

export type RepeatInterval = 'None' | 'Daily' | 'Weekly' | 'Monthly';

export interface Task {
  id: number;
  title: string;
  description: string;
  isCompleted: boolean;
  isImportant: boolean;
  isMyDay: boolean;
  dueDate: string | null;
  reminderDate: string | null;
  repeatInterval: RepeatInterval;
  createdAt: string;
  taskListId: number | null;
  taskListName: string | null;
  categories: Category[];
}

export interface CreateTaskDto {
  title: string;
  taskListId: number | null;
}

export interface UpdateTaskDto {
  title: string;
  description: string;
  isCompleted: boolean;
  isImportant: boolean;
  isMyDay: boolean;
  dueDate: string | null;
  reminderDate: string | null;
  repeatInterval: RepeatInterval;
  taskListId: number | null;
  categoryIds: number[];
}

export interface TaskFilter {
  page: number;
  pageSize: number;
  search?: string;
  categoryId?: number;
  grouped?: boolean;
  isCompleted?: boolean;
  isUnassigned?: boolean;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}