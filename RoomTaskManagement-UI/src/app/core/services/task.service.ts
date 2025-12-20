import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Task, CreateTaskRequest, TriggerTaskRequest } from '../models/task.model';
import { ApiResponse } from '../models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private readonly API_URL = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getAllTasks(): Observable<ApiResponse<Task[]>> {
    return this.http.get<ApiResponse<Task[]>>(`${this.API_URL}/Tasks`);
  }

  getTaskById(id: number): Observable<ApiResponse<Task>> {
    return this.http.get<ApiResponse<Task>>(`${this.API_URL}/Tasks/${id}`);
  }

  createTask(request: CreateTaskRequest): Observable<ApiResponse<Task>> {
    return this.http.post<ApiResponse<Task>>(`${this.API_URL}/Tasks`, request);
  }

  triggerTask(request: TriggerTaskRequest): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.API_URL}/Tasks/trigger`, request);
  }

  completeTask(taskId: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.API_URL}/Tasks/${taskId}/complete`, {});
  }

  deleteTask(id: number): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.API_URL}/Tasks/${id}`);
  }
}
