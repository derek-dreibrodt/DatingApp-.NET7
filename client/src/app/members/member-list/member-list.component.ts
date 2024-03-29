import { Component, OnInit } from '@angular/core';
import { Observable, take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userParams';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css'],
})
export class MemberListComponent implements OnInit {
  //members$: Observable<Member[]> | undefined;
  members: Member[] = [];
  pagination: Pagination | undefined;
  userParams: UserParams | undefined;
  genderList = [
    {value: 'male', display: 'Males'},
    {value: 'female', display: 'Females'}
  ]
  

  constructor(private memberService: MembersService) {
    this.userParams = this.memberService.getUserParams();
  }

  ngOnInit(): void {
    this.loadMembers();
  } // Get the members from the members service
  // the members service is not destroyed when a component is destroyed

  loadMembers() {
    if(this.userParams) {
      this.memberService.setUserParams(this.userParams); // set the current search params when loading members in the member service (So page is remembered)
      this.memberService.getMembers(this.userParams).subscribe({
        next: response => {
          if(response.result && response.pagination) {
            this.members = response.result;
            this.pagination = response.pagination; 
          }
        }
      })
    }; // if no parameters, don't try to load the members
    
  }

  resetFilters() {
    this.userParams = this.memberService.resetUserParams(); // 
    this.loadMembers(); // load the new list of members based on the user params
    
  }

  pageChanged(event: any) { // if the current page number isn't equal to
    if (this.userParams && this.userParams?.pageNumber !== event.page ) {
      this.userParams.pageNumber = event.page;
      this.memberService.setUserParams(this.userParams);
      this.loadMembers();
    }
  }
}
