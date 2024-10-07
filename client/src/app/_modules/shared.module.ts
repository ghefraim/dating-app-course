import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { NgxSpinnerModule } from 'ngx-spinner';

@NgModule({
  declarations: [],
  imports: [CommonModule, BsDropdownModule, TabsModule, NgxSpinnerModule],
  exports: [BsDropdownModule, TabsModule, NgxSpinnerModule],
})
export class SharedModule {}
