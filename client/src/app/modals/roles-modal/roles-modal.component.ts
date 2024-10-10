import { Component, EventEmitter, Input } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { User } from '../../_models/user';
import { SharedModule } from '../../_modules/shared.module';

@Component({
  selector: 'app-roles-modal',
  standalone: true,
  imports: [SharedModule],
  templateUrl: './roles-modal.component.html',
  styleUrl: './roles-modal.component.css',
})
export class RolesModalComponent {
  @Input() updateSelectedRoles = new EventEmitter();
  user?: User;
  roles: any[] = [];

  constructor(public modalRef: BsModalRef) {}

  updateRoles() {
    this.updateSelectedRoles.emit(this.roles);
    this.modalRef.hide();
  }
}
