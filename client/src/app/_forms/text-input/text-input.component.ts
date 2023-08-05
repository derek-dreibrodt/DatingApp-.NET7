import { Component, Input, OnInit, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css']
})
export class TextInputComponent implements ControlValueAccessor { // create custom form control directives that integrate with angular forms
  @Input() label = '';
  @Input() type = 'text'; // These are default inputs, but we can change the values still

  constructor(@Self() public ngControl: NgControl //We have been using form controls derived from NgControl in our forms
  // We use self because angular checks memory for recently injected values - we want to make sure this NgControl is unique by using @Self
  ) { 
    this.ngControl.valueAccessor = this; //initializes NgControl. Set the valueAccessor to the TextInputComponent
  }
  // What we are doing in our form will pass through these values without us having to do anything
  // We will have access to these functions, but their functionality is controlled via the ControlValueAccessor
  writeValue(obj: any): void {
    
  }

  registerOnChange(fn: any): void {
    
  }

  registerOnTouched(fn: any): void {
    
  }

  get control(): FormControl { // when we try to access "control" it returns the get control value
    return this.ngControl.control as FormControl 
    // casts the ngControl.control into type FormControl as name "control" to get around around nullable errors
    // Allows us to use control instead of ngControl in the html because ngControl is nullable
  }

}
