import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../shared/material.module';
import { TaskService } from '../../core/services/task.service';
import { UserService } from '../../core/services/user.service';
import { SignalRService } from '../../core/services/signalr.service';
import { AuthService } from '../../core/services/auth.service';
import { Task } from '../../core/models/task.model';
import { User } from '../../core/models/user.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit, OnDestroy {
  tasks: Task[] = [];
  users: User[] = [];
  currentUser: User | null = null;
  stats = {
    totalTasks: 0,
    activeTasks: 0,
    myPendingTasks: 0,
    availableMembers: 0
  };

  private subscriptions: Subscription[] = [];

  constructor(
    private taskService: TaskService,
    private userService: UserService,
    private signalRService: SignalRService,
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    this.loadDashboardData();
    this.setupSignalR();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  loadDashboardData(): void {
    // Load tasks
    this.taskService.getAllTasks().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.tasks = response.data;
          this.calculateStats();
        }
      },
      error: (error) => {
        this.snackBar.open('Failed to load tasks', 'Close', { duration: 3000 });
      }
    });

    // Load users
    this.userService.getAllUsers().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.users = response.data;
          this.calculateStats();
        }
      },
      error: (error) => {
        this.snackBar.open('Failed to load users', 'Close', { duration: 3000 });
      }
    });
  }

  calculateStats(): void {
    this.stats.totalTasks = this.tasks.length;
    this.stats.activeTasks = this.tasks.filter(t => t.isActive).length;
    this.stats.myPendingTasks = this.tasks.filter(t => 
      t.currentAssignedTo === this.currentUser?.fullName && t.currentStatus === 'InProgress'
    ).length;
    this.stats.availableMembers = this.users.filter(u => !u.isOutOfStation).length;
  }

  setupSignalR(): void {
    // Listen for task triggered events
    const taskTriggeredSub = this.signalRService.taskTriggered$.subscribe((data) => {
      this.snackBar.open(`${data.taskName} triggered for ${data.assignedTo}`, 'Close', { duration: 5000 });
      this.loadDashboardData();
    });

    // Listen for task completed events
    const taskCompletedSub = this.signalRService.taskCompleted$.subscribe((data) => {
      this.snackBar.open(`${data.taskName} completed by ${data.completedBy}`, 'Close', { duration: 5000 });
      this.loadDashboardData();
    });

    // Listen for user status changes
    const userStatusSub = this.signalRService.userStatusChanged$.subscribe((data) => {
      const status = data.isOutOfStation ? 'Out of Station' : 'Available';
      this.snackBar.open(`${data.userName} is now ${status}`, 'Close', { duration: 3000 });
      this.loadDashboardData();
    });

    this.subscriptions.push(taskTriggeredSub, taskCompletedSub, userStatusSub);
  }

  triggerTask(task: Task): void {
    if (!task.canTrigger) {
      this.snackBar.open('This task is already in progress', 'Close', { duration: 3000 });
      return;
    }

    this.taskService.triggerTask({ taskId: task.id, triggeredBy: this.currentUser!.id }).subscribe({
      next: (response) => {
        if (response.success) {
          this.snackBar.open(response.message, 'Close', { duration: 3000 });
          this.loadDashboardData();
        }
      },
      error: (error) => {
        this.snackBar.open('Failed to trigger task', 'Close', { duration: 3000 });
      }
    });
  }

  completeTask(task: Task): void {
    if (task.currentAssignedTo !== this.currentUser?.fullName) {
      this.snackBar.open('Only the assigned person can complete this task', 'Close', { duration: 3000 });
      return;
    }

    this.taskService.completeTask(task.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.snackBar.open('Task completed successfully!', 'Close', { duration: 3000 });
          this.loadDashboardData();
        }
      },
      error: (error) => {
        this.snackBar.open('Failed to complete task', 'Close', { duration: 3000 });
      }
    });
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'Available': return 'primary';
      case 'InProgress': return 'warn';
      case 'Completed': return 'accent';
      default: return 'primary';
    }
  }
}
