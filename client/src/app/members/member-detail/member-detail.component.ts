import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {
  NgxGalleryAnimation,
  NgxGalleryImage,
  NgxGalleryModule,
  NgxGalleryOptions,
} from '@kolkov/ngx-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessagesService } from 'src/app/_services/messages.service';
import { Message } from 'src/app/_models/message';
import { MemberEditComponent } from '../member-edit/member-edit.component';

@Component({
  selector: 'app-member-detail',
  standalone: true, // makes the imports not separate
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [CommonModule, TabsModule, NgxGalleryModule, TimeagoModule, MemberMessagesComponent]
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs', {static: true}) memberTabs?: TabsetComponent;
  
  member: Member = {} as Member; //initialized the member as an empty member
  activeTab?: TabDirective;
  messages: Message[] = [];


  galleryOptions: NgxGalleryOptions[] = [];
  galleryImages: NgxGalleryImage[] = [];
  constructor(
    private memberService: MembersService,
    private route: ActivatedRoute,
    private messageService: MessagesService
  ) {}

  ngOnInit(): void { // happens before view is initialized
    this.route.data.subscribe({
      next: data => this.member = data['member'] // matches what we called the parameter in the resolve: {member} in app.routing
    })
    this.route.queryParams.subscribe({
      next: params => {
        params['tab'] && this.selectTab(params['tab']) // If 'tab' is in params, select the tab
      }
    })
    this.galleryImages = this.getImages();
    this.galleryOptions = [
      {
        width: '600px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
      },
    ];

    //this.galleryImages = this.getImages(); // this method hasn't loaded the member yet
  }

  selectTab(heading: string) {
    if (this.memberTabs)
    {
      this.memberTabs.tabs.find(x => x.heading == heading)!.active = true; // ! turns off typescript
    }
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data; // when tab is clicked on
    if (this.activeTab.heading === "Messages") {
      this.loadMessages();
    }
  }

  loadMessages() {
    if (this.member) {
      this.messageService.getMessageThread(this.member.userName).subscribe({
        next: messages => this.messages = messages
      })
    }
  }

  getImages() {
    if (!this.member) return []; // if the member object in this object is not there, do not get the images
    const imageUrls = [];
    for (const photo of this.member.photos) {
      imageUrls.push({
        small: photo.url,
        medium: photo.url,
        big: photo.url,
      });
    }
    return imageUrls;
  }


}
