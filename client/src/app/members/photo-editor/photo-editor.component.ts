import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs';
import { User } from 'src/app/_models/user';
import { Member } from 'src/app/_modules/member';
import { AccountService } from 'src/app/_services/account.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input () member: Member | undefined;
  uploader : FileUploader | undefined;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  user: User | undefined

  constructor(private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) this.user = user; // Get the user
        
      }
    })
   }

  ngOnInit(): void {
    this.intializeUploader()
  }

  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e;
  }

  intializeUploader() {
    this.uploader = new FileUploader({
      url : this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.user?.token, // made outside of http service so we need to manually add auth token
      isHTML5: true,
      allowedFileType: ['image'], // Allows all image types
      removeAfterUpload: true,
      autoUpload: false, // users will have to press a button to upload
      maxFileSize: 10 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false // Without this we would need to adjust our course configuration
    }

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if(response) {
        const photo = JSON.parse(response);
        this.member?.photos.push(photo);// If we successfully upload a photo, add the photo to the current member
      }
    }
  }

}
