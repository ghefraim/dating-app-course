import { Component, inject, ViewChild, AfterViewInit } from '@angular/core';
import { Member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { ActivatedRoute, Router } from '@angular/router';
import { SharedModule } from '../../_modules/shared.module';
import {
  NgxGalleryAnimation,
  NgxGalleryImage,
  NgxGalleryModule,
  NgxGalleryOptions,
} from '@kolkov/ngx-gallery';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { MessageService } from '../../_services/message.service';
import { Message } from '../../_models/message';
import { PresenceService } from '../../_services/presence.service';
import { AccountService } from '../../_services/account.service';
import { User } from '../../_models/user';
import { take } from 'rxjs';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [SharedModule, NgxGalleryModule, MemberMessagesComponent],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css',
})
export class MemberDetailComponent implements AfterViewInit {
  @ViewChild('memberTabs') memberTabs?: TabsetComponent;

  member: Member | undefined;
  messages: Message[] = [];
  user?: User;

  galleryOptions: NgxGalleryOptions[] | undefined;
  galleryImages: NgxGalleryImage[] | undefined;

  activeTab: TabDirective | undefined;

  memberService = inject(MembersService);
  messageService = inject(MessageService);
  route = inject(ActivatedRoute);
  presence = inject(PresenceService);
  accountService = inject(AccountService);
  router = inject(Router);

  constructor() {
    this.accountService.currentUser$.pipe(take(1)).subscribe((user) => {
      this.user = user!;
    });
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.member = data['member'];
    });

    this.route.queryParams.subscribe((params) => {
      params['tab'] && this.member ? this.selectTab(params['tab']) : null;
    });

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false,
      },
    ];

    this.galleryImages = this.getImages();
  }

  ngAfterViewInit() {
    if (this.member) {
      this.route.queryParams.subscribe((params) => {
        params['tab'] ? this.selectTab(params['tab']) : this.selectTab(0);
      });
    }
  }

  loadMember() {
    this.memberService
      .getMember(this.route.snapshot.paramMap.get('username')!)
      .subscribe((member) => {
        this.member = member;
      });
  }

  getImages(): NgxGalleryImage[] {
    return this.member!.photos?.map((photo) => ({
      small: photo?.url,
      medium: photo?.url,
      big: photo?.url,
    }))!;
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;

    if (this.activeTab?.heading === 'Messages' && this.messages.length === 0) {
      this.messageService.createHubConnection(
        this.user!,
        this.member?.username!
      );
    } else {
      this.messageService.stopHubConnection();
    }
  }

  selectTab(tabId: number) {
    if (
      this.memberTabs &&
      this.memberTabs.tabs &&
      this.memberTabs.tabs[tabId]
    ) {
      this.memberTabs.tabs[tabId].active = true;
    }
  }

  loadMessages() {
    this.messageService
      .getMessageThread(this.member?.username!)
      .subscribe((messages) => {
        this.messages = messages;
      });
  }

  ngOnDestroy() {
    this.messageService.stopHubConnection();
  }
}
