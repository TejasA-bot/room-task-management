import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, Router, NavigationEnd } from '@angular/router';
import { NavbarComponent } from './shared/components/navbar/navbar.component';
import { SignalRService } from './core/services/signalr.service';
import { AuthService } from './core/services/auth.service';
import { filter } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, NavbarComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  showNavbar = false;

  constructor(
    private signalRService: SignalRService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Hide navbar on login page
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      this.showNavbar = !event.url.includes('/login');
    });

    // Start SignalR connection if logged in
    if (this.authService.isLoggedIn()) {
      this.signalRService.startConnection();
    }
  }
}
