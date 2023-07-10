import { Injectable } from '@angular/core';
import { NgxSpinner, NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  busyRequestCount = 0; // When a request occurs, we increment a request count
  constructor(private spinnerService: NgxSpinnerService) { }

  busy() {
    this.busyRequestCount++;
    this.spinnerService.show(undefined, {
      type: 'line-scale-party',
      bdColor: 'rgba(255,255,255,0)', // Set background of spinner to white
      color: "#1234fa" // Set the spinner color to dark gray
    })
  }

  idle() {
    this.busyRequestCount--; // decrement
    if (this.busyRequestCount <= 0) {
      this.busyRequestCount = 0;
      this.spinnerService.hide(); // Hide the service if the request count is <0
    }
  }
}
