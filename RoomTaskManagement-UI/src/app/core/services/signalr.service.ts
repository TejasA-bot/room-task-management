import { Injectable, PLATFORM_ID, Inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection!: signalR.HubConnection;
  private isBrowser: boolean;
  
  public taskTriggered$ = new Subject<any>();
  public taskCompleted$ = new Subject<any>();
  public userStatusChanged$ = new Subject<any>();

  constructor(@Inject(PLATFORM_ID) platformId: Object) {
    this.isBrowser = isPlatformBrowser(platformId);
  }

  startConnection(): void {
    if (!this.isBrowser) return;

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.hubUrl)
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('✅ SignalR Connected'))
      .catch(err => console.error('❌ SignalR Error:', err));

    this.registerEvents();
  }

  stopConnection(): void {
    if (this.isBrowser && this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  private registerEvents(): void {
    if (!this.isBrowser) return;

    this.hubConnection.on('ReceiveTaskTriggered', (data) => {
      console.log('📨 Task triggered event received:', data);
      
      // Show browser notification
      if (Notification.permission === 'granted') {
        const notification = new Notification('🔔 Room Task Alert', {
          body: `${data.taskName} - It's your turn, ${data.assignedTo}!`,
          icon: '/assets/icons/icon-192x192.png',
          badge: '/assets/icons/icon-72x72.png',
          tag: 'task-triggered',
          requireInteraction: true
        });

        notification.onclick = () => {
          window.focus();
          notification.close();
        };

        // Auto close after 15 seconds
        setTimeout(() => notification.close(), 15000);
      }
      
      this.taskTriggered$.next(data);
    });

    this.hubConnection.on('ReceiveTaskCompleted', (data) => {
      console.log('✅ Task completed event received:', data);
      
      if (Notification.permission === 'granted') {
        const notification = new Notification('✅ Task Completed', {
          body: `${data.taskName} completed by ${data.completedBy}`,
          icon: '/assets/icons/icon-192x192.png',
          tag: 'task-completed'
        });

        notification.onclick = () => {
          window.focus();
          notification.close();
        };

        setTimeout(() => notification.close(), 10000);
      }
      
      this.taskCompleted$.next(data);
    });

    this.hubConnection.on('ReceiveUserStatusChanged', (data) => {
      console.log('👤 User status changed:', data);
      this.userStatusChanged$.next(data);
    });

    this.hubConnection.on('ReceiveApprovalRequest', (data) => {
        console.log('📝 Approval request:', data);
        
        if (Notification.permission === 'granted') {
          const notification = new Notification('📝 Task Approval Needed', {
            body: `${data.completedBy} marked "${data.taskName}" as complete. Please verify!`,
            icon: '/assets/icons/icon-192x192.png',
            tag: 'approval-request',
            requireInteraction: true
          });
      
          notification.onclick = () => {
            window.focus();
            notification.close();
          };
        }
      });
      
      this.hubConnection.on('ReceiveTaskRejected', (data) => {
        console.log('❌ Task rejected:', data);
        
        if (Notification.permission === 'granted') {
          const notification = new Notification('❌ Task Rejected', {
            body: `Your completion of "${data.taskName}" was rejected. Please redo it properly.`,
            icon: '/assets/icons/icon-192x192.png',
            tag: 'task-rejected',
            requireInteraction: true
          });
        }
      });      
  }
}
