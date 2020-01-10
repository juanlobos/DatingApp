import { Component, OnInit } from '@angular/core';
import { AuthService } from './_services/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { User } from 'src/_models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'DatingAppSpa';
  jwtHelper = new JwtHelperService();

  constructor(private autService: AuthService) {}

  ngOnInit() {
    const token = localStorage.getItem('token');
    const user: User = JSON.parse(localStorage.getItem('user'));
    if (token) {
      this.autService.decodedToken = this.jwtHelper.decodeToken(token);
    }
    if (user) {
      this.autService.currentUser = user;
      this.autService.changeMemberPhoto(user.photoUrl);
    }
  }
}
