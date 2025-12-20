import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../../shared/material.module';
import { TaskService } from '../../../core/services/task.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-create-task-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MaterialModule],
  templateUrl: './create-task-dialog.component.html',
  styleUrl: './create-task-dialog.component.scss'
})
export class CreateTaskDialogComponent {
  taskForm: FormGroup;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private taskService: TaskService,
    private dialogRef: MatDialogRef<CreateTaskDialogComponent>,
    private snackBar: MatSnackBar
  ) {
    this.taskForm = this.fb.group({
      taskName: ['', Validators.required],
      description: ['']
    });
  }

  onSubmit(): void {
    if (this.taskForm.invalid) {
      return;
    }

    this.isLoading = true;
    this.taskService.createTask(this.taskForm.value).subscribe({
      next: (response) => {
        if (response.success) {
          this.snackBar.open('Task created successfully!', 'Close', { duration: 3000 });
          this.dialogRef.close(true);
        }
        this.isLoading = false;
      },
      error: (error) => {
        this.snackBar.open('Failed to create task', 'Close', { duration: 3000 });
        this.isLoading = false;
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
