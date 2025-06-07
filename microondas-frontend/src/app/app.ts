import { Component } from '@angular/core';
import { MicroondasComponent } from './microondas/microondas.component';

@Component({
  selector: 'app-root',
  imports: [
    MicroondasComponent
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected title = 'microondas-frontend';
}
