import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../../shared/material.module';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TaskService } from '../../../core/services/task.service';
import { AuthService } from '../../../core/services/auth.service';
import { Task } from '../../../core/models/task.model';
import { User } from '../../../core/models/user.model';
import { CreateTaskDialogComponent } from '../create-task-dialog/create-task-dialog.component';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.scss'
})
export class TaskListComponent implements OnInit {
  tasks: Task[] = [];
  currentUser: User | null = null;
  displayedColumns: string[] = ['taskName', 'description', 'status', 'assignedTo', 'actions'];

  constructor(
    private taskService: TaskService,
    private authService: AuthService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    this.loadTasks();
  }

  loadTasks(): void {
    this.taskService.getAllTasks().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.tasks = response.data;
        }
      },
      error: (error) => {
        this.snackBar.open('Failed to load tasks', 'Close', { duration: 3000 });
      }
    });
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(CreateTaskDialogComponent, {
      width: '500px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadTasks();
      }
    });
  }

  deleteTask(task: Task): void {
    if (confirm(`Are you sure you want to delete "${task.taskName}"?`)) {
      this.taskService.deleteTask(task.id).subscribe({
        next: (response) => {
          if (response.success) {
            this.snackBar.open('Task deleted successfully', 'Close', { duration: 3000 });
            this.loadTasks();
          }
        },
        error: (error) => {
          this.snackBar.open('Failed to delete task', 'Close', { duration: 3000 });
        }
      });
    }
  }

  canManageTasks(): boolean {
    return this.currentUser?.role === 'SuperAdmin' || this.currentUser?.role === 'Admin';
  }
}
