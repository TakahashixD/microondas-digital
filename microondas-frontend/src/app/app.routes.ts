import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { MicroondasComponent } from './microondas/microondas.component';
import { AuthGuard } from './auth/auth.guard';

export const routes: Routes = [
    {
        path: 'login',
        component: LoginComponent
    },
    {
        path: 'microondas',
        component: MicroondasComponent,
        canActivate: [AuthGuard],
    }
];
