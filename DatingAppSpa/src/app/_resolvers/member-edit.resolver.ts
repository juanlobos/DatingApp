import { Injectable } from '@angular/core';
import { User } from 'src/_models/user';
import { Resolve, Router, ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';

@Injectable()
export class MemberEditlResolver implements Resolve<User> {
    constructor(private UserSevice: UserService, private authService: AuthService,
                private router: Router, private alertify: AlertifyService) { }

    resolve(route: ActivatedRouteSnapshot): Observable<User> {
        return this.UserSevice.getUser(this.authService.decodedToken.nameid).pipe(
            catchError(error => {
                this.alertify.error('Problema al recibir data');
                this.router.navigate(['/members']);
                return of(null);
            })
        );
    }
}

