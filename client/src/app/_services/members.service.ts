import { HttpClient, HttpHeaders, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { map, of, take } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map(); // cache
  userParams: UserParams | undefined;
  user: User | undefined;

  

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) {
          this.userParams = new UserParams(user); // Set the parameters based on the user
          this.user = user;
        }
      }
    })
  }

  getUserParams() {
      return this.userParams;
  }

  setUserParams(params: UserParams) {
    this.userParams = params;
  }

  resetUserParams() {
    if (this.user) {
      this.userParams = new UserParams(this.user);
      return this.userParams;
    }
    return;
  }

  getMembers(userParams: UserParams) {
    const response = this.memberCache.get(Object.values(userParams).join('-'));

    if (response) return of(response);


    let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize);

    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);


    // if (this.members.length > 0) return of(this.members); // Check if the members are "cached"

    return this.getPaginatedResults<Member>(this.baseUrl + 'users', params).pipe(
      map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response); // cache the response with each call
        return response;
      })
    ); // Get a paginated list of users + pagination header (has extra values)
  }

  

  getMember(username: string) {
    // Get the member and "cache"
    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find((member: Member) => member.userName === username);
    console.log(member)
    if (member) return of(member); // If they are already loaded, return them
    return this.http.get<Member>(this.baseUrl + 'users/' + username); // Do not cache all members if just getting 1
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member); // tells us the index of the member in the "cached" array
        // the spread operator "..." takes the properties of this.getMembers[index] aka member and "spreads" them.
        // It then spreads the "member" (updated) properties over the older properties
        this.members[index] = { ...this.members[index], ...member };
      })
    );
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId); // call http service and delete in database via url
  }

  addLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {});
  }

  getLikes(predicate: string) {
    return this.http.get(this.baseUrl + 'likes?predicate=' + predicate)
  }

  private getPaginatedResults<T>(url: string, params: HttpParams) {
    const paginatedResult: PaginatedResult<T[]> = new PaginatedResult<T[]>;
    return this.http.get<T[]>(url, { observe: 'response', params }).pipe(
      map(response => { // project response
        if (response.body) {
          paginatedResult.result = response.body; // Get the list of paginated items
        }
        const pagination = response.headers.get('Pagination'); // Gets the Pagination header value
        if (pagination) {
          paginatedResult.pagination = JSON.parse(pagination); // Set the pagination header value
        }
        return paginatedResult;
      })
    );
  }

  private getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams(); // allows us to send http params with our request

    params = params.append('pageNumber', pageNumber);
    params = params.append('pageSize', pageSize);
    
    return params;
  }
}
