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
      .then(() => console.log('SignalR Connected'))
      .catch(err => console.error('SignalR Connection Error:', err));

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
      this.taskTriggered$.next(data);
    });

    this.hubConnection.on('ReceiveTaskCompleted', (data) => {
      this.taskCompleted$.next(data);
    });

    this.hubConnection.on('ReceiveUserStatusChanged', (data) => {
      this.userStatusChanged$.next(data);
    });
  }
}