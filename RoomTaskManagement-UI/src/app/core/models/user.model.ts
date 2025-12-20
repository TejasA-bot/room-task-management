export interface User {
    id: number;
    username: string;
    fullName: string;
    phoneNumber: string;
    role: string;
    isOutOfStation: boolean;
  }
  
  export interface LoginRequest {
    username: string;
    password: string;
  }
  
  export interface LoginResponse {
    token: string;
    user: User;
  }
  
  export interface CreateUserRequest {
    username: string;
    password: string;
    fullName: string;
    phoneNumber: string;
    role: string;
  }
  