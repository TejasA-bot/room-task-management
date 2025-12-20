import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../../shared/material.module';
import { AuthService } from '../../../core/services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-create-member-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MaterialModule],
  templateUrl: './create-member-dialog.component.html',
  styleUrl: './create-member-dialog.component.scss'
})
export class CreateMemberDialogComponent {
  memberForm: FormGroup;
  isLoading = false;
  roles = ['Member', 'Admin'];

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private dialogRef: MatDialogRef<CreateMemberDialogComponent>,
    private snackBar: MatSnackBar
  ) {
    this.memberForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      fullName: ['', Validators.required],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^[0-9]{10}$/)]],
      role: ['Member', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.memberForm.invalid) {
      return;
    }

    this.isLoading = true;
    this.authService.register(this.memberForm.value).subscribe({
      next: (response) => {
        if (response.success) {
          this.snackBar.open('Member created successfully!', 'Close', { duration: 3000 });
          this.dialogRef.close(true);
        } else {
          this.snackBar.open(response.message, 'Close', { duration: 3000 });
        }
        this.isLoading = false;
      },
      error: (error) => {
        this.snackBar.open('Failed to create member', 'Close', { duration: 3000 });
        this.isLoading = false;
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
