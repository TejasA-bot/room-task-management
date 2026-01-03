import { Injectable, PLATFORM_ID, Inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private isBrowser: boolean;

  constructor(@Inject(PLATFORM_ID) platformId: Object) {
    this.isBrowser = isPlatformBrowser(platformId);
  }

  async requestPermission(): Promise<boolean> {
    if (!this.isBrowser) return false;

    if (!('Notification' in window)) {
      console.log('Browser does not support notifications');
      return false;
    }

    const permission = await Notification.requestPermission();
    console.log('Notification permission:', permission);
    return permission === 'granted';
  }

  showNotification(title: string, body: string) {
    if (!this.isBrowser) return;

    if (Notification.permission === 'granted') {
      const notification = new Notification(title, {
        body: body,
        icon: '/assets/icons/icon-192x192.png',
        badge: '/assets/icons/icon-72x72.png',
        tag: 'room-task',
        requireInteraction: true
      });

      notification.onclick = () => {
        window.focus();
        notification.close();
      };

      // Auto close after 10 seconds
      setTimeout(() => notification.close(), 10000);
    }
  }
}
