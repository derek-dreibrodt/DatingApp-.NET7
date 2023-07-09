import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { Observable } from 'rxjs';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

@Injectable({
  providedIn: 'root'
})
// We specify the component we are preventing from deactivating
export class PreventUnsavedChangesGuard implements CanDeactivate<MemberEditComponent> {
  
  // Return a boolean : Can they move forward or are they stuck here?
  canDeactivate(
    component: MemberEditComponent): boolean {
    if (component.editForm?.dirty) {
      return confirm('Are you sure you want to continue? Any unsaved changes will be lost');
    } else {
      return true;
    }
  }
  
}
