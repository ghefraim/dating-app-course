import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { map } from 'rxjs';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Photo } from '../_models/photo';

@Injectable({
  providedIn: 'root',
})
export class PhotosService {
  baseApiUrl = environment.baseApiUrl;
  http = inject(HttpClient);

  getPhotosToModerate() {
    return this.http
      .get<Photo[]>(this.baseApiUrl + 'admin/photos-to-moderate')
      .pipe(
        map((photos: any[]) => {
          return photos;
        })
      );
  }

  approvePhoto(photoId: number) {
    return this.http.put(
      this.baseApiUrl + `admin/approve-photo/${photoId}`,
      {}
    );
  }

  rejectPhoto(photoId: number) {
    return this.http.delete(this.baseApiUrl + `admin/reject-photo/${photoId}`);
  }
}
