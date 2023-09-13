import { HttpClient, HttpParams } from "@angular/common/http";
import { PaginatedResult } from "../_models/pagination";
import { map } from "rxjs";

// Moved
export function getPaginatedResults<T>(url: string, params: HttpParams, http: HttpClient) {
    const paginatedResult: PaginatedResult<T[]> = new PaginatedResult<T[]>;
    return http.get<T[]>(url, { observe: 'response', params }).pipe(
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

export function getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams(); // allows us to send http params with our request

    params = params.append('pageNumber', pageNumber);
    params = params.append('pageSize', pageSize);
    
    return params;
  }