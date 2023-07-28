import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_modules/member';
import { map, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];

  constructor(private http: HttpClient) { }

  getMembers() {
    if (this.members.length > 0) return of(this.members); // Check if the members are "cached"
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe( // If not "cached", get the list of members
      map(members => {
        this.members = members;
        return members
      })
    );
  }

  getMember(username: string) { // Get the member and "cache"
    const member = this.members.find(x => x.userName == username);
    if (member) return of(member); // If they are already loaded, return them
    return this.http.get<Member>(this.baseUrl + 'users/' + username) // Do not cache all members if just getting 1
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member) // tells us the index of the member in the "cached" array
        // the spread operator "..." takes the properties of this.getMembers[index] aka member and "spreads" them. 
        // It then spreads the "member" (updated) properties over the older properties
        this.members[index] = {...this.members[index], ...member} 
        
      })
    );
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }
}
