import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup = new FormGroup({}); // need to make form optional undefined if not instantiatign at this point
  maxDate: Date = new Date();
  validationErrors: string[] | undefined; // holds errors for validation


  constructor(private accountService: AccountService, 
    private toastr: ToastrService, 
    private fb: FormBuilder,
    private router: Router,
    ) { }
  

  
  // Form parts will be form controls
  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18)
  }

  initializeForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],

      username: ['', Validators.required],

      knownAs: ['', Validators.required],

      dateOfBirth: ['', Validators.required],

      city: ['', Validators.required],

      country: ['', Validators.required],


      password: ['', 
        [Validators.required,  // Set validator state
        Validators.minLength(4), 
        Validators.maxLength(8)
      ]],

      confirmPassword: ['', 
        [Validators.required, 
          this.matchValues('password')
      ]],
    })
    this.registerForm.controls['password'].valueChanges.subscribe({ // subscribe to the value of the form's password
      next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity() // Check the validity when ??? is changed
    })
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === 
        control. // compare one control's value to the value of the control's parent value
        parent?. // Compare the value to the parent
        get(matchTo)?. // See if the parent's value matches
        value ? null: {notMatching: true} // If it is the alternate case with no case, we set property of notMatching to true
    }
  }

  register() {
    
    const dob = this.getDateOnly(this.registerForm.controls['dateOfBirth'].value);
    const values = {... this.registerForm.value, dateOfBirth: dob};
    //console.log(values)
    this.accountService.register(values).subscribe({
      next: () => {
        this.router.navigateByUrl('/members');

      },
      error: error => {
        this.validationErrors = error // contains an array of errors that we get from our server
      }
    })
  }

  cancel() {
    console.log('registration cancelled')
    this.cancelRegister.emit(false)
  }

  private getDateOnly(dob: string | undefined) {
    if (!dob) return;
    let theDob = new Date(dob);
    return new Date(theDob
      .setMinutes(theDob.getMinutes()-theDob.getTimezoneOffset())) // Removes the minutes from the date
    .toISOString().slice(0,10) // Gives us just the date portion of the ISO string
  }
}
