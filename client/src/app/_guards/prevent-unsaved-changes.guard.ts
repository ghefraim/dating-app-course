import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

export const preventUnsavedChangesGuard: CanDeactivateFn<
  MemberEditComponent
> = (component: MemberEditComponent): boolean => {

  if (component.editForm && component.editForm.dirty) {
    return confirm(
      'Are you sure you want to continue? Any unsaved changes will be lost'
    );
  }
  return true;
};


// export const preventUnsavedChangesGuard: CanDeactivateFn<MemberEditComponent> = (
//   component: MemberEditComponent
// ): Observable<boolean> | boolean => {
//   const confirmService = inject(ConfirmService);

//   if (component.editForm && component.editForm.dirty) {
//     if (component.editForm.dirty) {
//       return confirmService.confirm();
//     }
//   }
//   return true;
// };

