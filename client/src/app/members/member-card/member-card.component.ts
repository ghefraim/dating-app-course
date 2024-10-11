import { Component, Input } from '@angular/core';
import { Member } from '../../_models/member';
import { RouterModule } from '@angular/router';
import { MembersService } from '../../_services/members.service';
import { ToastrService } from 'ngx-toastr';
import { PresenceService } from '../../_services/presence.service';
import { SharedModule } from '../../_modules/shared.module';

@Component({
  selector: 'app-member-card',
  standalone: true,
  imports: [SharedModule],
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.css',
})
export class MemberCardComponent {
  @Input() member!: Partial<Member> | undefined;

  constructor(
    private memberService: MembersService,
    private toastr: ToastrService,
    public presence: PresenceService
  ) {}

  addLike(member: Partial<Member>) {
    this.memberService.addLike(member.username as string).subscribe(() => {
      this.toastr.success('You have liked ' + member.knownAs);
    });
  }
}
