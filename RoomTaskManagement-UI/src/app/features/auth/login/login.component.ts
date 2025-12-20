import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MaterialModule } from '../../../shared/material.module';
import { AuthService } from '../../../core/services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MaterialModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  loginForm: FormGroup;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  onLogin(): void {
    if (this.loginForm.invalid) {
      return;
    }

    this.isLoading = true;
    this.authService.login(this.loginForm.value).subscribe({
      next: (response) => {
        if (response.success) {
          this.snackBar.open('Login successful!', 'Close', { duration: 3000 });
          this.router.navigate(['/dashboard']);
        } else {
          this.snackBar.open(response.message, 'Close', { duration: 3000 });
        }
        this.isLoading = false;
      },
      error: (error) => {
        this.snackBar.open('Login failed. Please check your credentials.', 'Close', { duration: 3000 });
        this.isLoading = false;
      }
    });
  }
}
