import { Component, OnInit } from '@angular/core';
import { Router, UrlSerializer } from '@angular/router';
import { Observable, of } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {}
  constructor(public accountService: AccountService, private router: Router, private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  login() {
    console.log(this.model);
    this.accountService.login(this.model).subscribe({
      next: () => {
        this.toastr.success("Login successful"),
        this.router.navigateByUrl("/members") // what happens if success (200) when logging in
      },
      error: error => {
        console.log(error.error) // what happens if failure (4**)
        this.toastr.error(error.error)
      }
    })
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl("/home")

  }
}
