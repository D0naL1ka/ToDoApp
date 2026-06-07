import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Category, CreateCategoryDto, UpdateCategoryDto
} from '../../shared/models/category.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private apiUrl = `${environment.apiUrl}/categories`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<Category[]> {
    return this.http.get<Category[]>(this.apiUrl);
  }

  create(dto: CreateCategoryDto): Observable<Category> {
    return this.http.post<Category>(this.apiUrl, dto);
  }

  update(id: number, dto: UpdateCategoryDto): Observable<Category> {
    return this.http.put<Category>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}