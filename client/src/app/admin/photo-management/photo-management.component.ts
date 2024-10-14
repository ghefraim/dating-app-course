import { Component, inject } from '@angular/core';
import { PhotosService } from '../../_services/photos.service';
import { Photo } from '../../_models/photo';
import { SharedModule } from '../../_modules/shared.module';

@Component({
  selector: 'app-photo-management',
  standalone: true,
  imports: [SharedModule],
  templateUrl: './photo-management.component.html',
  styleUrl: './photo-management.component.css',
})
export class PhotoManagementComponent {
  photoService = inject(PhotosService);

  photos: Photo[] = [];

  ngOnInit() {
    this.loadPhotos();
  }

  loadPhotos() {
    this.photoService.getPhotosToModerate().subscribe((photos) => {
      this.photos = photos;
    });
  }

  approvePhoto(photoId: number) {
    this.photoService.approvePhoto(photoId).subscribe(() => {
      this.photos = this.photos.filter((x) => x.id !== photoId);
    });
  }

  rejectPhoto(photoId: number) {
    this.photoService.rejectPhoto(photoId).subscribe(() => {
      this.photos = this.photos.filter((x) => x.id !== photoId);
    });
  }
}
