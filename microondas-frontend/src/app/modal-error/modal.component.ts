// modal-erro.component.ts
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-modal-erro',
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.scss']
})
export class ModalErroComponent {
  @Input() isVisible: boolean = false;
  @Input() mensagemErro: string = '';
  @Input() mostrarBotaoTentarNovamente: boolean = true;
  
  @Output() onClose = new EventEmitter<void>();
  @Output() onRetry = new EventEmitter<void>();

  constructor() {}

  fecharModal(): void {
    this.onClose.emit();
  }

  tentarNovamente(): void {
    this.onRetry.emit();
  }
}