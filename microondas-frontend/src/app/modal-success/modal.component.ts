import { Component, EventEmitter, Input, Output, signal } from '@angular/core';
import { MicroondasService, ProgramaAquecimento } from '../microondas/microondas.service';
import { FormsModule } from '@angular/forms';

@Component({
    selector: 'app-modal',
    imports: [FormsModule],
    templateUrl: './modal.component.html',
    styleUrls: ['./modal.component.scss']
})
export class ModalComponent {
    @Input() isVisible: boolean = false;
    @Output() onClose = new EventEmitter<void>();
    @Output() onSave = new EventEmitter<ProgramaAquecimento>();

    @Input() mostrarModalSucesso: boolean = false;
    ultimoProgramaCriado: ProgramaAquecimento | null = null;

    novoPrograma: Omit<ProgramaAquecimento, 'id'> = {
        nome: '',
        alimento: '',
        tempo: 30,
        potencia: 10,
        caractereAquecimento: '.',
        instrucoes: '',
        customizado: true
    };

    constructor(private microondasService: MicroondasService) { }

    fecharModal(): void {
        if (!this.mostrarModalSucesso) {
            this.resetarFormulario();
            this.onClose.emit();
        }
    }

    salvarPrograma(): void {
        if (this.validarFormulario()) {
            const programaCompleto: ProgramaAquecimento = this.novoPrograma;
            this.ultimoProgramaCriado = programaCompleto;
            this.onSave.emit(programaCompleto);
            this.resetarFormulario();
            this.onClose.emit();
        }
    }

    formatarTempo(tempo: string | undefined): string {
        return this.microondasService.formatarTempo(tempo);
    }

    fecharModalSucesso(): void {
        this.mostrarModalSucesso = false;
        this.ultimoProgramaCriado = null;
        this.onClose.emit();
    }

    private validarFormulario(): boolean {
        return !!(
            this.novoPrograma.nome.trim() &&
            this.novoPrograma.alimento.trim() &&
            this.novoPrograma.tempo > 0 &&
            this.novoPrograma.potencia >= 1 && this.novoPrograma.potencia <= 10 &&
            this.novoPrograma.caractereAquecimento.trim() &&
            this.novoPrograma.instrucoes.trim()
        );
    }

    private resetarFormulario(): void {
        this.novoPrograma = {
            nome: '',
            alimento: '',
            tempo: 30,
            potencia: 10,
            caractereAquecimento: '.',
            instrucoes: '',
            customizado: true
        };
    }
}