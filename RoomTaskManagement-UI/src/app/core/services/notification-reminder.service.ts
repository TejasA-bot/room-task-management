import { Injectable, PLATFORM_ID, Inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { interval, Subscription } from 'rxjs';
import { TaskService } from './task.service';
import { AuthService } from './auth.service';
import { Task } from '../models/task.model';

@Injectable({
  providedIn: 'root'
})
export class NotificationReminderService {
  private isBrowser: boolean;
  private reminderSubscription?: Subscription;
  private pendingTasks: Task[] = [];

  constructor(
    @Inject(PLATFORM_ID) platformId: Object,
    private taskService: TaskService,
    private authService: AuthService
  ) {
    this.isBrowser = isPlatformBrowser(platformId);
  }

  startReminders(): void {
    if (!this.isBrowser) return;

    // Check every 1 minute
    this.reminderSubscription = interval(120000).subscribe(() => {
      this.checkPendingTasks();
    });

    // Initial check
    this.checkPendingTasks();
  }

  stopReminders(): void {
    if (this.reminderSubscription) {
      this.reminderSubscription.unsubscribe();
    }
  }

  private checkPendingTasks(): void {
    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) return;

    this.taskService.getAllTasks().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          // Filter tasks assigned to current user that are InProgress
          const myPendingTasks = response.data.filter(task => 
            task.currentAssignedTo === currentUser.fullName && 
            task.currentStatus === 'InProgress'
          );

          // Send reminder for each pending task
          myPendingTasks.forEach(task => {
            this.sendReminder(task);
          });
        }
      }
    });
  }

  private sendReminder(task: Task): void {
    if (Notification.permission === 'granted') {
      const notification = new Notification('⏰ Reminder: Task Pending!', {
        body: `${task.taskName} is still pending. Please complete it soon!`,
        icon: '/assets/icons/icon-192x192.png',
        badge: '/assets/icons/icon-72x72.png',
        tag: `reminder-${task.id}`,
        requireInteraction: true
      });

      notification.onclick = () => {
        window.focus();
        notification.close();
      };

      setTimeout(() => notification.close(), 15000);
    }
  }
}
