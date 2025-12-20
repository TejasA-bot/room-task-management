import { Injectable, PLATFORM_ID, Inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse, User, CreateUserRequest } from '../models/user.model';
import { ApiResponse } from '../models/api-response.model';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly API_URL = environment.apiUrl;
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  private isBrowser: boolean;

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(platformId);
    if (this.isBrowser) {
      this.loadUserFromStorage();
    }
  }

  login(request: LoginRequest): Observable<ApiResponse<LoginResponse>> {
    return this.http.post<ApiResponse<LoginResponse>>(`${this.API_URL}/Auth/login`, request)
      .pipe(
        tap(response => {
          if (response.success && response.data) {
            this.setSession(response.data);
          }
        })
      );
  }

  register(request: CreateUserRequest): Observable<ApiResponse<User>> {
    return this.http.post<ApiResponse<User>>(`${this.API_URL}/Auth/register`, request);
  }

  logout(): void {
    if (this.isBrowser) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
    }
    this.currentUserSubject.next(null);
  }

  isLoggedIn(): boolean {
    if (!this.isBrowser) return false;
    
    const token = this.getToken();
    if (!token) return false;

    try {
      const decoded: any = jwtDecode(token);
      const expiry = decoded.exp * 1000;
      return Date.now() < expiry;
    } catch {
      return false;
    }
  }

  getToken(): string | null {
    if (!this.isBrowser) return null;
    return localStorage.getItem('token');
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  private setSession(authResult: LoginResponse): void {
    if (this.isBrowser) {
      localStorage.setItem('token', authResult.token);
      localStorage.setItem('user', JSON.stringify(authResult.user));
    }
    this.currentUserSubject.next(authResult.user);
  }

  private loadUserFromStorage(): void {
    if (!this.isBrowser) return;
    
    const userJson = localStorage.getItem('user');
    if (userJson) {
      try {
        const user = JSON.parse(userJson);
        this.currentUserSubject.next(user);
      } catch {
        this.logout();
      }
    }
  }
}
