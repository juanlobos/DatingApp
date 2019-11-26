import { Injectable } from '@angular/core';
import { User } from 'src/_models/user';
import { Resolve, Router, ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class MemberListResolver implements Resolve<User[]> {
    constructor(private UserSevice: UserService, private router: Router, private alertify: AlertifyService) { }

    resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
        return this.UserSevice.getUsers().pipe(
            catchError(error => {
                this.alertify.error('Problema al recibir data');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}

