import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  TaskList, CreateTaskListDto, UpdateTaskListDto
} from '../../shared/models/task-list.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class TaskListService {
  private apiUrl = `${environment.apiUrl}/tasklists`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<TaskList[]> {
    return this.http.get<TaskList[]>(this.apiUrl);
  }

  create(dto: CreateTaskListDto): Observable<TaskList> {
    return this.http.post<TaskList>(this.apiUrl, dto);
  }

  update(id: number, dto: UpdateTaskListDto): Observable<TaskList> {
    return this.http.put<TaskList>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}