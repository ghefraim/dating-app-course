import { Component, inject } from '@angular/core';
import { Member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { ActivatedRoute } from '@angular/router';
import { SharedModule } from '../../_modules/shared.module';
import {
  NgxGalleryAnimation,
  NgxGalleryImage,
  NgxGalleryModule,
  NgxGalleryOptions,
} from '@kolkov/ngx-gallery';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [SharedModule, NgxGalleryModule],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css',
})
export class MemberDetailComponent {
  member: Member | undefined;

  galleryOptions: NgxGalleryOptions[] | undefined;
  galleryImages: NgxGalleryImage[] | undefined;

  memberService = inject(MembersService);
  route = inject(ActivatedRoute);

  ngOnInit() {
    this.loadMember();

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
  }

  loadMember() {
    this.memberService
      .getMember(this.route.snapshot.paramMap.get('username')!)
      .subscribe((member) => {
        this.member = member;
        this.galleryImages = this.getImages();
      });
  }

  getImages(): NgxGalleryImage[] {
    return this.member!.photos?.map((photo) => ({
      small: photo?.url,
      medium: photo?.url,
      big: photo?.url,
    }))!;
  }
}
