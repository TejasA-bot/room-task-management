import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../shared/material.module';
import { TaskService } from '../../core/services/task.service';
import { AuthService } from '../../core/services/auth.service';
import { SignalRService } from '../../core/services/signalr.service';
import { NotificationService } from '../../core/services/notification.service';
import { NotificationReminderService } from '../../core/services/notification-reminder.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { User } from '../../core/models/user.model';
import { Task } from '../../core/models/task.model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit, OnDestroy {
  currentUser: User | null = null;
  tasks: Task[] = [];
  stats = {
    totalTasks: 0,
    pendingTasks: 0,
    totalMembers: 0,
    completedToday: 0
  };

  private subscriptions: Subscription[] = [];

  constructor(
    private taskService: TaskService,
    private authService: AuthService,
    private signalRService: SignalRService,
    private notificationService: NotificationService,
    private notificationReminder: NotificationReminderService,
    private snackBar: MatSnackBar
  ) {}

  async ngOnInit(): Promise<void> {
    this.currentUser = this.authService.getCurrentUser();
    this.loadDashboardData();
    this.setupSignalR();
    
    // Request notification permission
    const permissionGranted = await this.notificationService.requestPermission();
    if (permissionGranted) {
      // Start reminder notifications
      this.notificationReminder.startReminders();
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
    this.notificationReminder.stopReminders();
    this.signalRService.stopConnection();
  }

  loadDashboardData(): void {
    const sub1 = this.taskService.getDashboardStats().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.stats = response.data;
        }
      },
      error: (error) => {
        console.error('Failed to load stats:', error);
      }
    });

    const sub2 = this.taskService.getAllTasks().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.tasks = response.data;
        }
      },
      error: (error) => {
        console.error('Failed to load tasks:', error);
      }
    });

    this.subscriptions.push(sub1, sub2);
  }

  setupSignalR(): void {
    this.signalRService.startConnection();

    const sub1 = this.signalRService.taskTriggered$.subscribe(() => {
      this.loadDashboardData();
    });

    const sub2 = this.signalRService.taskCompleted$.subscribe(() => {
      this.loadDashboardData();
    });

    const sub3 = this.signalRService.userStatusChanged$.subscribe(() => {
      this.loadDashboardData();
    });

    this.subscriptions.push(sub1, sub2, sub3);
  }

  triggerTask(task: Task): void {
    this.taskService.triggerTask(task.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.snackBar.open('Task triggered successfully!', 'Close', { duration: 3000 });
          this.loadDashboardData();
        }
      },
      error: (error) => {
        this.snackBar.open('Failed to trigger task', 'Close', { duration: 3000 });
      }
    });
  }

  completeTask(task: Task): void {
    this.taskService.completeTask(task.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.snackBar.open('Task marked as complete - Pending approval', 'Close', { duration: 3000 });
          this.loadDashboardData();
        }
      },
      error: (error) => {
        this.snackBar.open('Failed to complete task', 'Close', { duration: 3000 });
      }
    });
  }

  approveTask(task: Task): void {
    this.taskService.approveTask(task.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.snackBar.open('Task approved! ✅', 'Close', { duration: 3000 });
          this.loadDashboardData();
        }
      },
      error: (error) => {
        this.snackBar.open('Failed to approve task', 'Close', { duration: 3000 });
      }
    });
  }

  rejectTask(task: Task): void {
    if (confirm('Are you sure you want to reject this task completion?')) {
      this.taskService.rejectTask(task.id).subscribe({
        next: (response) => {
          if (response.success) {
            this.snackBar.open('Task rejected - User notified to redo', 'Close', { duration: 3000 });
            this.loadDashboardData();
          }
        },
        error: (error) => {
          this.snackBar.open('Failed to reject task', 'Close', { duration: 3000 });
        }
      });
    }
  }

  canApprove(task: Task): boolean {
    // Check if current user triggered this task
    // For now, allow all admins and superadmins
    return this.currentUser?.role === 'SuperAdmin' || 
           this.currentUser?.role === 'Admin';
  }
}
