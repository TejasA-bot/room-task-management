import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MaterialModule } from '../../material.module';
import { AuthService } from '../../../core/services/auth.service';
import { User } from '../../../core/models/user.model';
import { UserService } from '../../../core/services/user.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDividerModule } from '@angular/material/divider';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule, MaterialModule, MatDividerModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent implements OnInit {
  currentUser: User | null = null;

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });
  }

  toggleOutOfStation(): void {
    if (this.currentUser) {
      this.userService.toggleOutOfStation(this.currentUser.id).subscribe({
        next: (response) => {
          if (response.success) {
            this.currentUser!.isOutOfStation = !this.currentUser!.isOutOfStation;
            const status = this.currentUser!.isOutOfStation ? 'Out of Station' : 'Available';
            this.snackBar.open(`Status updated to: ${status}`, 'Close', { duration: 3000 });
          }
        },
        error: (error) => {
          this.snackBar.open('Failed to update status', 'Close', { duration: 3000 });
        }
      });
    }
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
