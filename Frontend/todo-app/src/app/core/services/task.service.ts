import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Task, CreateTaskDto, UpdateTaskDto,
  TaskFilter, PagedResult
} from '../../shared/models/task.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class TaskService {
    private apiUrl = `${environment.apiUrl}/tasks`;

  constructor(private http: HttpClient) {}

  getAll(filter: TaskFilter): Observable<PagedResult<Task>> {
    let params = new HttpParams()
      .set('page', filter.page)
      .set('pageSize', filter.pageSize);

    if (filter.search) params = params.set('search', filter.search);
    if (filter.categoryId) params = params.set('categoryId', filter.categoryId);
    if (filter.grouped !== undefined) params = params.set('grouped', filter.grouped);

    return this.http.get<PagedResult<Task>>(this.apiUrl, { params });
  }

  getMyDay(): Observable<Task[]> {
    return this.http.get<Task[]>(`${this.apiUrl}/myday`);
  }

  getImportant(): Observable<Task[]> {
    return this.http.get<Task[]>(`${this.apiUrl}/important`);
  }

  getPlanned(): Observable<Task[]> {
    return this.http.get<Task[]>(`${this.apiUrl}/planned`);
  }

  getByTaskList(taskListId: number): Observable<Task[]> {
    return this.http.get<Task[]>(
      `${environment.apiUrl}/tasklists/${taskListId}/tasks`
    );
  }

  getById(id: number): Observable<Task> {
    return this.http.get<Task>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateTaskDto): Observable<Task> {
    return this.http.post<Task>(this.apiUrl, dto);
  }

  update(id: number, dto: UpdateTaskDto): Observable<Task> {
    return this.http.put<Task>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}