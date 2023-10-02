import { CommonModule, NgFor } from '@angular/common';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { TimeagoClock, TimeagoModule } from 'ngx-timeago';
import { Message } from 'src/app/_models/message';
import { MessagesService } from 'src/app/_services/messages.service';

@Component({
  selector: 'app-member-messages',
  standalone: true,
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  imports: [CommonModule, TimeagoModule, FormsModule]
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm? : NgForm;// Allows access to messageForm
  @Input() username?: string;
  @Input() messages: Message[] = [];
  messageContent = '';
  constructor(private messageService: MessagesService) { }

  ngOnInit(): void {
    
  }

  sendMessage() {
    if (!this.username) return;
    this.messageService.sendMessage(this.username, this.messageContent).subscribe({
      next: message => {
        this.messages.push(message); // adds it to the list of messages in the client
        this.messageForm?.reset();
      }

    })
  }
  

}
