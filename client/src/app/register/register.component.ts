import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';

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
      username: new FormControl('Hello', Validators.required),
      password: new FormControl('Hello', 
        [Validators.required, 
        Validators.minLength(4), 
        Validators.maxLength(8)]),
      confirmPassword: new FormControl(),
      
    })
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
