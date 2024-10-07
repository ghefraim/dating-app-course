import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { NgxSpinnerModule } from 'ngx-spinner';
import { FileUploadModule } from 'ng2-file-upload';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    BsDropdownModule,
    TabsModule,
    NgxSpinnerModule,
    FileUploadModule,
  ],
  exports: [BsDropdownModule, TabsModule, NgxSpinnerModule, FileUploadModule],
})
export class SharedModule {}
