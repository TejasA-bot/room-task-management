import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { Task } from '../models/task.model';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private API_URL = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getAllTasks(): Observable<ApiResponse<Task[]>> {
    return this.http.get<ApiResponse<Task[]>>(`${this.API_URL}/Tasks`);
  }

  getDashboardStats(): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${this.API_URL}/Tasks/dashboard-stats`);
  }

  createTask(task: any): Observable<ApiResponse<Task>> {
    return this.http.post<ApiResponse<Task>>(`${this.API_URL}/Tasks`, task);
  }

  triggerTask(taskId: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.API_URL}/Tasks/${taskId}/trigger`, {});
  }

  completeTask(taskId: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.API_URL}/Tasks/${taskId}/complete`, {});
  }

  approveTask(taskId: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.API_URL}/Tasks/${taskId}/approve`, {});
  }

  rejectTask(taskId: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.API_URL}/Tasks/${taskId}/reject`, {});
  }

  deleteTask(taskId: number): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.API_URL}/Tasks/${taskId}`);
  }
}