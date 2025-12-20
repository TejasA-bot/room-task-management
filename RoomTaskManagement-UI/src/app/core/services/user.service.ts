import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { User } from '../models/user.model';
import { ApiResponse } from '../models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly API_URL = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getAllUsers(): Observable<ApiResponse<User[]>> {
    return this.http.get<ApiResponse<User[]>>(`${this.API_URL}/Users`);
  }

  getUserById(id: number): Observable<ApiResponse<User>> {
    return this.http.get<ApiResponse<User>>(`${this.API_URL}/Users/${id}`);
  }

  updateUser(id: number, user: User): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.API_URL}/Users/${id}`, user);
  }

  toggleOutOfStation(id: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.API_URL}/Users/${id}/toggle-out-of-station`, {});
  }

  deleteUser(id: number): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.API_URL}/Users/${id}`);
  }
}
