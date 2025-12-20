import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../../shared/material.module';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UserService } from '../../../core/services/user.service';
import { AuthService } from '../../../core/services/auth.service';
import { User } from '../../../core/models/user.model';
import { CreateMemberDialogComponent } from '../create-member-dialog/create-member-dialog.component';

@Component({
  selector: 'app-member-list',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.scss'
})
export class MemberListComponent implements OnInit {
  members: User[] = [];
  currentUser: User | null = null;
  displayedColumns: string[] = ['fullName', 'username', 'phoneNumber', 'role', 'status', 'actions'];

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    this.loadMembers();
  }

  loadMembers(): void {
    this.userService.getAllUsers().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.members = response.data;
        }
      },
      error: (error) => {
        this.snackBar.open('Failed to load members', 'Close', { duration: 3000 });
      }
    });
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(CreateMemberDialogComponent, {
      width: '600px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadMembers();
      }
    });
  }

  toggleMemberStatus(member: User): void {
    this.userService.toggleOutOfStation(member.id).subscribe({
      next: (response) => {
        if (response.success) {
          member.isOutOfStation = !member.isOutOfStation;
          const status = member.isOutOfStation ? 'Out of Station' : 'Available';
          this.snackBar.open(`${member.fullName} is now ${status}`, 'Close', { duration: 3000 });
        }
      },
      error: (error) => {
        this.snackBar.open('Failed to update status', 'Close', { duration: 3000 });
      }
    });
  }

  deleteMember(member: User): void {
    if (member.role === 'SuperAdmin') {
      this.snackBar.open('Cannot delete SuperAdmin', 'Close', { duration: 3000 });
      return;
    }

    if (confirm(`Are you sure you want to delete "${member.fullName}"?`)) {
      this.userService.deleteUser(member.id).subscribe({
        next: (response) => {
          if (response.success) {
            this.snackBar.open('Member deleted successfully', 'Close', { duration: 3000 });
            this.loadMembers();
          }
        },
        error: (error) => {
          this.snackBar.open('Failed to delete member', 'Close', { duration: 3000 });
        }
      });
    }
  }

  canManageMembers(): boolean {
    return this.currentUser?.role === 'SuperAdmin' || this.currentUser?.role === 'Admin';
  }

  getRoleBadgeColor(role: string): string {
    switch (role) {
      case 'SuperAdmin': return 'warn';
      case 'Admin': return 'accent';
      default: return 'primary';
    }
  }
}
