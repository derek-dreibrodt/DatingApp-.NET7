import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from 'src/app/_modules/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members$: Observable<Member[]> | undefined;

  constructor(private memberService: MembersService) { }

  ngOnInit(): void {
    this.members$ = this.memberService.getMembers(); // calls the method to load members on creation of the list of members
  } // Get the members from the members service
    // the members service is not destroyed when a component is destroyed

}
