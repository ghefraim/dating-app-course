import {
  ApplicationConfig,
  importProvidersFrom,
  //importProvidersFrom,
  //provideZoneChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';
import { routes } from './app.routes';
import {
  provideHttpClient,
  withInterceptors,
  withInterceptorsFromDi,
} from '@angular/common/http';
import { provideToastr } from 'ngx-toastr';
import { errorInterceptor } from './_interceptors/error.interceptor';
import { jwtInterceptor } from './_interceptors/jwt.interceptor';
import { loadingInterceptor } from './_interceptors/loading.interceptor';
import { TimeagoClock, TimeagoIntl, TimeagoModule } from 'ngx-timeago';
import { interval, Observable } from 'rxjs';

export class MyClock extends TimeagoClock {
  tick(then: number): Observable<number> {
    return interval(60000);
  }
}

export const appConfig: ApplicationConfig = {
  providers: [
    //provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),
    provideAnimations(),
    provideToastr({
      positionClass: 'toast-bottom-right',
      preventDuplicates: false,
    }),
    provideHttpClient(
      withInterceptors([errorInterceptor, jwtInterceptor, loadingInterceptor])
    ),
    importProvidersFrom(
      TimeagoModule.forRoot({
        intl: { provide: TimeagoClock, useClass: MyClock },
      })
    ),
  ],
};
