import { Component } from '@angular/core';
import { Member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { MemberCardComponent } from '../member-card/member-card.component';
import { Observable, take } from 'rxjs';
import { CommonModule } from '@angular/common';
import { Pagination } from '../../_models/pagination';
import { SharedModule } from '../../_modules/shared.module';
import { UserParams } from '../../_models/userParams';
import { AccountService } from '../../_services/account.service';
import { User } from '../../_models/user';

@Component({
  selector: 'app-member-list',
  standalone: true,
  imports: [MemberCardComponent, SharedModule, CommonModule],
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.css',
})
export class MemberListComponent {
  members: Member[] = [];
  pagination: Pagination | undefined;
  userParams: UserParams | undefined;
  user: User | undefined;
  genderList = [
    { value: 'male', display: 'Males' },
    { value: 'female', display: 'Females' },
  ];

  constructor(private membersService: MembersService) {
    this.userParams = this.membersService.getUserParams();
  }

  ngOnInit() {
    this.loadMembers();
  }

  loadMembers = () => {
    this.membersService.setUserParams(this.userParams!);

    this.membersService.getMembers(this.userParams!).subscribe((response) => {
      this.members = response.result!;
      this.pagination = response.pagination;
    });
  };

  resetFilters = () => {
    this.userParams = this.membersService.resetUserParams();

    this.loadMembers();
  };

  pageChanged = (event: any) => {
    this.userParams!.pageNumber = event.page;
    this.membersService.setUserParams(this.userParams!);
    this.loadMembers();
  };
}
