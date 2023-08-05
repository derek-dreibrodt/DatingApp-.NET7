import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';
import { AbstractControl, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  model: any = {}
  constructor(private accountService: AccountService, private toastr: ToastrService) { }
  registerForm: FormGroup = new FormGroup({}); // need to make form optional undefined if not instantiatign at this point
  // Form parts will be form controls
  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.registerForm = new FormGroup({
      username: new FormControl('', Validators.required),
      password: new FormControl('', 
        [Validators.required,  // Set validator state
        Validators.minLength(4), 
        Validators.maxLength(8)]),
      confirmPassword: new FormControl('', [Validators.required, this.matchValues('password')]),
    })
    this.registerForm.controls['password'].value.subscribe({ // subscribe to the value of the form's password
      next: () => this.registerForm.controls['confirmPassword'] .updateValueAndValidity() // Check the validity when ??? is changed
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
    console.log(this.registerForm?.value)

    // this.accountService.register(this.model).subscribe({
    //   next: () => {
    //     this.cancel();

    //   },
    //   error: error => {
    //     console.log(error.error) // what happens if failure (4**)
    //     this.toastr.error(error.error)
    //   }
    // })
  }

  cancel() {
    console.log('registration cancelled')
    this.cancelRegister.emit(false)
  }
}
