import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ToastrModule } from 'ngx-toastr';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { NgxGalleryModule } from '@kolkov/ngx-gallery';


// Lists out the 3rd party modules
@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    BsDropdownModule.forRoot(), // dropdown for "welcome {user}"
    TabsModule.forRoot(), // Allows creating ngx tabs
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right'}), // ToastrModule added for notifications on side
    NgxGalleryModule
  ],
  exports: [
    BsDropdownModule,
    ToastrModule,
    TabsModule,
    NgxGalleryModule
  ]
})
export class SharedModule { }
