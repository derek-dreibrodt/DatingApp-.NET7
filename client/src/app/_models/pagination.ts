export interface Pagination {
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
}


export class PaginatedResult<T> {// Make it general so we can paginate anything
    result?: T; // will be list of items
    pagination?: Pagination;
}