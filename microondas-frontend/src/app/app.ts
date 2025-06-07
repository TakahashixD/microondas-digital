import { Component } from '@angular/core';
import { MicroondasComponent } from './microondas/microondas.component';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected title = 'microondas-frontend';
}
