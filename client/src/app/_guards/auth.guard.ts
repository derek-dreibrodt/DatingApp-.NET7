import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { map, Observable, of } from 'rxjs';
import { AccountService } from '../_services/account.service';

@Injectable({
  providedIn: 'root'
})
// AuthGuards are not secure - they simply hide
export class AuthGuard implements CanActivate {
  constructor(private accountService: AccountService, 
    private toastr :ToastrService // use for toast message when trying to go to the wrong route
    ) {}
  
  canActivate(): Observable<boolean> {
    return this.accountService.currentUser$.pipe(
      map(user => {
        if (user) return true;
        else {
          // if user is not logged in, do not allow to pass - set canActivate to false
          this.toastr.error('You shall not pass!')
          return false;
        }
      })
      );
  }
  
}
