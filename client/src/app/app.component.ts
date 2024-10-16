import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavComponent } from './nav/nav.component';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { AccountService } from './_services/account.service';
import { HomeComponent } from './home/home.component';
import { SharedModule } from './_modules/shared.module';
import { ReactiveFormsModule } from '@angular/forms';
import { HasRoleDirective } from './_directives/has-role.directive';
import { PresenceService } from './_services/presence.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    CommonModule,
    NavComponent,
    SharedModule,
    HomeComponent,
    ReactiveFormsModule,
    HasRoleDirective,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  title = 'The dating app';
  users: any;
  constructor(
    private accountService: AccountService,
    private presence: PresenceService
  ) {}

  ngOnInit(): void {
    this.setCurrentUser();
  }

  setCurrentUser() {
    const user = JSON.parse(localStorage.getItem('user') as string);
    if (user) {
      this.accountService.setCurrentUser(user);
      this.presence.createHubConnection(user);
    }
  }
}
