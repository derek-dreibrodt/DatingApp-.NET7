import { Component, OnInit } from '@angular/core';
import { Member } from 'src/app/_modules/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members: Member[] = [];

  constructor(private memberService: MembersService) { }

  ngOnInit(): void {
    this.loadMembers() // calls the method to load members on creation of the list of members
  }

  loadMembers() {
    // loads the members by subscribing to the memberService's getMembers
    // getMembers() just gets a list of the members from the API with the HTTP Authorization options (JSON auth token)
    this.memberService.getMembers().subscribe({
      next: members => this.members = members
    })
  }

}
