import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, ReplaySubject } from 'rxjs';
import { User } from '../_models/user';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = environment.baseUrl;
  private currentUserSource = new ReplaySubject<User | null>(1);
  currentUser$ = this.currentUserSource.asObservable();
  http = inject(HttpClient);

  login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model).pipe(
      map((response: any) => {
        const user: User = response;
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      map((response: any) => {
        const user: User = response;
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  setCurrentUser(user: User) {
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;
    Array.isArray(roles) ? (user.roles = roles) : user.roles.push(roles);
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }

  getDecodedToken(token: string) {
    return JSON.parse(atob(token.split('.')[1]));
  }
}
