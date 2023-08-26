import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { ToastrService } from 'ngx-toastr';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-card', // what to type to put this element in
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'], // file with the style of the component
  encapsulation: ViewEncapsulation.Emulated, //default value, determined scope of CSS styles by encapsulating them
})
export class MemberCardComponent implements OnInit {
  @Input() member: Member | undefined;

  constructor(private memberService: MembersService, private toastr : ToastrService) {}

  ngOnInit(): void {}

  addLike(member: Member) {
    this.memberService.addLike(member.userName).subscribe({
      next: () => this.toastr.success('You have liked ' + member.knownAs)
    })
  }
}
