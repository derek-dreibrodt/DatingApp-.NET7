
import { Component, OnInit } from '@angular/core';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  // Typicall ordered 1. properties 2. constructor 3. methods
  title = 'Dating App!';
  constructor(private accountService: AccountService) {}
  ngOnInit(): void {
    this.setCurrentUser(); // sets user to token saved in local storage (browser storage)
  }

  
  
  setCurrentUser() {
    const userString = localStorage.getItem('user');
    if (!userString) return;
    const user: User = JSON.parse(userString);
    //console.log(user)
    this.accountService.setCurrentUser(user);
  }
}
