import { Component, inject, TemplateRef } from '@angular/core';
import { User } from '../../_models/user';
import { AdminService } from '../admin.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { RolesModalComponent } from '../../modals/roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.css',
  providers: [BsModalService],
})
export class UserManagementComponent {
  modalRef?: BsModalRef;
  users: Partial<User[]> | undefined;

  constructor(
    private modalService: BsModalService,
    private adminService: AdminService
  ) {}

  ngOnInit() {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    this.adminService.getUserWithRoles().subscribe((users) => {
      this.users = users;
      console.log(this.users);
    });
  }

  openRolesModal(user: User) {
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        user: user,
        roles: this.getRolesArray(user),
      },
    };

    this.modalRef = this.modalService.show(RolesModalComponent, config);

    this.modalRef.content.updateSelectedRoles.subscribe((values: any) => {
      const rolesToUpdate = {
        roles: [
          ...values
            .filter((el: any) => el.checked === true)
            .map((el: any) => el.name),
        ],
      };

      if (rolesToUpdate) {
        this.adminService
          .updateUserRoles(user.username, rolesToUpdate.roles)
          .subscribe(() => {
            user.roles = [...rolesToUpdate.roles];
          });
      }
    });
  }

  private getRolesArray(user: User) {
    const roles: any[] = [];
    const userRoles = user.roles;

    const availableRoles: any[] = [
      { name: 'Admin', value: 'Admin' },
      { name: 'Moderator', value: 'Moderator' },
      { name: 'Member', value: 'Member' },
    ];

    availableRoles.forEach((role) => {
      let isMatch = false;

      for (const userRole of userRoles) {
        if (role.name === userRole) {
          isMatch = true;
          role.checked = true;
          roles.push(role);
          break;
        }
      }

      if (!isMatch) {
        role.checked = false;
        roles.push(role);
      }
    });

    return roles;
  }
}
