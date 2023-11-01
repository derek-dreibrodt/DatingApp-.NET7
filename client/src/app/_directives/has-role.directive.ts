import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';
import { take } from 'rxjs';

@Directive({
  selector: '[appHasRole]' // structural directives use *appHasRole='["Admin","Thing"]' will make something display if they are in any roles
})
export class HasRoleDirective {
  @Input() appHasRole: string[] = [];
  user: User = {} as User; // create an empty user
  constructor(private viewContainerRef: ViewContainerRef, private templateRef: TemplateRef<any>,
    private accountService: AccountService) {
      this.accountService.currentUser$.pipe(take(1)).subscribe({
        next: user => {
          if (user) this.user = user
        }
      })
     }
  ngOnInit(): void {
    if (this.user.roles.some(r => this.appHasRole.includes(r))) {// if the user roles match, then display the content
      this.viewContainerRef.createEmbeddedView(this.templateRef); // if they are in the correct role, create the view from the template
    } else { // user doesn't have required role
      this.viewContainerRef.clear() // removes the element from the dom (the admin link)
    }

  }
}
