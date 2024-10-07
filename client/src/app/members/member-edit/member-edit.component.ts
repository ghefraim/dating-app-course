import { Component, HostListener, inject, ViewChild } from '@angular/core';
import { Member } from '../../_models/member';
import { User } from '../../_models/user';
import { AccountService } from '../../_services/account.service';
import { MembersService } from '../../_services/members.service';
import { take } from 'rxjs';
import { SharedModule } from '../../_modules/shared.module';
import { NgxGalleryModule } from '@kolkov/ngx-gallery';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-member-edit',
  standalone: true,
  imports: [SharedModule, NgxGalleryModule, FormsModule],
  templateUrl: './member-edit.component.html',
  styleUrl: './member-edit.component.css',
})
export class MemberEditComponent {
  @ViewChild('editForm') editForm: NgForm | undefined;
  member: Member | undefined;
  user: User | undefined;
  @HostListener('window:beforeunload', ['$event']) unloadNotification(
    $event: any
  ) {
    if (this.editForm && this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  accountService = inject(AccountService);
  memberService = inject(MembersService);
  toastr = inject(ToastrService);

  constructor() {
    this.accountService.currentUser$.pipe(take(1)).subscribe((user) => {
      if (user) {
        this.user = user;
      }
    });

    this.loadMember();
  }

  loadMember() {
    this.memberService.getMember(this.user!.username).subscribe((member) => {
      this.member = member;
    });
  }

  updateMember() {
    this.memberService.updateMember(this.member!).subscribe(() => {
      this.toastr.success('Profile updated successfully');
      this.editForm!.reset(this.member);
    });
  }
}
