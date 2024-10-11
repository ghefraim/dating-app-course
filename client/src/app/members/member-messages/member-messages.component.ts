import { Component, Input, ViewChild } from '@angular/core';
import { Message } from '../../_models/message';
import { MessageService } from '../../_services/message.service';
import { SharedModule } from '../../_modules/shared.module';

@Component({
  selector: 'app-member-messages',
  standalone: true,
  imports: [SharedModule],
  templateUrl: './member-messages.component.html',
  styleUrl: './member-messages.component.css',
})
export class MemberMessagesComponent {
  @Input() username: string | undefined;
  @Input() messages: Message[] = [];
  @ViewChild('messageForm') messageForm: any;

  messageContent: string | undefined;

  constructor(public messageService: MessageService) {}

  ngOnInit() {}

  sendMessage() {
    this.messageService
      .sendMessage(this.username!, this.messageContent!)
      .then(() => {
        this.messageForm.reset();
      });
  }
}
